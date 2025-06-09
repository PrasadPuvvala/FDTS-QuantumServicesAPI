using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.APIHelper
{
    /// <summary>
    /// Helper class for interacting with REST APIs using RestSharp.
    /// </summary>
    public class APIHelperClass
    {
        private readonly RestClient restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIHelperClass"/> class.
        /// </summary>
        public APIHelperClass()
        {
            restClient = new RestClient();
        }

        /// <summary>
        /// Constructs a RestClient for OCR-related API calls.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="endpoint">The specific endpoint for the OCR API.</param>
        /// <returns>A Task containing the RestClient.</returns>
        public Task<RestClient> OCRUrl(string baseUrl, string endpoint)
        {
            var url = Path.Combine($"{baseUrl}/", endpoint);
            return Task.FromResult(new RestClient(url));
        }

        /// <summary>
        /// Constructs a RestClient for Process Control-related API calls.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="actionUrl">The action URL for the Process Control API.</param>
        /// <param name="partitionKey">The partition key for the Process Control API.</param>
        /// <returns>A Task containing the RestClient.</returns>
        public Task<RestClient> ProcessControlUrl(string baseUrl, string actionUrl, string partitionKey)
        {
            var url = $"{baseUrl}/{actionUrl}/{partitionKey}";
            return Task.FromResult(new RestClient(url));
        }

        /// <summary>
        /// Constructs a RestClient for MFG Data-related API calls.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="endpoint">The endpoint for the MFG Data API.</param>
        /// <param name="mfgDataFile">The MFG data file name (including the .zip extension).</param>
        /// <returns>A Task containing the RestClient.</returns>
        public Task<RestClient> MFGDataUrl(string baseUrl, string endpoint, string mfgDataFile)
        {
            string mfgFile = mfgDataFile.Substring(0, mfgDataFile.Length - ".zip".Length);
            string testDate = DateTime.Now.ToString("yyyy-MM-dd");
            var url = $"{baseUrl}/{endpoint}/{testDate}/{mfgFile}";
            return Task.FromResult(new RestClient(url));
        }

        /// <summary>
        /// Creates a RestRequest for POST operations with common headers.
        /// </summary>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>A Task containing the RestRequest.</returns>
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
