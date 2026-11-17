using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;
public partial class DecksPage:ContentPage
{
    private readonly AppDatabase _db;private readonly SessionService _session;private readonly IServiceProvider _services;
    public DecksPage(AppDatabase db,SessionService session,IServiceProvider services){InitializeComponent();_db=db;_session=session;_services=services;}
    protected override async void OnAppearing(){base.OnAppearing();if(_session.IsLoggedIn)DecksView.ItemsSource=await _db.GetDecksAsync(_session.CurrentUser!.Id);}
    private async void New_Clicked(object s,EventArgs e){var p=_services.Resolve<DeckEditorPage>();p.Configure(null);await Navigation.PushAsync(p);}
    private async void Edit_Clicked(object s,EventArgs e){if(s is Button b&&int.TryParse(b.CommandParameter?.ToString(),out var id)){var p=_services.Resolve<DeckEditorPage>();p.Configure(id);await Navigation.PushAsync(p);}}
    private async void Delete_Clicked(object s,EventArgs e){if(s is not Button b||!int.TryParse(b.CommandParameter?.ToString(),out var id))return;if(!await DisplayAlert("Brisanje","Obrisati ovaj špil?","Da","Ne"))return;await _db.DeleteDeckAsync(_session.CurrentUser!.Id,id);DecksView.ItemsSource=await _db.GetDecksAsync(_session.CurrentUser!.Id);}
}
