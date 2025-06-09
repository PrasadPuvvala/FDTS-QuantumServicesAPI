using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) for encapsulating Process Control Service API Keys.
    /// </summary>
    public class ProcessControlServiceAPIKeysDTO
    {
        /// <summary>
        /// Gets or sets the Process Control Service API Keys.
        /// </summary>
        public ProcessControlServiceAPIKeys? processControlServiceAPIKeys { get; set; }
    }

    /// <summary>
    /// Represents the API keys for different environments and regions.
    /// </summary>
    public class ProcessControlServiceAPIKeys
    {
        /// <summary>
        /// Gets or sets the API key for the Development environment in Europe.
        /// </summary>
        public string DevEurope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Development environment in the US.
        /// </summary>
        public string DevUs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Development environment in Asia.
        /// </summary>
        public string DevAsia { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Test environment in Europe.
        /// </summary>
        public string TstEurope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Test environment in the US.
        /// </summary>
        public string TstUs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Test environment in Asia.
        /// </summary>
        public string TstAsia { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Production environment in Europe.
        /// </summary>
        public string PrdEurope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Production environment in the US.
        /// </summary>
        public string PrdUs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for the Production environment in Asia.
        /// </summary>
        public string PrdAsia { get; set; } = string.Empty;
    }
}