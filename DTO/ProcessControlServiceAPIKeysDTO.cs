using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class ProcessControlServiceAPIKeysDTO
    {
        public ProcessControlServiceAPIKeys? processControlServiceAPIKeys { get; set; }
    }
    public class ProcessControlServiceAPIKeys
    {
        public string DevEurope { get; set; } = string.Empty;
        public string DevUs { get; set; } = string.Empty;
        public string DevAsia { get; set; } = string.Empty;
        public string TstEurope { get; set; } = string.Empty;
        public string TstUs { get; set; } = string.Empty;
        public string TstAsia { get; set; } = string.Empty;
    }
}
