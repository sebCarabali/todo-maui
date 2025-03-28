using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.Services
{
    public class FacialAuthenticationService : IFacialAuthenticationService
    {
        private readonly IFeatureExtractionService _featureExtractionService;
        private readonly IClienteService _clienteService;

        public FacialAuthenticationService(
            IFeatureExtractionService featureExtractionService,
            IClienteService clienteService)
        {
            _featureExtractionService = featureExtractionService ?? throw new ArgumentNullException(nameof(featureExtractionService));
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
        }

        public async Task<bool> AuthenticateAsync(string identifier, Stream image)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("El criterio no puede estar vacío o ser nulo.", nameof(identifier));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
            }

            try
            {
                var encoding = await _clienteService.GetEncodingAsync(identifier);

                if (encoding == null)
                {
                    throw new ApplicationException("No se encuentra el vector de características con los datos proporcionados.");
                }

                var similarity = await _featureExtractionService.CompareFeatureAsync(encoding, image);
                return similarity < 0.40;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error durante la autenticación facial: {ex.Message}", ex);
            }
        }

    }
}