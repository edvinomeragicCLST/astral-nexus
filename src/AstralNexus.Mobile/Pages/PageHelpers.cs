using AstralNexus.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AstralNexus.Mobile.Pages;

internal static class PageHelpers
{
    public static T Resolve<T>(this IServiceProvider services) where T : notnull => services.GetRequiredService<T>();

    public static async Task<bool> EnsureLoggedInAsync(Page page, SessionService session)
    {
        if (session.IsLoggedIn) return true;
        await page.DisplayAlert("Prijava", "Morate biti prijavljeni.", "U redu");
        return false;
    }
}
