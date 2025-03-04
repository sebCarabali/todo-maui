using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Messages;
using LoginApplication.ViewModels;

namespace LoginApplication.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            WeakReferenceMessenger.Default.Register<LoginMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(m.IsSuccess ? "Success" : "Error", m.Message, "OK");
                });
            });
        }
    }

}
