using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace QuantumServicesAPI.APIHelper
{
    public class ProcessControlServiceAPIHelperClass
    {
        private readonly RestClient restClient;
        public ProcessControlServiceAPIHelperClass()
        {
            restClient = new RestClient();
        }
        public Task<RestClient> SetUrl(string env, string region, string actionUrl, string partitionKey)
        {
            var url = $"https://{env}.{region}.api.apt.gn.com/process-control-service/v1/{actionUrl}/{partitionKey}";
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestRequest> CreatePostRequest(string apikey)
        {
            var request = new RestRequest { Method = Method.Post };
            string machineName = Environment.MachineName;
            // Add Headers
            request.AddHeader("Ocp-Apim-Subscription-Key", $"{apikey}");
            request.AddHeader("Content-Type", "application/json"); // Binary format
            request.AddHeader("username", "surya");
            request.AddHeader("machinename", machineName);
            request.AddHeader("site", "99");
            return Task.FromResult(request);
        }
    }
}
