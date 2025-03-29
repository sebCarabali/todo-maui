using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    public partial class UserRegistrationViewModel : UserBaseViewModel
    {
        
        private readonly IClienteService _clienteService;

        public UserRegistrationViewModel(ICameraService cameraService, IClienteService clienteService) 
            : base(cameraService)
        {
            _clienteService = clienteService;
        }

        [RelayCommand]
        public async Task SignUp() =>
            await HandleSubmitAsync(PerformSignUp);

        private async Task PerformSignUp()
        {
            ValidateInputs();

            var success = await _clienteService.SetEncodingAsync(Identifier, PhotoBytes!);

            if (success)
            {
                await ShowAlertAsync("¡Registro exitoso!",
                    "Perfil biométrico creado correctamente.\n\n" +
                    "Datos registrados:\n" +
                    $"• Identificador: {Identifier}\n" +
                    $"• Fecha: {DateTime.Now:dd/MM/yyyy}\n\n" +
                    "Ya puede autenticarse usando reconocimiento facial.");
            }
            else
            {
                await ShowAlertAsync("Registro incompleto",
                    "El servidor no pudo procesar su registro.\n\n" +
                    "Por favor:\n" +
                    "1. Verifique su conexión a internet\n" +
                    "2. Intente con otra fotografía\n" +
                    "3. Contacte a soporte si persiste el error");
            }
        }
    }
}
