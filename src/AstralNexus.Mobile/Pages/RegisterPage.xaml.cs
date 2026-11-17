using AstralNexus.Mobile.Data;
using AstralNexus.Mobile.Services;

namespace AstralNexus.Mobile.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AppDatabase _db; private readonly SessionService _session; private readonly IServiceProvider _services;
    public RegisterPage(AppDatabase db, SessionService session, IServiceProvider services) { InitializeComponent(); _db = db; _session = session; _services = services; }
    private async void Register_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        if ((PasswordEntry.Text ?? "") != (ConfirmEntry.Text ?? "")) { ErrorLabel.Text = "Lozinke se ne podudaraju."; ErrorLabel.IsVisible = true; return; }
        var result = await _db.RegisterAsync(NameEntry.Text ?? "", EmailEntry.Text ?? "", PasswordEntry.Text ?? "");
        if (!result.Success || result.User is null) { ErrorLabel.Text = result.Error; ErrorLabel.IsVisible = true; return; }
        _session.SignIn(result.User);
        Application.Current!.Windows[0].Page = new NavigationPage(_services.Resolve<DashboardPage>()) { BarBackgroundColor = Color.FromArgb("#4C3598"), BarTextColor = Colors.White };
    }
}
