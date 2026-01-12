using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class VivianeRecommendationServicePage
    {
        private readonly APIHelperClass _APIHelper;
        public VivianeRecommendationServicePage()
        {
            _APIHelper = new APIHelperClass();
        }
        public async Task<RestResponse?> PostCorrectiveAction(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey, string vivianeRecommandationData)
        {
            try
            {
                if (apiEndpointsDTO?.apiEndpoint == null)
                {
                    throw new ArgumentNullException(nameof(apiEndpointsDTO.apiEndpoint), "API endpoint details cannot be null.");
                }

                var client = await _APIHelper.VivianeUrl(baseUrl, apiEndpointsDTO.apiEndpoint.PostCorrectiveAction);
                ExtentReportManager.GetInstance().LogToReport(test, Status.Info, $"Endpoint is {apiEndpointsDTO.apiEndpoint.PostCorrectiveAction}");
                var request = await _APIHelper.CreatePostRequest(apikey);
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
                var jsonFilePath = Path.Combine(projectRoot, "VivianeRecommendationServicesTestData", $"{vivianeRecommandationData}");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("EventData JSON file not found.", jsonFilePath);
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var Data = JsonSerializer.Deserialize<PostCorrectiveActionData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Data == null)
                {
                    throw new InvalidOperationException("Failed to deserialize VivianeRecommandationServiceData from JSON.");
                }

                // Add JSON body to request
                request.AddJsonBody(Data);
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

        public async Task<RestResponse?> PostFdtsDataForRecommendation(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey, string vivianeRecommandationData)
        {
            try
            {
                if (apiEndpointsDTO?.apiEndpoint == null)
                {
                    throw new ArgumentNullException(nameof(apiEndpointsDTO.apiEndpoint), "API endpoint details cannot be null.");
                }

                var client = await _APIHelper.VivianeUrl(baseUrl, apiEndpointsDTO.apiEndpoint.PostFdtsDataForRecommendation);
                ExtentReportManager.GetInstance().LogToReport(test, Status.Info, $"Endpoint is {apiEndpointsDTO.apiEndpoint.PostFdtsDataForRecommendation}");
                var request = await _APIHelper.CreatePostRequest(apikey);
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
                var jsonFilePath = Path.Combine(projectRoot, "VivianeRecommendationServicesTestData", $"{vivianeRecommandationData}");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("EventData JSON file not found.", jsonFilePath);
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var Data = JsonSerializer.Deserialize<PostFdtsDataForRecommendationData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Data == null)
                {
                    throw new InvalidOperationException("Failed to deserialize VivianeRecommandationServiceData from JSON.");
                }

                // Add JSON body to request
                request.AddJsonBody(Data);
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
        public async Task<RestResponse?> PostRecommendationFeedback(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey, string vivianeRecommandationData)
        {
            try
            {
                if (apiEndpointsDTO?.apiEndpoint == null)
                {
                    throw new ArgumentNullException(nameof(apiEndpointsDTO.apiEndpoint), "API endpoint details cannot be null.");
                }

                var client = await _APIHelper.VivianeUrl(baseUrl, apiEndpointsDTO.apiEndpoint.PostRecommendationFeedback);
                ExtentReportManager.GetInstance().LogToReport(test, Status.Info, $"Endpoint is {apiEndpointsDTO.apiEndpoint.PostRecommendationFeedback}");
                var request = await _APIHelper.CreatePostRequest(apikey);
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
                var jsonFilePath = Path.Combine(projectRoot, "VivianeRecommendationServicesTestData", $"{vivianeRecommandationData}");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("EventData JSON file not found.", jsonFilePath);
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var Data = JsonSerializer.Deserialize<PostCorrectiveActionData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Data == null)
                {
                    throw new InvalidOperationException("Failed to deserialize VivianeRecommandationServiceData from JSON.");
                }

                // Add JSON body to request
                request.AddJsonBody(Data);
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
