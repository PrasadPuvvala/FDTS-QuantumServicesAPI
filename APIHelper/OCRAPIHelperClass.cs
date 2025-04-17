using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace QuantumServicesAPI.APIHelper
{
    public class OCRAPIHelperClass
    {
        private readonly RestClient restClient;

        public OCRAPIHelperClass()
        {
            restClient = new RestClient();
        }
        public Task<RestClient> SetUrl(string env, string region, string endpoint)
        {
            var url = Path.Combine($"https://{env}.{region}.api.apt.gn.com/ocr-service/v1/", endpoint);
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestRequest> CreatePostRequest(string apikey)
        {
            var request = new RestRequest { Method = Method.Post };
            // Add Headers
            request.AddHeader("Ocp-Apim-Subscription-Key", $"{apikey}");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/octet-stream"); // Binary format
            request.AddHeader("username", "surya");
            request.AddHeader("machinename", "FSWIRAY112");
            request.AddHeader("site", "99");
            return Task.FromResult(request);
        }
    }
}
