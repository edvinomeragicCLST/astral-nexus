using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;
public partial class ShopPage:ContentPage
{
 private readonly AppDatabase _db;private readonly SessionService _session;public ShopPage(AppDatabase db,SessionService session){InitializeComponent();_db=db;_session=session;}
 protected override async void OnAppearing(){base.OnAppearing();await RefreshCoins();}
 private async Task RefreshCoins(){if(!_session.IsLoggedIn)return;var u=await _db.RefreshUserAsync(_session.CurrentUser!.Id);_session.SignIn(u);CoinsLabel.Text=$"Stanje: {u.Coins} coina";}
 private async void Open_Clicked(object s,EventArgs e){try{var r=await _db.OpenPackAsync(_session.CurrentUser!.Id);ResultLabel.Text=$"Dobijeno 6 karata • duplikati +{r.DuplicateCoins} coina";ResultLabel.IsVisible=true;ResultView.ItemsSource=r.Cards;await RefreshCoins();}catch(Exception ex){await DisplayAlert("Paket",ex.Message,"U redu");}}
}
