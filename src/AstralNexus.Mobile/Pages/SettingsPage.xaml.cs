using AstralNexus.Mobile.Services;
namespace AstralNexus.Mobile.Pages;
public partial class SettingsPage:ContentPage
{
 private readonly CatalogSyncService _sync;public SettingsPage(CatalogSyncService sync){InitializeComponent();_sync=sync;var t=Preferences.Default.Get("theme","System");ThemePicker.SelectedIndex=t=="Light"?1:t=="Dark"?2:0;ApiEntry.Text=Preferences.Default.Get("api_url","http://10.0.2.2:5092");}
 private void ApplyTheme_Clicked(object s,EventArgs e){var value=ThemePicker.SelectedIndex switch{1=>"Light",2=>"Dark",_=>"System"};Preferences.Default.Set("theme",value);App.ApplySavedTheme();}
 private async void Sync_Clicked(object s,EventArgs e){try{var url=(ApiEntry.Text??"").Trim();Preferences.Default.Set("api_url",url);SyncStatus.Text="Sinhronizacija...";var count=await _sync.SyncAsync(url);SyncStatus.Text=$"Ažurirano {count} lokalnih karata putem REST API-ja.";}catch(Exception ex){SyncStatus.Text=$"REST nije dostupan: {ex.Message}";}}
}
