using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.DTO
{
    public class gRPCDeviceInfo
    {
        public class DeviceSerialNumber
        {
            public string SerialNumber { get; set; } = string.Empty;
        }
        public class HearingInstrumentInformation
        {
            public string BatteryType { get; set; } = string.Empty;
        }
        public class ProductIdentificationInformation
        {
            public string FittingSide {  get; set; } = string.Empty;
            public string BleId {  get; set; } = string.Empty;
            public string Brand {  get; set; } = string.Empty;
            public string PrivateLabelCode {  get; set; } = string.Empty;
            public string MFIBrand {  get; set; } = string.Empty;
            public string MFIModel {  get; set; } = string.Empty;
            public string MFIFamily {  get; set; } = string.Empty;
            public string GapDeviceName {  get; set; } = string.Empty;
            public string optionsForDevice {  get; set; } = string.Empty;
        }
        public class DeviceImageInformation
        {
            public string fdiPath { get; set; } = string.Empty;
            public string hdiPath { get; set; } = string.Empty;

        }
        public class ProductionTestDataInformation
        {
            public string TestSite { get; set; } = string.Empty;
            public string TestStation { get; set; } = string.Empty;
            public string TPIReleaseCode { get; set; } = string.Empty;
            public string Year { get; set; } = string.Empty;
            public string Month { get; set; } = string.Empty;
            public string Day { get; set; } = string.Empty;
            public string Hour { get; set; } = string.Empty;
            public string Minute { get; set; } = string.Empty;
            public string Second { get; set; } = string.Empty;
            public string ModelVerificationId { get; set; } = string.Empty;
        }
    }
}
