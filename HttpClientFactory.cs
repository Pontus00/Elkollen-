using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elkollen
{
    public static class HttpClientFactory
    {
        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var newHandler = new HttpClient(handler);
            newHandler.BaseAddress = new Uri("https://wattnowtestapi-d4a0h4dedmfnfybt.northeurope-01.azurewebsites.net");
            //newHandler.BaseAddress = new Uri("https://localhost:7197"); //Windows
            //newHandler.BaseAddress = new Uri("https://10.0.2.2:7197"); //Android
            return newHandler;
        }
    }
}