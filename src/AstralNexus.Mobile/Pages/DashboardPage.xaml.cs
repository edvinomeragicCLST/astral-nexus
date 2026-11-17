using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly AppDatabase _db; private readonly SessionService _session; private readonly IServiceProvider _services;
    public DashboardPage(AppDatabase db, SessionService session, IServiceProvider services) { InitializeComponent(); _db = db; _session = session; _services = services; }
    protected override async void OnAppearing() { base.OnAppearing(); if (!_session.IsLoggedIn) return; var u = await _db.RefreshUserAsync(_session.CurrentUser!.Id); _session.SignIn(u); var s = await _db.GetStatsAsync(u.Id); WelcomeLabel.Text = $"Dobrodošao, {u.DisplayName}"; CoinsLabel.Text=s.Coins.ToString(); CardsLabel.Text=s.OwnedCards.ToString(); DecksLabel.Text=s.Decks.ToString(); WinLossLabel.Text=s.WinLoss; }
    private async void Collection_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<CollectionPage>());
    private async void Decks_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<DecksPage>());
    private async void Shop_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<ShopPage>());
    private async void Match_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<MatchPage>());
    private async void History_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<HistoryPage>());
    private async void Profile_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<ProfilePage>());
    private async void Settings_Clicked(object s, EventArgs e)=>await Navigation.PushAsync(_services.Resolve<SettingsPage>());
    private void Logout_Clicked(object s, EventArgs e) { _session.SignOut(); Application.Current!.Windows[0].Page = new NavigationPage(_services.Resolve<WelcomePage>()) { BarBackgroundColor=Color.FromArgb("#4C3598"), BarTextColor=Colors.White}; }
}
