using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Dtos;
using LoginApplication.Messages;
using LoginApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userName;
        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool _isBussy = false;

        public bool IsNotBussy => !IsBussy;

        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        partial void OnIsBussyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBussy));
        }

        [RelayCommand]
        private async void Login()
        {
            if (IsBussy) return;

            IsBussy = true;

            // create login request object
            var loginRequest = new LoginRequestDTO
            {
                IdTipoIdentificacion = 0, // What values should be here?
                Identificacion = UserName,
                Contrasenia = Password
            };

            bool success = await _authService.LoginAsync(loginRequest);

            IsBussy = false;

            if (!success) WeakReferenceMessenger.Default.Send(new LoginMessage(false, "Login failed"));
            else WeakReferenceMessenger.Default.Send(new LoginMessage(true, "Login successful"));
        }
    }
}
