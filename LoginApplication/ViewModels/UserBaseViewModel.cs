using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoginApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    public abstract partial class UserBaseViewModel : ObservableObject
    {
        protected readonly ICameraService _cameraService;

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

        protected UserBaseViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;
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

                CapturedPhoto = ImageSource.FromStream(() => new MemoryStream(PhotoBytes));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Photo capture failed: {ex}");
                await Shell.Current.DisplayAlert("Error", $"Failed to capture photo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task HandleSubmitAsync(Func<Task> action)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await action();
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ShowAlertAsync(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }

        protected virtual void ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Identifier))
                throw new InvalidOperationException("Por favor ingrese su identificación o correo electrónico");

            if (PhotoBytes == null || PhotoBytes.Length == 0)
                throw new InvalidOperationException("Por favor tome una fotografía primero");
        }
    }
}
