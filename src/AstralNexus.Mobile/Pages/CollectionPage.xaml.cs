using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Models;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;
public partial class CollectionPage : ContentPage
{
    private readonly AppDatabase _db; private readonly SessionService _session; private List<OwnedCardItem> _all=[];
    public CollectionPage(AppDatabase db, SessionService session){InitializeComponent();_db=db;_session=session;}
    protected override async void OnAppearing(){base.OnAppearing();if(!await PageHelpers.EnsureLoggedInAsync(this,_session))return;_all=await _db.GetCollectionAsync(_session.CurrentUser!.Id);var types=_all.Select(x=>x.Type).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x=>x).Prepend("Svi tipovi").ToList();TypePicker.ItemsSource=types;TypePicker.SelectedIndex=0;ApplyFilter();}
    private void Filter_Changed(object? sender, EventArgs e)=>ApplyFilter();
    private void ApplyFilter(){var q=(Search.Text??"").Trim();var type=TypePicker.SelectedItem as string;IEnumerable<OwnedCardItem> data=_all;if(!string.IsNullOrWhiteSpace(q))data=data.Where(x=>x.Name.Contains(q,StringComparison.OrdinalIgnoreCase)||x.Type.Contains(q,StringComparison.OrdinalIgnoreCase));if(!string.IsNullOrWhiteSpace(type)&&type!="Svi tipovi")data=data.Where(x=>x.Type.Equals(type,StringComparison.OrdinalIgnoreCase));CardsView.ItemsSource=data.ToList();}
}
