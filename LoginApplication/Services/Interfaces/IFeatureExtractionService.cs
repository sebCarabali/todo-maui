using LoginApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    public interface IFeatureExtractionService
    {
        Task<(string Encoding, string Message)> ExtractFeaturesAsync(Stream image);

        Task<(double Score, string Message)> CompareFeatureAsync(string encodingFeature, Stream image);
    }
}
