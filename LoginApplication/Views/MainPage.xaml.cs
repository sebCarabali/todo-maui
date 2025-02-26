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
        }
    }

}
