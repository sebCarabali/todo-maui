using System;
using System.IO;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    /// <summary>
    /// Servicio para la captura de fotos utilizando la cámara del dispositivo.
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Captura una foto usando la cámara del dispositivo.
        /// </summary>
        /// <returns>Stream con la imagen capturada.</returns>
        /// <exception cref="AppException">
        /// Se lanza cuando ocurre un error durante la captura de la foto.
        /// Los códigos de error comunes incluyen:
        /// - "Cámara no soportada": El dispositivo no soporta la captura de fotos.
        /// - "Permiso denegado": El usuario no otorgó los permisos necesarios.
        /// - "Captura cancelada": El usuario canceló la captura o esta falló.
        /// - "Error en la foto": No se pudo obtener la imagen capturada.
        /// - "Error de almacenamiento": Error al acceder al almacenamiento del dispositivo.
        /// - "Error inesperado": Error no manejado durante la captura.
        /// </exception>
        Task<Stream> CapturePhotoAsync();
    }
}
