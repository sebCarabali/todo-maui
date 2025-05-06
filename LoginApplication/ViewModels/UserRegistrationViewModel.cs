using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.Dtos;
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

        [ObservableProperty]
        private string _Identificacion;

        [ObservableProperty]
        private string _Correo;

        [ObservableProperty]
        private string _Celular;

        [ObservableProperty]
        private string _PrimerNombre;

        [ObservableProperty]
        private string _SegundoNombre;

        [ObservableProperty]
        private string _PrimerApellido;

        [ObservableProperty]
        private string _SegundoApellido;

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

            var cliente = new Cliente
            {
                identificacion = Identificacion,
                primer_nombre = PrimerNombre,
                segundo_nombre = SegundoNombre,
                primer_apellido = PrimerApellido,
                segundo_apellido = SegundoApellido,
                correo = Correo,
                celular = Celular
            };

            var result = await _clienteService.AgregarCliente(cliente, new MemoryStream(PhotoBytes)!);

            if (result != null)
            {
                await ShowAlertAsync($"¡Registro exitoso!, {PrimerNombre}",
                    "Perfil biométrico creado correctamente.\n\n" +
                    "Datos registrados:\n" +
                    $"• Identificador: {Identificacion}\n" +
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
