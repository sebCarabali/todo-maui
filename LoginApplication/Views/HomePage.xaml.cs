using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Messages;

namespace LoginApplication.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

        WeakReferenceMessenger.Default.Send(new LoginMessage(true, "Login successful"));
    }
}