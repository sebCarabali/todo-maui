namespace LoginApplication.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
	}

	public async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/user-singup");
    }

	public async void OnAuthenticateButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/user-auth");
    }
}