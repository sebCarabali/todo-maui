using LoginApplication.ViewModels;

namespace LoginApplication.Views;

public partial class UserAuthenticationPage : ContentPage
{
	public UserAuthenticationPage(UserAuthenticationViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}