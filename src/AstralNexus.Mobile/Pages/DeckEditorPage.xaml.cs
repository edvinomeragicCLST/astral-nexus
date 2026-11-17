using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Models;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;
public partial class DeckEditorPage:ContentPage
{
    private readonly AppDatabase _db;private readonly SessionService _session;private readonly DeckRulesService _rules;private int? _deckId;private List<OwnedCardItem> _owned=[];private readonly List<CardEntity> _selected=[];
    public DeckEditorPage(AppDatabase db,SessionService session,DeckRulesService rules){InitializeComponent();_db=db;_session=session;_rules=rules;}
    public void Configure(int? deckId)=>_deckId=deckId;
    protected override async void OnAppearing(){base.OnAppearing();if(_owned.Count>0)return;_owned=await _db.GetCollectionAsync(_session.CurrentUser!.Id);if(_deckId.HasValue){var deck=await _db.GetDeckAsync(_deckId.Value);if(deck is not null)NameEntry.Text=deck.Name;_selected.AddRange(await _db.GetDeckCardsAsync(_deckId.Value));}Refresh();}
    private void Refresh(){AvailableView.ItemsSource=_owned.Where(x=>_selected.All(s=>s.Id!=x.Id)).ToList();SelectedView.ItemsSource=_selected.ToList();CountLabel.Text=$"Odabrano: {_selected.Count}/20";}
    private async void Add_Clicked(object s,EventArgs e){if(s is not Button b||!int.TryParse(b.CommandParameter?.ToString(),out var id))return;var item=_owned.FirstOrDefault(x=>x.Id==id);if(item is null)return;if(!_rules.CanAdd(item.Card,_selected,out var reason)){await DisplayAlert("Pravilo špila",reason,"U redu");return;}_selected.Add(item.Card);Refresh();}
    private void Remove_Clicked(object s,EventArgs e){if(s is Button b&&int.TryParse(b.CommandParameter?.ToString(),out var id)){var c=_selected.FirstOrDefault(x=>x.Id==id);if(c is not null)_selected.Remove(c);Refresh();}}
    private async void Save_Clicked(object s,EventArgs e){var result=await _db.SaveDeckAsync(_session.CurrentUser!.Id,_deckId,NameEntry.Text??"",_selected,_rules);if(!result.Success){await DisplayAlert("Špil nije sačuvan",result.Error,"U redu");return;}await DisplayAlert("Uspjeh","Špil je sačuvan u SQLite bazi.","U redu");await Navigation.PopAsync();}
}
