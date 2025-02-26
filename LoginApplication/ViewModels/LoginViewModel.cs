using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    [ObservableObject]
    public partial class LoginViewModel
    {
        [ObservableProperty]
        private string userName;
        [ObservableProperty]
        private string password;

        [RelayCommand]        
        private void Login()
        {
            // TODO: Call the authentication service and save the jwt(JSON Web Token) token 
            Console.WriteLine($"Try to login with {userName} and {password}");
        }
    }
}
