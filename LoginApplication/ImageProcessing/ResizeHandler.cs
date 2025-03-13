using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal class ResizeHandler : ImageHandler
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ResizeHandler(int maxWidth, int maxHeight)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        protected override async Task<Stream> HandleAsync(Stream imageStream)
        {
            using (var inputStream = new SKManagedStream(imageStream))
            using (var bitmap = SKBitmap.Decode(inputStream))
            {
                // Calculate new dimensions while maintaining aspect ratio
                var (newWidth, newHeight) = CalculateDimensions(bitmap.Width, bitmap.Height);

                // Resize the image
                var resizedBitmap = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
                var resizedImage = SKImage.FromBitmap(resizedBitmap);

                // Convert to stream
                var outputStream = new MemoryStream();
                resizedImage.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
        }

        private (int Width, int Height) CalculateDimensions(int originalWidth, int originalHeight)
        {
            double ratioX = (double)_maxWidth / originalWidth;
            double ratioY = (double)_maxHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            return (newWidth, newHeight);
        }
    }
}
