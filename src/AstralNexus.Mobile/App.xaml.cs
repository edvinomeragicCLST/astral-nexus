using AstralNexus.Mobile.Pages;

namespace AstralNexus.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        ApplySavedTheme();

        var welcomePage = _services.Resolve<WelcomePage>();

        return new Window(new NavigationPage(welcomePage)
        {
            BarBackgroundColor = Color.FromArgb("#4C3598"),
            BarTextColor = Colors.White
        });
    }

    public static void ApplySavedTheme()
    {
        var value = Preferences.Default.Get("theme", "System");

        if (Current is null)
            return;

        Current.UserAppTheme = value switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
