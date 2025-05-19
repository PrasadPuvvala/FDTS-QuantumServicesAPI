using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.APIHelper
{
    public class APIHelperClass
    {
        private readonly RestClient restClient;

        public APIHelperClass()
        {
            restClient = new RestClient();
        }
        public Task<RestClient> OCRUrl(string baseUrl, string endpoint)
        {
            var url = Path.Combine($"{baseUrl}/", endpoint);
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestClient> ProcessControlUrl(string baseUrl, string actionUrl, string partitionKey)
        {
            var url = $"{baseUrl}/{actionUrl}/{partitionKey}";
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestClient> MFGDataUrl(string baseUrl, string endpoint, string mfgDataFile)
        {
            string mfgFile = mfgDataFile.Substring(0, mfgDataFile.Length - ".zip".Length);
            string testDate = DateTime.Now.ToString("yyyy-MM-dd");
            var url = $"{baseUrl}/{endpoint}/{testDate}/{mfgFile}";
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestRequest> CreatePostRequest(string apikey)
        {
            var request = new RestRequest { Method = Method.Post };
            string machineName = Environment.MachineName;
            // Add Headers
            request.AddHeader("Ocp-Apim-Subscription-Key", apikey);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/octet-stream"); // Binary format
            request.AddHeader("username", "surya");
            request.AddHeader("machinename", machineName);
            request.AddHeader("site", "99");
            return Task.FromResult(request);
        }
    }
}
