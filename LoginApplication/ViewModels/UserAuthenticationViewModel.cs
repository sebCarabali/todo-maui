using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.ViewModels
{
    [ObservableObject]
    public partial class UserAuthenticationViewModel
    {
        private readonly ICameraService _cameraService;
        private readonly IFacialAuthenticationService _facialAuthenticationService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy = false;

        [ObservableProperty]
        private string _identifier;

        [ObservableProperty]
        private ImageSource _capturedPhoto;

        [ObservableProperty]
        private byte[] _photoBytes;

        public bool IsNotBusy => !IsBusy;

        public UserAuthenticationViewModel(ICameraService cameraService, IFacialAuthenticationService facialAuthenticationService)
        {
            _cameraService = cameraService;
            _facialAuthenticationService = facialAuthenticationService;
        }



        [RelayCommand]
        public async Task TakePhoto()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                CapturedPhoto = null;
                PhotoBytes = null;

                var photoStream = await _cameraService.CapturePhotoAsync();
                if (photoStream == null) return;

                using (var memoryStream = new MemoryStream())
                {
                    await photoStream.CopyToAsync(memoryStream);
                    PhotoBytes = memoryStream.ToArray();
                }
                photoStream.Position = 0;

                CapturedPhoto = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(PhotoBytes);
                });

                photoStream.Dispose();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to capture photo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Authenticate()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(Identifier))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your identification or email", "OK");
                return;
            }
            if (PhotoBytes == null || PhotoBytes.Length == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please take a photo first", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                await DoAuthenticaction(Identifier, new MemoryStream(PhotoBytes));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Authentication error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
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
