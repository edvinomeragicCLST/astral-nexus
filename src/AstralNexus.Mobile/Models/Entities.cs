using SQLite;

namespace AstralNexus.Mobile.Models;

[Table("Users")]
public class UserEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed(Unique = true), MaxLength(160)] public string Email { get; set; } = string.Empty;
    [MaxLength(80)] public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public int Coins { get; set; } = 500;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

[Table("Cards")]
public class CardEntity
{
    [PrimaryKey] public int Id { get; set; }
    [Indexed, MaxLength(120)] public string ExternalKey { get; set; } = string.Empty;
    [Indexed, MaxLength(120)] public string Name { get; set; } = string.Empty;
    [Indexed, MaxLength(80)] public string Type { get; set; } = string.Empty;
    public int BaseValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageFile { get; set; } = "logo.png";
}

[Table("UserCards")]
public class UserCardEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }
    [Indexed] public int CardId { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime AcquiredAtUtc { get; set; } = DateTime.UtcNow;
}

[Table("Decks")]
public class DeckEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }
    [MaxLength(80)] public string Name { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

[Table("DeckCards")]
public class DeckCardEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int DeckId { get; set; }
    [Indexed] public int CardId { get; set; }
    public int SortOrder { get; set; }
}

[Table("PackOpenings")]
public class PackOpeningEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }
    public int Cost { get; set; }
    public int DuplicateCoins { get; set; }
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
}

[Table("Matches")]
public class MatchRecordEntity
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int UserId { get; set; }
    public int? DeckId { get; set; }
    [MaxLength(20)] public string Result { get; set; } = string.Empty;
    public int TurnsPlayed { get; set; }
    public int CoinsAwarded { get; set; }
    public DateTime PlayedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class OwnedCardItem
{
    public CardEntity Card { get; init; } = new();
    public int Quantity { get; init; }
    public int Id => Card.Id;
    public string Name => Card.Name;
    public string Type => Card.Type;
    public int BaseValue => Card.BaseValue;
    public string Description => Card.Description;
    public string ImageFile => Card.ImageFile;
    public string QuantityText => $"x{Quantity}";
    public string Meta => $"{Type} • vrijednost {BaseValue}";
}

public sealed class DeckSummaryItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CardCount { get; init; }
    public bool IsValid { get; init; }
    public string StatusText => IsValid ? $"Validan • {CardCount}/20" : $"Nije validan • {CardCount}/20";
}

public sealed record AppStats(int Coins, int OwnedCards, int Decks, int Wins, int Losses, int PackOpenings)
{
    public string WinLoss => $"{Wins} / {Losses}";
}

public sealed class PackResultCard
{
    public CardEntity Card { get; init; } = new();
    public bool WasDuplicate { get; init; }
    public string DuplicateText => WasDuplicate ? "Duplikat +1 coin" : "Nova karta";
}

public sealed class PackOpenResult
{
    public IReadOnlyList<PackResultCard> Cards { get; init; } = Array.Empty<PackResultCard>();
    public int DuplicateCoins { get; init; }
    public int RemainingCoins { get; init; }
}

public sealed class HistoryItem
{
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public DateTime WhenUtc { get; init; }
    public string DateText => WhenUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
}

public sealed class CardSeedDto
{
    public int Id { get; set; }
    public string ExternalKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int BaseValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageFile { get; set; } = string.Empty;
}

public sealed class ApiCardDto
{
    public int Id { get; set; }
    public string ExternalKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int BaseValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
