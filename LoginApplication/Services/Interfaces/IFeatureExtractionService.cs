using LoginApplication.Dtos;
using LoginApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    public interface IFeatureExtractionService
    {
        Task<string> ExtractFeaturesAsync(Stream image);

        Task<double> CompareFeatureAsync(string encodingFeature, Stream image);
    }
}
