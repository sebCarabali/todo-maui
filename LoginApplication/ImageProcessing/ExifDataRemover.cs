using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal class ExifDataRemover : ImageHandler
    {
        protected override async Task<Stream> HandleAsync(Stream imageStream)
        {
            using (var inputStream = new SKManagedStream(imageStream))
            using (var bitmap = SKBitmap.Decode(inputStream))
            {
                // Create a new image without EXIF data
                var image = SKImage.FromBitmap(bitmap);

                // Encode without metadata
                var outputStream = new MemoryStream();
                image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
        }

    }
}
