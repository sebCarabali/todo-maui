using LoginApplication.ViewModels;

namespace LoginApplication.Views;

public partial class UserRegistrationPage : ContentPage
{
	public UserRegistrationPage(UserRegistrationViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}