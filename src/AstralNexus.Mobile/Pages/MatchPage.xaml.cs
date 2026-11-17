using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Models;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;
public partial class MatchPage:ContentPage
{
 private readonly AppDatabase _db;private readonly SessionService _session;private readonly DeckRulesService _rules;private List<CardEntity> _draw=[];private readonly List<CardEntity> _hand=[];private int _playerHp=10,_opponentHp=10,_turn=1,_turns;private int _deckId;
 public MatchPage(AppDatabase db,SessionService session,DeckRulesService rules){InitializeComponent();_db=db;_session=session;_rules=rules;}
 protected override async void OnAppearing(){base.OnAppearing();if(!_session.IsLoggedIn)return;DeckPicker.ItemsSource=(await _db.GetDecksAsync(_session.CurrentUser!.Id)).Where(x=>x.IsValid).ToList();}
 private async void Start_Clicked(object s,EventArgs e){if(DeckPicker.SelectedItem is not DeckSummaryItem deck){await DisplayAlert("Meč","Odaberite validan špil.","U redu");return;}var cards=await _db.GetDeckCardsAsync(deck.Id);var valid=_rules.Validate(cards);if(!valid.IsValid){await DisplayAlert("Špil",string.Join("\n",valid.Errors),"U redu");return;}_deckId=deck.Id;_draw=cards.OrderBy(_=>Random.Shared.Next()).ToList();_hand.Clear();_playerHp=_opponentHp=10;_turn=1;_turns=0;for(var i=0;i<6&&_draw.Count>0;i++)DrawOne();GamePanel.IsVisible=true;StartButton.IsEnabled=false;StatusLabel.Text="Meč je pokrenut. Početna ruka: 6 karata.";Refresh();}
 private void DrawOne(){var c=_draw[^1];_draw.RemoveAt(_draw.Count-1);_hand.Add(c);}
 private void Refresh(){PlayerHpLabel.Text=_playerHp.ToString();OpponentHpLabel.Text=_opponentHp.ToString();HandView.ItemsSource=_hand.ToList();}
 private void Draw_Clicked(object s,EventArgs e){if(_hand.Count>=6){StatusLabel.Text="Ruka je puna (max 6).";return;}if(_draw.Count==0){StatusLabel.Text="Nema više karata u špilu.";return;}DrawOne();StatusLabel.Text="Izvučena je karta.";Refresh();}
 private void Discard_Clicked(object s,EventArgs e){if(s is Button b&&int.TryParse(b.CommandParameter?.ToString(),out var id)){var c=_hand.FirstOrDefault(x=>x.Id==id);if(c is not null){_hand.Remove(c);StatusLabel.Text=$"Odbačena: {c.Name}.";Refresh();}}}
 private void PlayerMinus_Clicked(object s,EventArgs e){_playerHp--;Refresh();} private void PlayerPlus_Clicked(object s,EventArgs e){_playerHp++;Refresh();} private void OpponentMinus_Clicked(object s,EventArgs e){_opponentHp--;Refresh();} private void OpponentPlus_Clicked(object s,EventArgs e){_opponentHp++;Refresh();}
 private void Dice_Clicked(object s,EventArgs e)=>StatusLabel.Text=$"Kocka: {Random.Shared.Next(1,7)}"; private void Coin_Clicked(object s,EventArgs e)=>StatusLabel.Text=$"Novčić: {(Random.Shared.Next(2)==0?"Glava":"Pismo")}";
 private void EndTurn_Clicked(object s,EventArgs e){_turn=_turn==1?2:1;_turns++;StatusLabel.Text=$"Potez predan. Na redu je igrač {_turn}.";}
 private async void Win_Clicked(object s,EventArgs e)=>await FinishAsync("Win"); private async void Loss_Clicked(object s,EventArgs e)=>await FinishAsync("Loss");
 private async Task FinishAsync(string result){var reward=await _db.SaveMatchAsync(_session.CurrentUser!.Id,_deckId,result,Math.Max(1,_turns));await DisplayAlert("Meč",$"Rezultat je sačuvan u SQLite bazi. Nagrada: {reward} coina.","U redu");await Navigation.PopAsync();}
}
