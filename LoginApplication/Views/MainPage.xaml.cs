using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Messages;
using LoginApplication.ViewModels;

namespace LoginApplication.Views
{
    public partial class MainPage : ContentPage
    {
        private LoginViewModel vm = new LoginViewModel();

        public MainPage()
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
