using LoginApplication.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class ApiSettings
    {
        private readonly ApiSetting _apiSettings;

        public ApiSettings(IOptions<ApiSetting> apiSettings)
        {
            this._apiSettings = apiSettings.Value;
        }

        public string BaseUrl => _apiSettings.BaseUrl;
        public string Key => _apiSettings.Key;
        public string PublicUser => _apiSettings.PublicUser;
        public string PublicPass => _apiSettings.PublicPass;
    }
}
