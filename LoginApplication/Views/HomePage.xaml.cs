using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Messages;

namespace LoginApplication.Views;

public partial class HomePage : ContentPage
{
    private readonly ISecureStorage _secureStorage;
    public HomePage(ISecureStorage secureStorage)
    {
        _secureStorage = secureStorage;
        InitializeComponent();
        CheckAuthenticated();
        WeakReferenceMessenger.Default.Send(new LoginMessage(true, "Login successful"));
    }

    private async void CheckAuthenticated()
    {
        string jwt = await _secureStorage.GetAsync("JwtToken") ?? string.Empty;
        if (string.IsNullOrEmpty(jwt))
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }

    public async Task OnLogout(object sender, EventArgs e)
    {
        _secureStorage.Remove("JwtToken");
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
