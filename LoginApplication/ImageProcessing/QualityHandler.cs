using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal class QualityHandler : ImageHandler
    {
        private readonly int _quality;

        public QualityHandler(int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "La calidad debe estar entre 0 y 100.");

            _quality = quality;
        }

        protected override async Task<Stream> HandleAsync(Stream imageStream)
        {
            using (var inputStream = new SKManagedStream(imageStream))
            using (var bitmap = SKBitmap.Decode(inputStream))
            {
                var image = SKImage.FromBitmap(bitmap);

                var outputStream = new MemoryStream();
                image.Encode(SKEncodedImageFormat.Jpeg, _quality).SaveTo(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
        }
    }
}
