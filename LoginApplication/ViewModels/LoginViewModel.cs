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

            try
            {
                var request = new LoginRequestDTO { Identificacion = UserName, Contrasenia = Password };
                bool isSuccess = await _authService.LoginAsync(request);

                if (isSuccess)
                {
                    WeakReferenceMessenger.Default.Send(new LoginMessage(true, "Login successful"));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, ex.Message));
            }
            catch (HttpRequestException ex)
            {
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, ex.Message));
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, ex.Message));
            }

            IsBussy = false;
        }
    }
}
