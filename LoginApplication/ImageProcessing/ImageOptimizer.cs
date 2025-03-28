using LoginApplication.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    public class ImageOptimizer
    {
        private readonly CompressionPipeline _pipeline;
        private readonly ImageOptimization _imageOptimization;

        public ImageOptimizer(IOptions<ImageOptimization> imageOptimization)
        {
            _imageOptimization = imageOptimization.Value;
            _pipeline = new CompressionPipeline()
                .AddHandler(new QualityHandler(_imageOptimization.Quality))
                .AddHandler(new ResizeHandler(_imageOptimization.Width, _imageOptimization.Height))
                .AddHandler(new ExifDataRemover());
        }

        public async Task<Stream> OptimizeAsync(Stream imageStream)
        {
            if (imageStream == null)
                throw new ArgumentNullException(nameof(imageStream));

            return await _pipeline.ProcessAsync(imageStream);
        }
    }
}
