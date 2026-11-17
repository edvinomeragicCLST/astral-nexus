using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AppDatabase _db; private readonly SessionService _session; private readonly IServiceProvider _services;
    public LoginPage(AppDatabase db, SessionService session, IServiceProvider services) { InitializeComponent(); _db = db; _session = session; _services = services; }
    private async void Demo_Clicked(object sender, EventArgs e) { EmailEntry.Text = "demo@astralnexus.local"; PasswordEntry.Text = "Demo123!"; await LoginAsync(); }
    private async void Login_Clicked(object sender, EventArgs e) => await LoginAsync();
    private async Task LoginAsync()
    {
        ErrorLabel.IsVisible = false;
        var user = await _db.LoginAsync(EmailEntry.Text ?? "", PasswordEntry.Text ?? "");
        if (user is null) { ErrorLabel.Text = "Pogrešan email ili lozinka."; ErrorLabel.IsVisible = true; return; }
        _session.SignIn(user);
        Application.Current!.Windows[0].Page = new NavigationPage(_services.Resolve<DashboardPage>()) { BarBackgroundColor = Color.FromArgb("#4C3598"), BarTextColor = Colors.White };
    }
}
