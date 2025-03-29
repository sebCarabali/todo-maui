using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.ViewModels
{
    public partial class UserAuthenticationViewModel : UserBaseViewModel
    {
        private readonly IFacialAuthenticationService _facialAuthenticationService;

        public UserAuthenticationViewModel(
            ICameraService cameraService,
            IFacialAuthenticationService facialAuthenticationService)
            : base(cameraService)
        {
            _facialAuthenticationService = facialAuthenticationService;
        }

        [RelayCommand]
        private async Task Authenticate() =>
            await HandleSubmitAsync(PerformAuthentication);

        private async Task PerformAuthentication()
        {
            ValidateInputs();

            using var photoStream = new MemoryStream(PhotoBytes!);
            bool authenticated = await _facialAuthenticationService.AuthenticateAsync(Identifier, photoStream);

            if (authenticated)
            {
                await ShowAlertAsync("¡Bienvenido!",
                    "Autenticación biométrica exitosa.\n\n" +
                    "Acceso concedido.");
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
            }
        }
    }
}
