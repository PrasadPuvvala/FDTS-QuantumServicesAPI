using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    /// <summary>
    /// Represents event data with various properties related to the event, product, and process.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public string? eventDateTime { get; set; }
        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string? eventName { get; set; }
        /// <summary>
        /// Gets or sets the status of the event.
        /// </summary>
        public string? status { get; set; }
        /// <summary>
        /// Gets or sets the description of the event.
        /// </summary>
        public string? description { get; set; }
        /// <summary>
        /// Gets or sets the order number associated with the event.
        /// </summary>
        public string? orderNumber { get; set; }
        /// <summary>
        /// Gets or sets the serial number associated with the event.
        /// </summary>
        public string? serialNumber { get; set; }
        /// <summary>
        /// Gets or sets the name of the process associated with the event.
        /// </summary>
        public string? processName { get; set; }
        /// <summary>
        /// Gets or sets the name of the process step associated with the event.
        /// </summary>
        public string? processStepName { get; set; }
        /// <summary>
        /// Gets or sets the name of the product associated with the event.
        /// </summary>
        public string? productName { get; set; }
        /// <summary>
        /// Gets or sets the site ID associated with the event.
        /// </summary>
        public string? siteId { get; set; }
        /// <summary>
        /// Gets or sets the workstation ID associated with the event.
        /// </summary>
        public string? workstationId { get; set; }
        /// <summary>
        /// Gets or sets the user ID associated with the event.
        /// </summary>
        public string? userId { get; set; }
        /// <summary>
        /// Gets or sets the tags associated with the event.
        /// </summary>
        public string? tags { get; set; }
        /// <summary>
        /// Gets or sets the domain properties associated with the event.
        /// </summary>
        public DomainProperties? domainProperties { get; set; }
    }
    /// <summary>
    /// Represents domain-specific properties associated with an event.
    /// </summary>
    public class DomainProperties
    {
        /// <summary>
        /// Gets or sets the product brand.
        /// </summary>
        public string? productBrand { get; set; }
        /// <summary>
        /// Gets or sets the product family.
        /// </summary>
        public string? productFamily { get; set; }
        /// <summary>
        /// Gets or sets the test type.
        /// </summary>
        public string? testType { get; set; }
        /// <summary>
        /// Gets or sets the hybrid serial number.
        /// </summary>
        public string? hybridSerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the BLE ID.
        /// </summary>
        public string? bleId { get; set; }
        /// <summary>
        /// Gets or sets the BLE address.
        /// </summary>
        public string? bleAddress { get; set; }
        /// <summary>
        /// Gets or sets the hearing instrument ID.
        /// </summary>
        public string? hearingInstrumentId { get; set; }
        /// <summary>
        /// Gets or sets the firmware version.
        /// </summary>
        public string? firmwareVersion { get; set; }
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string? deviceName { get; set; }
        /// <summary>
        /// Gets or sets the TPI version.
        /// </summary>
        public string? tpiVersion { get; set; }
        /// <summary>
        /// Gets or sets the TPI release code.
        /// </summary>
        public string? tpiReleaseCode { get; set; }
        /// <summary>
        /// Gets or sets the test framework version.
        /// </summary>
        public string? testFrameworkVersion { get; set; }
        /// <summary>
        /// Gets or sets the DSA platform.
        /// </summary>
        public string? dsaPlatform { get; set; }
    }
}
