using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LoginApplication.Dtos
{

    public class EncodingFileApiResponse
    {
        public bool Exito { get; set; }
        public string Data { get; set; }
        public List<string> Mensajes { get; set; }
    }

    public class EncodingData
    {
        [JsonProperty("encoding")]
        public List<EncodingItem> Encoding { get; set; }
    }

    public class EncodingItem
    {
        [JsonProperty("_Encoding")]
        public List<double> EncodingData { get; set; }

        [JsonProperty("Rows")]
        public int Rows { get; set; }

        [JsonProperty("Columns")]
        public int Columns { get; set; }
    }
}
