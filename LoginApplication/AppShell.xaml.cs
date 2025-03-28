using LoginApplication.Views;

namespace LoginApplication
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("user-auth", typeof(UserAuthenticationPage));
            Routing.RegisterRoute("user-singup", typeof(UserRegistrationPage));
            Routing.RegisterRoute("login", typeof(LoginPage));
        }
    }
}
