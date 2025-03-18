using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elkollen
{
    public static class HttpClientFactory
    {
        //Singleton. Returnerar samma HttpClient varje gång.
        private static readonly HttpClient _httpClient;

        static HttpClientFactory()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://wattnowtestapi-d4a0h4dedmfnfybt.northeurope-01.azurewebsites.net");
        }

        public static HttpClient CreateHttpClient()
        {
            return _httpClient;
        }
    }
}