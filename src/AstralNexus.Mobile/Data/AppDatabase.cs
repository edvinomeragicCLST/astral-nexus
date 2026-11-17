using System.Text.Json;
using AstralNexus.Mobile.Models;
using AstralNexus.Mobile.Services;
using SQLite;

namespace AstralNexus.Mobile.Data;

public sealed class AppDatabase
{
    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    private async Task<SQLiteAsyncConnection> DbAsync()
    {
        if (_initialized && _db is not null) return _db;
        await _initLock.WaitAsync();
        try
        {
            if (_initialized && _db is not null) return _db;
            var path = Path.Combine(FileSystem.AppDataDirectory, "astral_nexus_mobile.db3");
            _db = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await _db.CreateTableAsync<UserEntity>();
            await _db.CreateTableAsync<CardEntity>();
            await _db.CreateTableAsync<UserCardEntity>();
            await _db.CreateTableAsync<DeckEntity>();
            await _db.CreateTableAsync<DeckCardEntity>();
            await _db.CreateTableAsync<PackOpeningEntity>();
            await _db.CreateTableAsync<MatchRecordEntity>();
            await SeedCardsAsync(_db);
            await SeedDemoUserAsync(_db);
            _initialized = true;
            return _db;
        }
        finally { _initLock.Release(); }
    }

    private static async Task SeedCardsAsync(SQLiteAsyncConnection db)
    {
        if (await db.Table<CardEntity>().CountAsync() > 0) return;
        await using var stream = await FileSystem.OpenAppPackageFileAsync("cards_mobile.json");
        var seed = await JsonSerializer.DeserializeAsync<List<CardSeedDto>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        foreach (var c in seed)
        {
            await db.InsertAsync(new CardEntity { Id = c.Id, ExternalKey = c.ExternalKey, Name = c.Name, Type = c.Type, BaseValue = c.BaseValue, Description = c.Description, ImageFile = c.ImageFile });
        }
    }

    private static async Task SeedDemoUserAsync(SQLiteAsyncConnection db)
    {
        if (await db.Table<UserEntity>().Where(x => x.Email == "demo@astralnexus.local").FirstOrDefaultAsync() is not null) return;
        var (hash, salt) = PasswordService.Hash("Demo123!");
        var user = new UserEntity { Email = "demo@astralnexus.local", DisplayName = "Demo Igrač", PasswordHash = hash, PasswordSalt = salt, Coins = 500 };
        await db.InsertAsync(user);
        var starter = await db.Table<CardEntity>().OrderBy(x => x.Id).Take(35).ToListAsync();
        foreach (var card in starter) await db.InsertAsync(new UserCardEntity { UserId = user.Id, CardId = card.Id, Quantity = 1 });
    }

    public async Task<UserEntity?> LoginAsync(string email, string password)
    {
        var db = await DbAsync();
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Table<UserEntity>().Where(x => x.Email == normalized).FirstOrDefaultAsync();
        return user is not null && PasswordService.Verify(password, user.PasswordHash, user.PasswordSalt) ? user : null;
    }

    public async Task<(bool Success, string Error, UserEntity? User)> RegisterAsync(string name, string email, string password)
    {
        var db = await DbAsync();
        var normalized = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(normalized)) return (false, "Ime i email su obavezni.", null);
        if (password.Length < 6) return (false, "Lozinka mora imati najmanje 6 znakova.", null);
        if (await db.Table<UserEntity>().Where(x => x.Email == normalized).CountAsync() > 0) return (false, "Korisnik sa tim emailom već postoji.", null);
        var (hash, salt) = PasswordService.Hash(password);
        var user = new UserEntity { DisplayName = name.Trim(), Email = normalized, PasswordHash = hash, PasswordSalt = salt, Coins = 400 };
        await db.InsertAsync(user);
        var starter = await db.Table<CardEntity>().OrderBy(x => x.Id).Take(30).ToListAsync();
        foreach (var card in starter) await db.InsertAsync(new UserCardEntity { UserId = user.Id, CardId = card.Id, Quantity = 1 });
        return (true, string.Empty, user);
    }

    public async Task<UserEntity> RefreshUserAsync(int userId)
    {
        var db = await DbAsync();
        return await db.Table<UserEntity>().Where(x => x.Id == userId).FirstAsync();
    }

    public async Task UpdateProfileAsync(UserEntity user)
    {
        var db = await DbAsync();
        await db.UpdateAsync(user);
    }

    public async Task<List<OwnedCardItem>> GetCollectionAsync(int userId)
    {
        var db = await DbAsync();
        var owned = await db.Table<UserCardEntity>().Where(x => x.UserId == userId && x.Quantity > 0).ToListAsync();
        var cards = await db.Table<CardEntity>().ToListAsync();
        var byId = cards.ToDictionary(x => x.Id);
        return owned.Where(x => byId.ContainsKey(x.CardId)).Select(x => new OwnedCardItem { Card = byId[x.CardId], Quantity = x.Quantity }).OrderBy(x => x.Name).ToList();
    }

    public async Task<List<CardEntity>> GetAllCardsAsync()
    {
        var db = await DbAsync();
        return await db.Table<CardEntity>().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<List<DeckSummaryItem>> GetDecksAsync(int userId)
    {
        var db = await DbAsync();
        var decks = await db.Table<DeckEntity>().Where(x => x.UserId == userId).OrderBy(x => x.Name).ToListAsync();
        var allDeckCards = await db.Table<DeckCardEntity>().ToListAsync();
        return decks.Select(d => new DeckSummaryItem { Id = d.Id, Name = d.Name, IsValid = d.IsValid, CardCount = allDeckCards.Count(x => x.DeckId == d.Id) }).ToList();
    }

    public async Task<DeckEntity?> GetDeckAsync(int deckId)
    {
        var db = await DbAsync();
        return await db.Table<DeckEntity>().Where(x => x.Id == deckId).FirstOrDefaultAsync();
    }

    public async Task<List<CardEntity>> GetDeckCardsAsync(int deckId)
    {
        var db = await DbAsync();
        var links = await db.Table<DeckCardEntity>().Where(x => x.DeckId == deckId).OrderBy(x => x.SortOrder).ToListAsync();
        var cards = await db.Table<CardEntity>().ToListAsync();
        var byId = cards.ToDictionary(x => x.Id);
        return links.Where(x => byId.ContainsKey(x.CardId)).Select(x => byId[x.CardId]).ToList();
    }

    public async Task<(bool Success, string Error, int DeckId)> SaveDeckAsync(int userId, int? deckId, string name, IReadOnlyList<CardEntity> cards, DeckRulesService rules)
    {
        var db = await DbAsync();
        if (string.IsNullOrWhiteSpace(name)) return (false, "Naziv špila je obavezan.", 0);
        var validation = rules.Validate(cards);
        if (!validation.IsValid) return (false, string.Join("\n", validation.Errors), 0);
        var duplicateName = await db.Table<DeckEntity>().Where(x => x.UserId == userId && x.Name == name.Trim()).FirstOrDefaultAsync();
        if (duplicateName is not null && (!deckId.HasValue || duplicateName.Id != deckId.Value)) return (false, "Špil sa tim nazivom već postoji.", 0);

        DeckEntity deck;
        if (deckId.HasValue)
        {
            deck = await db.Table<DeckEntity>().Where(x => x.Id == deckId.Value && x.UserId == userId).FirstAsync();
            deck.Name = name.Trim(); deck.IsValid = true; deck.UpdatedAtUtc = DateTime.UtcNow;
            await db.UpdateAsync(deck);
            var old = await db.Table<DeckCardEntity>().Where(x => x.DeckId == deck.Id).ToListAsync();
            foreach (var link in old) await db.DeleteAsync(link);
        }
        else
        {
            deck = new DeckEntity { UserId = userId, Name = name.Trim(), IsValid = true };
            await db.InsertAsync(deck);
        }
        for (var i = 0; i < cards.Count; i++) await db.InsertAsync(new DeckCardEntity { DeckId = deck.Id, CardId = cards[i].Id, SortOrder = i });
        return (true, string.Empty, deck.Id);
    }

    public async Task DeleteDeckAsync(int userId, int deckId)
    {
        var db = await DbAsync();
        var deck = await db.Table<DeckEntity>().Where(x => x.Id == deckId && x.UserId == userId).FirstOrDefaultAsync();
        if (deck is null) return;
        var links = await db.Table<DeckCardEntity>().Where(x => x.DeckId == deckId).ToListAsync();
        foreach (var link in links) await db.DeleteAsync(link);
        var matches = await db.Table<MatchRecordEntity>().Where(x => x.DeckId == deckId).ToListAsync();
        foreach (var match in matches) { match.DeckId = null; await db.UpdateAsync(match); }
        await db.DeleteAsync(deck);
    }

    public async Task<PackOpenResult> OpenPackAsync(int userId)
    {
        const int cost = 100, count = 6;
        var db = await DbAsync();
        var user = await db.Table<UserEntity>().Where(x => x.Id == userId).FirstAsync();
        if (user.Coins < cost) throw new InvalidOperationException("Nemate dovoljno coina za otvaranje paketa.");
        var cards = await db.Table<CardEntity>().ToListAsync();
        if (cards.Count == 0) throw new InvalidOperationException("Katalog karata je prazan.");
        var owned = await db.Table<UserCardEntity>().Where(x => x.UserId == userId).ToListAsync();
        var byCard = owned.ToDictionary(x => x.CardId);
        user.Coins -= cost;
        var result = new List<PackResultCard>();
        var duplicates = 0;
        for (var i = 0; i < count; i++)
        {
            var card = cards[Random.Shared.Next(cards.Count)];
            var duplicate = byCard.TryGetValue(card.Id, out var uc) && uc.Quantity > 0;
            if (duplicate)
            {
                uc!.Quantity++; await db.UpdateAsync(uc); duplicates++; user.Coins += 1;
            }
            else
            {
                uc = new UserCardEntity { UserId = userId, CardId = card.Id, Quantity = 1 };
                await db.InsertAsync(uc); byCard[card.Id] = uc;
            }
            result.Add(new PackResultCard { Card = card, WasDuplicate = duplicate });
        }
        await db.UpdateAsync(user);
        await db.InsertAsync(new PackOpeningEntity { UserId = userId, Cost = cost, DuplicateCoins = duplicates });
        return new PackOpenResult { Cards = result, DuplicateCoins = duplicates, RemainingCoins = user.Coins };
    }

    public async Task<AppStats> GetStatsAsync(int userId)
    {
        var db = await DbAsync();
        var user = await db.Table<UserEntity>().Where(x => x.Id == userId).FirstAsync();
        return new AppStats(
            user.Coins,
            await db.Table<UserCardEntity>().Where(x => x.UserId == userId && x.Quantity > 0).CountAsync(),
            await db.Table<DeckEntity>().Where(x => x.UserId == userId).CountAsync(),
            await db.Table<MatchRecordEntity>().Where(x => x.UserId == userId && x.Result == "Win").CountAsync(),
            await db.Table<MatchRecordEntity>().Where(x => x.UserId == userId && x.Result == "Loss").CountAsync(),
            await db.Table<PackOpeningEntity>().Where(x => x.UserId == userId).CountAsync());
    }

    public async Task<int> SaveMatchAsync(int userId, int deckId, string result, int turns)
    {
        var db = await DbAsync();
        var normalized = result == "Win" ? "Win" : "Loss";
        var reward = normalized == "Win" ? 20 : 5 + Math.Max(1, turns) / 2;
        var user = await db.Table<UserEntity>().Where(x => x.Id == userId).FirstAsync();
        user.Coins += reward; await db.UpdateAsync(user);
        await db.InsertAsync(new MatchRecordEntity { UserId = userId, DeckId = deckId, Result = normalized, TurnsPlayed = Math.Max(1, turns), CoinsAwarded = reward });
        return reward;
    }

    public async Task<List<HistoryItem>> GetHistoryAsync(int userId)
    {
        var db = await DbAsync();
        var matches = await db.Table<MatchRecordEntity>().Where(x => x.UserId == userId).OrderByDescending(x => x.PlayedAtUtc).ToListAsync();
        var packs = await db.Table<PackOpeningEntity>().Where(x => x.UserId == userId).OrderByDescending(x => x.OpenedAtUtc).ToListAsync();
        return matches.Select(x => new HistoryItem { Title = x.Result == "Win" ? "Pobjeda" : "Poraz", Detail = $"{x.TurnsPlayed} poteza • +{x.CoinsAwarded} coina", WhenUtc = x.PlayedAtUtc })
            .Concat(packs.Select(x => new HistoryItem { Title = "Otvoren paket", Detail = $"Trošak {x.Cost} • povrat {x.DuplicateCoins} coina", WhenUtc = x.OpenedAtUtc }))
            .OrderByDescending(x => x.WhenUtc).ToList();
    }

    public async Task<int> SynchronizeCardsAsync(IReadOnlyList<ApiCardDto> remoteCards)
    {
        var db = await DbAsync();
        var local = await db.Table<CardEntity>().ToListAsync();
        var byExternal = local.ToDictionary(x => x.ExternalKey, StringComparer.OrdinalIgnoreCase);
        var updated = 0;
        foreach (var remote in remoteCards)
        {
            if (!byExternal.TryGetValue(remote.ExternalKey, out var card)) continue; // zadržava lokalni artwork
            card.Name = remote.Name; card.Type = remote.Type; card.BaseValue = remote.BaseValue; card.Description = remote.Description;
            await db.UpdateAsync(card); updated++;
        }
        return updated;
    }
}
