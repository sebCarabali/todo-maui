using LoginApplication.Exceptions;
using LoginApplication.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class CameraService : ICameraService
    {
        private readonly ILogger<CameraService> _logger;

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
        }
        public async Task<Stream> CapturePhotoAsync()
        {
            _logger.LogInformation("Iniciando captura de foto");
            
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    _logger.LogWarning("La captura de cámara no es soportada en este dispositivo");
                    throw new AppException("Cámara no soportada", 
                        "La captura de fotos no es compatible con este dispositivo.");
                }

                var status = await RequestCameraPermissionsAsync();
                if (status != PermissionStatus.Granted)
                {
                    _logger.LogWarning("Permiso de cámara denegado por el usuario");
                    throw new AppException("Permiso denegado", 
                        "Se requiere permiso de cámara para capturar fotos. Por favor, otorga los permisos necesarios en la configuración de la aplicación.");
                }

                _logger.LogDebug("Solicitando captura de foto");
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                
                if (photo == null)
                {
                    _logger.LogWarning("El usuario canceló la captura de foto o falló");
                    throw new AppException("Captura cancelada", 
                        "La captura de foto fue cancelada o falló. Por favor, inténtalo de nuevo.");
                }

                _logger.LogInformation("Foto capturada exitosamente, abriendo stream");
                
                try
                {
                    var stream = await photo.OpenReadAsync();
                    if (stream == null || stream.Length == 0)
                    {
                        _logger.LogError("El stream de la foto está vacío o es nulo");
                        throw new AppException("Error en la foto", 
                            "No se pudo obtener la imagen capturada. Por favor, intenta de nuevo.");
                    }
                    
                    return stream;
                }
                catch (FileNotFoundException ex)
                {
                    _logger.LogError(ex, "No se pudo encontrar el archivo de la foto capturada");
                    throw new AppException(
                        $"Error al procesar la foto: No se pudo acceder a la foto capturada. Por favor, intenta de nuevo.", 
                        ex);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error de E/S al leer la foto capturada");
                    throw new AppException(
                        $"Error de almacenamiento: Ocurrió un error al procesar la imagen. Verifica el espacio de almacenamiento disponible.", 
                        ex);
                }
            }
            catch (AppException)
            {
                // Re-lanzar excepciones de aplicación sin modificar
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al capturar foto");
                throw new AppException(
                    $"Error inesperado: Ocurrió un error inesperado al capturar la foto. Por favor, inténtalo de nuevo.", 
                    ex);
            }
        }

        private async Task<PermissionStatus> RequestCameraPermissionsAsync()
        {
            try
            {
                _logger.LogDebug("Verificando estado del permiso de cámara");
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                
                if (status != PermissionStatus.Granted)
                {
                    _logger.LogInformation("Solicitando permiso de cámara al usuario");
                    status = await MainThread.InvokeOnMainThreadAsync(async () => 
                        await Permissions.RequestAsync<Permissions.Camera>());
                    
                    _logger.LogInformation("Estado del permiso de cámara: {Status}", status);
                }
                
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar o solicitar permisos de cámara");
                return PermissionStatus.Denied;
            }
        }
    }
}
