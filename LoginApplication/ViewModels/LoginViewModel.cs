using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Dtos;
using LoginApplication.Exceptions;
using LoginApplication.Messages;
using LoginApplication.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _isBussy = false;

        public bool IsNotBussy => !IsBussy;

        private readonly IAuthService _authService;
        private readonly ISecureStorage _secureStorage;
        private readonly ILogger<LoginViewModel> _logger;
        
        public LoginViewModel(
            IAuthService authService, 
            ISecureStorage secureStorage,
            ILogger<LoginViewModel> logger)
        {
            _authService = authService;
            _secureStorage = secureStorage;
            _logger = logger;
            CheckLoginStateAsync();
        }

        public async void CheckLoginStateAsync()
        {
            try
            {
                var token = await _secureStorage.GetAsync("jwtToken");
                if (!string.IsNullOrEmpty(token))
                {
                    await Shell.Current.GoToAsync("dashboard");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking token: {ex.Message}");
            }
        }

        partial void OnIsBussyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBussy));
        }

        [RelayCommand]
        private async void Login()
        {
            if (IsBussy) 
            {
                _logger.LogInformation("Intento de inicio de sesión mientras ya hay una operación en curso");
                return;
            }

            IsBussy = true;
            _logger.LogInformation("Iniciando proceso de login para el email: {Email}", Email);

            try
            {
                var request = new LoginRequestDTO { Correo = Email, Contrasenia = Password };
                bool isSuccess = await _authService.LoginAsync(request);

                if (isSuccess)
                {
                    _logger.LogInformation("Login exitoso, navegando al dashboard");
                    await Shell.Current.GoToAsync("dashboard");
                }
            }
            catch (AppUnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Error de autenticación");
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, ex.Message));
            }
            catch (AppValidationException ex)
            {
                _logger.LogWarning("Error de validación: {Errors}", string.Join(", ", 
                    ex.Errors.SelectMany(e => e.Value)));
                
                // Tomar el primer error para mostrar al usuario
                var firstError = ex.Errors.FirstOrDefault();
                var errorMessage = firstError.Value?.FirstOrDefault() ?? "Error de validación";
                
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, errorMessage));
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.RequestTimeout)
            {
                _logger.LogError(ex, "Tiempo de espera agotado al intentar conectar con el servidor");
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, 
                    "Tiempo de espera agotado. Por favor, verifica tu conexión a internet e inténtalo de nuevo."));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión");
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, 
                    "No se pudo conectar con el servidor. Verifica tu conexión a internet."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante el inicio de sesión");
                WeakReferenceMessenger.Default.Send(new LoginMessage(false, 
                    "Ocurrió un error inesperado. Por favor, inténtalo de nuevo más tarde."));
            }
            finally
            {
                IsBussy = false;
            }
        }
    }
}
