using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LoginApplication.Dtos;
using Newtonsoft.Json;

namespace LoginApplication.Utils
{
    public class HttpUtils
    {
        public static MultipartFormDataContent CreateMultipartContent(Stream compressedImage)
        {
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(compressedImage);
            streamContent.Headers.Add("Content-Type", "image/jpeg");
            content.Add(streamContent, "file", "image.jpg");
            return content;
        }

        public static async Task<HttpResponseMessage> PostToApiAsync(HttpClient _httpClient, string apiUrl, HttpContent content)
        {
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error en la API: {response.StatusCode}. Respuesta: {errorContent}");
            }

            return response;
        }

        public static ApplicationException HandleServiceException(string contextMessage, Exception ex)
        {
            return ex switch
            {
                HttpRequestException httpEx =>
                    new ApplicationException($"{contextMessage}: Error de comunicación con la API", httpEx),
                _ => new ApplicationException($"{contextMessage}: Error inesperado", ex)
            };
        }

        public static MultipartFormDataContent CreateClientMultipartContent(Cliente cliente, Stream image)
        {
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(image);
            streamContent.Headers.Add("Content-Type", "image/jpeg");
            content.Add(streamContent, "file", "image.jpg");
            content.Add(new StringContent(cliente.identificacion), "identificacion");
            content.Add(new StringContent(cliente.primer_nombre), "primer_nombre");
            content.Add(new StringContent(cliente.segundo_nombre ?? string.Empty), "segundo_nombre");
            content.Add(new StringContent(cliente.primer_apellido), "primer_apellido");
            content.Add(new StringContent(cliente.segundo_apellido ?? string.Empty), "segundo_apellido");
            content.Add(new StringContent(cliente.correo), "correo");
            content.Add(new StringContent(cliente.celular), "celular");
            return content;
        }
    }
}
