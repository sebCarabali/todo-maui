using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Config
{
    public class AppSettings
    {
        public string BaseUrl { get; set; }
        public string Key { get; set; }
        public string PublicUser { get; set; }
        public string PublicPass { get; set; }

        public string FaceRecognitionApi { get; set; }
    }

    public class Endpoints
    {
        public string Login { get; set; }
        public string Authenticate { get; set; }
        public string EncodingByEmail { get; set; }

        public string GetEncodingFile { get; set; }

        public string ValidateEncoding { get; set; }

        public string UpdateEncoding { get; set; }

        public string SaveAccessLog { get; set; }

        public string AddClient { get; set; }
    }

    public class ImageOptimization
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quality { get; set; }
    }
}
