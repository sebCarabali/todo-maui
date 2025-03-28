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
    [ObservableObject]
    public partial class UserRegistrationViewModel
    {
        private readonly ICameraService _cameraService;
        private readonly IClienteService _clienteService;

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

        public UserRegistrationViewModel(ICameraService cameraService, IClienteService clienteService)
        {
            _cameraService = cameraService;
            _clienteService = clienteService;
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
        public async Task SingUp()
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

                await DoSingUpAsync();
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

        private async Task DoSingUpAsync()
        {
            try
            {
                var success = await _clienteService.SetEncodingAsync(Identifier, PhotoBytes);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "User registered successfully", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to register user", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to register user: {ex.Message}", "OK");
            }
        }
    }
}
