using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class APIEndpointsDTO
    {
        public APIEndpoint? apiEndpoint { get; set; }
    }
    public class APIEndpoint
    {
        public string AnalyzeImage { get; set; } = string.Empty;
    }
}
