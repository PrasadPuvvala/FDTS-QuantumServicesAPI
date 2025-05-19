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
    public class MFGDataServicePage
    {
        private readonly APIHelperClass _APIHelper;
        public MFGDataServicePage()
        {
            _APIHelper = new APIHelperClass();
        }
        public async Task<RestResponse?> PostMFGData(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string mfgDataFile, string baseUrl, string apikey)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
            string mfgDataFilePath = Path.Combine(projectRoot, "MFGDataFiles", mfgDataFile);

            if (!File.Exists(mfgDataFilePath))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, $"File not found: {mfgDataFilePath}");
                return null;
            }

            var client = await _APIHelper.MFGDataUrl(baseUrl, apiEndpointsDTO.apiEndpoint.storeTestDataRoute, mfgDataFile);
            var request = await _APIHelper.CreatePostRequest(apikey);
            try
            {
                // Read Image as Byte Array
                byte[] mfgDataBytes = await File.ReadAllBytesAsync(mfgDataFilePath);

                // Add Image as Binary Body
                request.AddParameter("application/octet-stream", mfgDataBytes, ParameterType.RequestBody);

                // Execute Request
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
