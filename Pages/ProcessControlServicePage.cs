using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using RestSharp;

namespace QuantumServicesAPI.Pages
{
    public class ProcessControlServicePage
    {
        private readonly APIHelperClass _APIHelper;
        public ProcessControlServicePage()
        {
            _APIHelper = new APIHelperClass();
        }
        public async Task<RestResponse?> PostEventData(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey)
        {
            try
            {
                var client = await _APIHelper.ProcessControlUrl(baseUrl, apiEndpointsDTO.apiEndpoint.actionUrl, apiEndpointsDTO.apiEndpoint.partitionKey);
                var request = await _APIHelper.CreatePostRequest(apikey);
                // Create your DTO and fill it with data
                var eventData = new EventData
                {
                    eventDateTime = DateTime.Parse("2025-03-05T10:27:14.9703827Z"),
                    eventName = "DSATestCompleted",
                    status = "Success",
                    description = "DSA test event data",
                    orderNumber = null,
                    serialNumber = "2100819205",
                    processName = null,
                    processStepName = null,
                    productName = "RU962-DRW",
                    siteId = "21 - GN ReSound, Ballerup (Denmark)",
                    workstationId = "DSA7-3157588",
                    userId = "TESTER",
                    domainProperties = new DomainProperties
                    {
                        productBrand = "ReSound",
                        productFamily = "ReSound OMNIA 9",
                        testType = "",
                        hybridSerialNumber = "BE27-570A-6150",
                        bleId = "1092029696",
                        bleAddress = "",
                        hearingInstrumentId = "GDImT27D0IBWU-Zu55PQb2T8qi0",
                        firmwareVersion = "",
                        deviceName = "RS.D3.Top.CbRIE13DW.7.44.1.1",
                        tpiVersion = "1.0.28",
                        tpiReleaseCode = "0000-0000-0000",
                        testFrameworkVersion = "13.7.0",
                        dsaPlatform = "FDTS"
                    }
                };
                // Add JSON body to request
                request.AddJsonBody(eventData);
                // Execute the request
                var response = await client.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, $"{ex.Message}");
                return null;
            }
        }
    }
}
