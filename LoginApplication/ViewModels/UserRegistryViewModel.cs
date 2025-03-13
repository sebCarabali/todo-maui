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
    public partial class UserRegistryViewModel
    {
        private readonly ICameraService _cameraService;

        [ObservableProperty]
        public Stream _photo;

        [ObservableProperty]
        private string _identificationNumber;

        public UserRegistryViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [RelayCommand]
        public async Task TakePhoto()
        {
            _photo = await _cameraService.CapturePhotoAsync();
        }

        public async Task SendPhoto()
        {
            if (_photo == null || string.IsNullOrEmpty(_identificationNumber))
            {
                throw new InvalidOperationException("Photo or identification number is missing.");
            }

            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(_photo), "photo", "photo.jpg");
                content.Add(new StringContent(_identificationNumber), "identificationNumber");

                var response = await httpClient.PostAsync("https://api.example.com/upload", content);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
