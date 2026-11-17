using System.Net.Http.Json;
using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Models;

namespace AstralNexus.Mobile.Services;

public sealed class CatalogSyncService(AppDatabase db)
{
    public async Task<int> SyncAsync(string baseUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new InvalidOperationException("Unesite REST API adresu.");
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var url = $"{baseUrl.TrimEnd('/')}/api/cards";
        var cards = await client.GetFromJsonAsync<List<ApiCardDto>>(url, cancellationToken) ?? [];
        return await db.SynchronizeCardsAsync(cards);
    }
}
