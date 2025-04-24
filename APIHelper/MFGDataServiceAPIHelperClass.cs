using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace QuantumServicesAPI.APIHelper
{
    public class MFGDataServiceAPIHelperClass
    {
        private readonly RestClient restClient;
        public MFGDataServiceAPIHelperClass()
        {
            restClient = new RestClient();
        }
        public Task<RestClient> SetUrl(string env, string region, string endpoint, string mfgDataFile)
        {
            string mfgFile = mfgDataFile.Substring(0, mfgDataFile.Length - ".zip".Length);
            string testDate = DateTime.Now.ToString("yyyy-MM-dd");
            var url = $"https://{env}.{region}.api.apt.gn.com/mfg-data-service/v1/{endpoint}/{testDate}/{mfgFile}";
            return Task.FromResult(new RestClient(url));
        }
        public Task<RestRequest> CreatePostRequest(string apikey)
        {
            var request = new RestRequest { Method = Method.Post };
            string machineName = Environment.MachineName;
            // Add Headers
            request.AddHeader("Ocp-Apim-Subscription-Key", $"{apikey}");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/octet-stream"); // Binary format
            request.AddHeader("username", "surya");
            request.AddHeader("machinename", machineName);
            request.AddHeader("site", "99");
            return Task.FromResult(request);
        }
    }
}
