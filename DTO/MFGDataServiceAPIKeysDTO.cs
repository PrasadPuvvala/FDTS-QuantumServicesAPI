using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class MFGDataServiceAPIKeysDTO
    {
        public MFGDataServiceAPIKeys? mfgDataServiceapiKeys { get; set; }
    }
    public class MFGDataServiceAPIKeys
    {
        public string DevEurope { get; set; } = string.Empty;
        public string DevUs { get; set; } = string.Empty;
        public string DevAsia { get; set; } = string.Empty;
        public string TstEurope { get; set; } = string.Empty;
        public string TstUs { get; set; } = string.Empty;
        public string TstAsia { get; set; } = string.Empty;
        public string PrdEurope { get; set; } = string.Empty;
        public string PrdUs { get; set; } = string.Empty;
        public string PrdAsia { get; set; } = string.Empty;
    }
}
