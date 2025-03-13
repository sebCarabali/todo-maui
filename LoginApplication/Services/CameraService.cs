using LoginApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class CameraService : ICameraService
    {
        public async Task<Stream> CapturePhotoAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                    throw new NotSupportedException("Camera capture is not supported on this device.");

                var status = await RequestCameraPermissionsAsync();
                if (status != PermissionStatus.Granted)
                    throw new UnauthorizedAccessException("Camera permission was not granted.");

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null)
                    throw new InvalidOperationException("Photo capture was canceled or failed.");

                return await photo.OpenReadAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to capture photo.", ex);
            }
        }

        private async Task<PermissionStatus> RequestCameraPermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            return status;
        }
    }
}
