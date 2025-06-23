using LoginApplication.Dtos;
using LoginApplication.Exceptions;
using System;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    /// <summary>
    /// Servicio para manejar la autenticación de usuarios.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica a un usuario con sus credenciales.
        /// </summary>
        /// <param name="request">Datos de inicio de sesión del usuario.</param>
        /// <returns>True si la autenticación fue exitosa, de lo contrario lanza una excepción.</returns>
        /// <exception cref="ArgumentNullException">Cuando el objeto request es nulo.</exception>
        /// <exception cref="AppValidationException">
        /// Cuando los datos de la solicitud no son válidos o faltan campos requeridos.
        /// Contiene un diccionario con los errores de validación.
        /// </exception>
        /// <exception cref="AppUnauthorizedException">
        /// Cuando las credenciales proporcionadas son inválidas o el usuario no está autorizado.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Cuando ocurre un error al comunicarse con el servidor de autenticación.
        /// </exception>
        /// <exception cref="JsonException">
        /// Cuando hay un error al procesar la respuesta del servidor.
        /// </exception>
        /// <exception cref="AppException">
        /// Para otros errores inesperados durante el proceso de autenticación.
        /// </exception>
        Task<bool> LoginAsync(LoginRequestDTO request);
    }
}
