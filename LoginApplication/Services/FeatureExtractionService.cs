using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LoginApplication.Config;
using LoginApplication.Dtos;
using LoginApplication.ImageProcessing;
using LoginApplication.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LoginApplication.Services
{
    internal record ValidateEncodingResponse(bool Success, double Data, List<string> Messages);

    public class FeatureExtractionService : IFeatureExtractionService
    {
        private readonly ImageOptimizer _imageOptimizer;
        private HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly Endpoints _endpoints;

        public FeatureExtractionService(
            ImageOptimizer imageOptimizer,
            IOptionsSnapshot<Endpoints> endpoints,
            IOptionsSnapshot<AppSettings> appSettings
        )
        {
            _imageOptimizer =
                imageOptimizer ?? throw new ArgumentNullException(nameof(imageOptimizer));
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings =
                appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

            ConfigureHttpClient();
        }

        public async Task<double> CompareFeatureAsync(string encoding, Stream image)
        {
            if (string.IsNullOrWhiteSpace(encoding))
            {
                throw new ArgumentException(
                    "El encoding no puede estar vacío o ser nulo.",
                    nameof(encoding)
                );
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
            }

            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(image);
                streamContent.Headers.Add("Content-Type", "image/jpeg");
                content.Add(streamContent, "file", "image.jpg");
                content.Add(new StringContent(encoding), "encodingJson");

                var apiUrl = $"{_appSettings.FaceRecognitionApi}{_endpoints.ValidateEncoding}";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Error al llamar a la API de extracción: {response.StatusCode}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ValidateEncodingResponse>(jsonResponse);

                if (data == null)
                {
                    throw new InvalidOperationException(
                        "La respuesta de la API no pudo ser deserializada."
                    );
                }

                return data.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException(
                    "Error al llamar a la API de extracción de características.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al comparar características.", ex);
            }
        }

        public async Task<string> ExtractFeaturesAsync(Stream image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
            }

            try
            {
                var compressedImage = await _imageOptimizer.OptimizeAsync(image);
                return await CallFeatureExtractionApiAsync(compressedImage);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Error al extraer características de la imagen.",
                    ex
                );
            }
        }

        private async Task<string> CallFeatureExtractionApiAsync(Stream compressedImage)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(compressedImage);
                streamContent.Headers.Add("Content-Type", "image/jpeg");
                content.Add(streamContent, "file", "image.jpg");

                var apiUrl = $"{_appSettings.FaceRecognitionApi}{_endpoints.GetEncodingFile}";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Error calling the feature extraction API: {response.StatusCode}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();


                var apiResponse = JsonConvert.DeserializeObject<EncodingFileApiResponse>(jsonResponse);

                return apiResponse?.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException(
                    "Error al llamar a la API de extracción de características.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Error inesperado al llamar a la API de extracción de características.",
                    ex
                );
            }
        }

        private void ConfigureHttpClient()
        {
            _httpClient = new HttpClient();
        }
    }
}
