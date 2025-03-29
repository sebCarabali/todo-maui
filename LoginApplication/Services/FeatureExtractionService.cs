using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LoginApplication.Config;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LoginApplication.Services
{
    internal record ValidateEncodingResponse(bool Success, double Data, List<string> Messages);
    internal record EncodingFileApiResponse(bool Success, string Data, List<string> Messages);
    internal record ApiErrorResponse(string Message, string Details);

    public class FeatureExtractionService : IFeatureExtractionService
    {
        private readonly ImageOptimizer _imageOptimizer;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly Endpoints _endpoints;

        public FeatureExtractionService(
            ImageOptimizer imageOptimizer,
            IOptionsSnapshot<Endpoints> endpoints,
            IOptionsSnapshot<AppSettings> appSettings)
        {
            _imageOptimizer = imageOptimizer ?? throw new ArgumentNullException(nameof(imageOptimizer));
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _httpClient = CreateHttpClient();
        }

        public async Task<(double Score, string Message)> CompareFeatureAsync(string encoding, Stream image)
        {
            ValidateInputs(encoding, image);

            try
            {
                using var content = CreateMultipartContent(encoding, image);
                var response = await PostToApiAsync($"{_appSettings.FaceRecognitionApi}{_endpoints.ValidateEncoding}", content);

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ValidateEncodingResponse>(jsonResponse) ??
                    throw new InvalidOperationException("La respuesta de la API no pudo ser deserializada.");

                if (!data.Success)
                {
                    var errorMessage = data.Messages?.FirstOrDefault() ?? "Error desconocido al validar el encoding";
                    return (0, errorMessage);
                }

                return (data.Data, "Comparación exitosa");
            }
            catch (Exception ex)
            {
                throw HandleServiceException("Error al comparar características", ex);
            }
        }

        public async Task<(string Encoding, string Message)> ExtractFeaturesAsync(Stream image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");

            try
            {
                using var compressedImage = await _imageOptimizer.OptimizeAsync(image);
                return await CallFeatureExtractionApiAsync(compressedImage);
            }
            catch (Exception ex)
            {
                throw HandleServiceException("Error al extraer características de la imagen", ex);
            }
        }

        private async Task<(string? Encoding, string Message)> CallFeatureExtractionApiAsync(Stream compressedImage)
        {
            try
            {
                using var content = CreateMultipartContent(compressedImage);
                var response = await PostToApiAsync($"{_appSettings.FaceRecognitionApi}{_endpoints.GetEncodingFile}", content);

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<EncodingFileApiResponse>(jsonResponse) ??
                    throw new InvalidOperationException("La respuesta de la API no pudo ser deserializada.");


                return (apiResponse.Data, "Extracción de características exitosa");
            }
            catch (Exception ex)
            {
                throw HandleServiceException("Error al llamar a la API de extracción", ex);
            }
        }

        #region Helper Methods

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            return client;
        }

        private MultipartFormDataContent CreateMultipartContent(string encoding, Stream image)
        {
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(image);
            streamContent.Headers.Add("Content-Type", "image/jpeg");
            content.Add(streamContent, "file", "image.jpg");
            content.Add(new StringContent(encoding), "encodingJson");
            return content;
        }

        private MultipartFormDataContent CreateMultipartContent(Stream compressedImage)
        {
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(compressedImage);
            streamContent.Headers.Add("Content-Type", "image/jpeg");
            content.Add(streamContent, "file", "image.jpg");
            return content;
        }

        private async Task<HttpResponseMessage> PostToApiAsync(string apiUrl, HttpContent content)
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

        private void ValidateInputs(string encoding, Stream image)
        {
            if (string.IsNullOrWhiteSpace(encoding))
                throw new ArgumentException("El encoding no puede estar vacío o ser nulo.", nameof(encoding));

            if (image == null)
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
        }

        private ApplicationException HandleServiceException(string contextMessage, Exception ex)
        {
            return ex switch
            {
                HttpRequestException httpEx =>
                    new ApplicationException($"{contextMessage}: Error de comunicación con la API", httpEx),
                _ => new ApplicationException($"{contextMessage}: Error inesperado", ex)
            };
        }

        #endregion
    }
}