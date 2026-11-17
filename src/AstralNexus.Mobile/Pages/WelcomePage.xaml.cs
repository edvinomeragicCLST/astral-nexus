namespace AstralNexus.Mobile.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly IServiceProvider _services;
    public WelcomePage(IServiceProvider services)
    {
        InitializeComponent(); _services = services;
        InfoCarousel.ItemsSource = new[]
        {
            new IntroSlide("Kolekcija", "Pregledaj i filtriraj originalne Astral Nexus karte koje posjeduješ."),
            new IntroSlide("Kreator špilova", "Kreiraj, uređuj i trajno sačuvaj špilove uz automatsku provjeru pravila."),
            new IntroSlide("Paketi i mečevi", "Otvori paket, proširi kolekciju i testiraj špil u simulatoru meča.")
        };
    }
    private async void Login_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(_services.Resolve<LoginPage>());
    private async void Register_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(_services.Resolve<RegisterPage>());
    private async void About_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(_services.Resolve<AboutPage>());
    private sealed record IntroSlide(string Title, string Text);
}
