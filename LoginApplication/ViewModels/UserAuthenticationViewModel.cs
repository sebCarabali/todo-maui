using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.Dtos.Acceso;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.ViewModels
{
    public partial class UserAuthenticationViewModel : UserBaseViewModel
    {
        private readonly IFacialAuthenticationService _facialAuthenticationService;
        private readonly IRegistroAccesoService _registroAccesoService;

        public UserAuthenticationViewModel(
            ICameraService cameraService,
            IFacialAuthenticationService facialAuthenticationService,
            IRegistroAccesoService registroAccesoService)
            : base(cameraService)
        {
            _facialAuthenticationService = facialAuthenticationService;
            _registroAccesoService = registroAccesoService;
        }

        [RelayCommand]
        private async Task Authenticate() =>
            await HandleSubmitAsync(PerformAuthentication);

        private async Task PerformAuthentication()
        {
            ValidateInputs();

            using var photoStream = new MemoryStream(PhotoBytes!);
            var authResult = await _facialAuthenticationService.AuthenticateAsync(photoStream);

            if (authResult.IsAuthenticated)
            {
                await ShowAlertAsync($"¡Bienvenido!, {authResult.Cliente.primer_nombre}",
                    "Autenticación biométrica exitosa.\n\n" +
                    "Acceso concedido.");

                await SaveAccesLog(true);
            }
            else
            {
                await ShowAlertAsync("Acceso no autorizado",
                    "No coinciden los datos biométricos.\n\n" +
                    "Posibles causas:\n" +
                    "1. La foto no es clara\n" +
                    "2. No está registrado en el sistema\n" +
                    "3. Cambios significativos en su rostro\n\n" +
                    "¿Necesita ayuda? Contacte al soporte técnico.");
                await SaveAccesLog(false);
            }
        }

        private async Task SaveAccesLog(bool success)
        {
            AccesoRequestDTO accessLog;
            if (Identifier.Contains("@"))
            {
                accessLog = new AccesoRequestDTO
                {
                    Correo = Identifier,
                    Resultado = success ? 'P' : 'D'
                };
            }
            else
            {
                accessLog = new AccesoRequestDTO
                {
                    Identificacion = Identifier,
                    Resultado = success ? 'P' : 'D'
                };
            }

            await _registroAccesoService.RegistrarAccesoAsync(accessLog);
        }
    }
}