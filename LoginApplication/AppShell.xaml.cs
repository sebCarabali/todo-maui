using LoginApplication.Views;

namespace LoginApplication
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("home", typeof(HomePage));
            Routing.RegisterRoute("login", typeof(LoginPage));
        }
    }
}
