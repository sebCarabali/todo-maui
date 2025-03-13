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

        public async Task<bool> AuthenticateAsync(string criteria, Stream image)
        {
            if (string.IsNullOrWhiteSpace(criteria))
            {
                throw new ArgumentException("El criterio no puede estar vacío o ser nulo.", nameof(criteria));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "La imagen no puede ser nula.");
            }

            try
            {
                var featureExtractionData = await GetFeatureDataWithId(criteria);

                if (featureExtractionData == null)
                {
                    throw new ApplicationException("No se encuentra el vector de características con los datos proporcionados.");
                }

                var similarity = await _featureExtractionService.CompareFeatureAsync(featureExtractionData, image);
                return similarity < 0.15;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error durante la autenticación facial.", ex);
            }
        }

        private async Task<string> GetFeatureDataWithId(string criteria)
        {
            if (IsValidEmail(criteria))
            {
                return await _clienteService.GetFeatureExtractionDataAsyncByEmail(criteria);
            }

            return await _clienteService.GetFeatureExtractionDataAsyncByIdentification(criteria);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}