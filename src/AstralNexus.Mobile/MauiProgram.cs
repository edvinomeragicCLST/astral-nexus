using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Pages;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<DeckRulesService>();
        builder.Services.AddSingleton<CatalogSyncService>();

        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<CollectionPage>();
        builder.Services.AddTransient<DecksPage>();
        builder.Services.AddTransient<DeckEditorPage>();
        builder.Services.AddTransient<ShopPage>();
        builder.Services.AddTransient<MatchPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<AboutPage>();
        return builder.Build();
    }
}
