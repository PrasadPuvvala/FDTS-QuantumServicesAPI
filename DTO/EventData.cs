using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class EventData
    {
        public string? eventDateTime { get; set; }
        public string? eventName { get; set; }
        public string? status { get; set; }
        public string? description { get; set; }
        public string? orderNumber { get; set; }
        public string? serialNumber { get; set; }
        public string? processName { get; set; }
        public string? processStepName { get; set; }
        public string? productName { get; set; }
        public string? siteId { get; set; }
        public string? workstationId { get; set; }
        public string? userId { get; set; }
        public string? tags { get; set; }
        public DomainProperties? domainProperties { get; set; }
    }
    public class DomainProperties
    {
        public string? productBrand { get; set; }
        public string? productFamily { get; set; }
        public string? testType { get; set; }
        public string? hybridSerialNumber { get; set; }
        public string? bleId { get; set; }
        public string? bleAddress { get; set; }
        public string? hearingInstrumentId { get; set; }
        public string? firmwareVersion { get; set; }
        public string? deviceName { get; set; }
        public string? tpiVersion { get; set; }
        public string? tpiReleaseCode { get; set; }
        public string? testFrameworkVersion { get; set; }
        public string? dsaPlatform { get; set; }
    }
}
