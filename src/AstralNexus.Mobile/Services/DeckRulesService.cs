using AstralNexus.Mobile.Models;

namespace AstralNexus.Mobile.Services;

public sealed class DeckRulesService
{
    public (bool IsValid, List<string> Errors) Validate(IReadOnlyCollection<CardEntity> cards)
    {
        var errors = new List<string>();
        if (cards.Count != 20) errors.Add("Špil mora sadržavati tačno 20 karata.");
        if (cards.Select(x => x.Id).Distinct().Count() != cards.Count) errors.Add("Ista karta ne može biti dodana više puta.");
        if (cards.Count(x => x.BaseValue == 3) > 3) errors.Add("Najviše 3 karte sa vrijednošću 3.");
        if (cards.Count(x => x.Type.Contains("mage", StringComparison.OrdinalIgnoreCase)) > 8) errors.Add("Najviše 8 Mage karata.");
        if (cards.Count(x => x.Type.Equals("sentinel", StringComparison.OrdinalIgnoreCase)) > 1) errors.Add("Najviše 1 Sentinel karta.");
        if (cards.Count(x => x.Type.Contains("effect", StringComparison.OrdinalIgnoreCase)) > 5) errors.Add("Najviše 5 Effect karata.");
        return (errors.Count == 0, errors);
    }

    public bool CanAdd(CardEntity candidate, IReadOnlyCollection<CardEntity> existing, out string reason)
    {
        if (existing.Count >= 20) { reason = "Špil već ima 20 karata."; return false; }
        if (existing.Any(x => x.Id == candidate.Id)) { reason = "Ova karta je već u špilu."; return false; }
        if (candidate.BaseValue == 3 && existing.Count(x => x.BaseValue == 3) >= 3) { reason = "Najviše 3 karte sa vrijednošću 3."; return false; }
        if (candidate.Type.Contains("mage", StringComparison.OrdinalIgnoreCase) && existing.Count(x => x.Type.Contains("mage", StringComparison.OrdinalIgnoreCase)) >= 8) { reason = "Najviše 8 Mage karata."; return false; }
        if (candidate.Type.Equals("sentinel", StringComparison.OrdinalIgnoreCase) && existing.Any(x => x.Type.Equals("sentinel", StringComparison.OrdinalIgnoreCase))) { reason = "Najviše 1 Sentinel karta."; return false; }
        if (candidate.Type.Contains("effect", StringComparison.OrdinalIgnoreCase) && existing.Count(x => x.Type.Contains("effect", StringComparison.OrdinalIgnoreCase)) >= 5) { reason = "Najviše 5 Effect karata."; return false; }
        reason = string.Empty;
        return true;
    }
}
