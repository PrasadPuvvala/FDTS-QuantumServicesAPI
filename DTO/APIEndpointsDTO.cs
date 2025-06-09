using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    /// <summary>
    /// Data Transfer Object for API Endpoints.
    /// </summary>
    public class APIEndpointsDTO
    {
        /// <summary>
        /// Gets or sets the API endpoint.
        /// </summary>
        public APIEndpoint? apiEndpoint { get; set; }
    }

    /// <summary>
    /// Represents a specific API endpoint with its related properties.
    /// </summary>
    public class APIEndpoint
    {
        /// <summary>
        /// Gets or sets the route for analyzing images.
        /// </summary>
        public string AnalyzeImage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL for performing actions.
        /// </summary>
        public string actionUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the partition key for data storage.
        /// </summary>
        public string partitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the route for storing test data.
        /// </summary>
        public string storeTestDataRoute { get; set; } = string.Empty;
    }
}
