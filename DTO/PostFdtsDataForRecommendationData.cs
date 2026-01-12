using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class PostFdtsDataForRecommendationData
    {
        public Metadata? Metadata { get; set; }
        public Payload? Payload { get; set; }
    }
    public class Metadata
    {
        public string? EventType { get; set; }
        public string? EventDateTime { get; set; }
        public string? ResourceId { get; set; }
        public string? SerialNumber { get; set; }
        public string? ProductName { get; set; }
        public string? MachineName { get; set; }
        public string? TestSite { get; set; }
        public string? Result { get; set; }
    }

    public class Payload
    {
        public string? ResponseAt90dB { get; set; }
        public string? ResponseAt60dB { get; set; }
        public string? ResponseAt50dB { get; set; }
    }
}
