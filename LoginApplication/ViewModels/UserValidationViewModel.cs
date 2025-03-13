using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.ViewModels
{
    [ObservableObject]
    public partial class UserValidationViewModel
    {
        private readonly ICameraService _cameraService;
        private readonly IFacialAuthenticationService _facialAuthenticationService;

        [ObservableProperty]
        private Stream? _photo;

        [ObservableProperty]
        private string? _identificacion;

        [ObservableProperty]
        private ImageSource _capturedImage;

        public UserValidationViewModel(ICameraService cameraService, IFacialAuthenticationService facialAuthenticationService)
        {
            _cameraService = cameraService;
            _facialAuthenticationService = facialAuthenticationService;
        }



        [RelayCommand]
        public async Task TakePhoto()
        {
            Stream photoStream = null;
            try
            {
                photoStream = await _cameraService.CapturePhotoAsync();

                if (photoStream != null)
                {
                    CapturedImage = ImageSource.FromStream(() => photoStream);

                    await DoAuthenticaction(Identificacion, photoStream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error : {ex.Message}", "OK");
            }
            finally
            {
                photoStream?.Close();
            }
        }

        private async Task DoAuthenticaction(string identificacion, Stream photo)
        {
            try
            {
                bool authenticated = await _facialAuthenticationService.AuthenticateAsync(identificacion, photo);
                if (authenticated)
                {
                    await Shell.Current.DisplayAlert("Success", "Usuario autenticado correctamente", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "OK");
            }
        }
    }
}
