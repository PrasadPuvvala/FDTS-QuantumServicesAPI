using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    /// <summary>
    /// Provides methods for interacting with the Process Control Service API.
    /// </summary>
    public class ProcessControlServicePage
    {
        private readonly APIHelperClass _APIHelper;
        private static List<double> _responseTimes = new List<double>();  // Stores response times

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessControlServicePage"/> class.
        /// </summary>
        public ProcessControlServicePage()
        {
            _APIHelper = new APIHelperClass();
        }

        /// <summary>
        /// Posts event data to the specified API endpoint.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint details.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <param name="eventMetaData">The name of the JSON file containing event metadata.</param>
        /// <returns>The RestResponse from the API, or null if an error occurs.</returns>
        /// <exception cref="ArgumentNullException">Thrown if API endpoint details are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the EventData JSON file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if deserializing EventData from JSON fails.</exception>
        public async Task<RestResponse?> PostEventData(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey, string eventMetaData)
        {
            try
            {
                if (apiEndpointsDTO?.apiEndpoint == null)
                {
                    throw new ArgumentNullException(nameof(apiEndpointsDTO.apiEndpoint), "API endpoint details cannot be null.");
                }

                var client = await _APIHelper.ProcessControlUrl(baseUrl, apiEndpointsDTO.apiEndpoint.actionUrl, apiEndpointsDTO.apiEndpoint.partitionKey);
                var request = await _APIHelper.CreatePostRequest(apikey);
                // Load EventData from JSON file
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
                var jsonFilePath = Path.Combine(projectRoot, "ProcessControlTestData", $"{eventMetaData}");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("EventData JSON file not found.", jsonFilePath);
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var eventData = JsonSerializer.Deserialize<EventData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventData == null)
                {
                    throw new InvalidOperationException("Failed to deserialize EventData from JSON.");
                }

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

        /// <summary>
        /// Posts event data to the specified API endpoint and measures the response time.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint details.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <param name="eventMetaData">The name of the JSON file containing event metadata.</param>
        /// <returns>The RestResponse from the API, or null if an error occurs.</returns>
        /// <exception cref="ArgumentNullException">Thrown if API endpoint details are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the EventData JSON file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if deserializing EventData from JSON fails.</exception>
        public async Task<RestResponse?> PostEventDataResponseTime(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey, string eventMetaData)
        {
            try
            {
                if (apiEndpointsDTO?.apiEndpoint == null)
                {
                    throw new ArgumentNullException(nameof(apiEndpointsDTO.apiEndpoint), "API endpoint details cannot be null.");
                }

                var client = await _APIHelper.ProcessControlUrl(baseUrl, apiEndpointsDTO.apiEndpoint.actionUrl, apiEndpointsDTO.apiEndpoint.partitionKey);
                var request = await _APIHelper.CreatePostRequest(apikey);
                // Load EventData from JSON file
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
                var jsonFilePath = Path.Combine(projectRoot, "ProcessControlTestData", $"{eventMetaData}");

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("EventData JSON file not found.", jsonFilePath);
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var eventData = JsonSerializer.Deserialize<EventData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventData == null)
                {
                    throw new InvalidOperationException("Failed to deserialize EventData from JSON.");
                }

                // Add JSON body to request
                request.AddJsonBody(eventData);

                // Measure response time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Execute the request
                var response = await client.ExecuteAsync(request);

                stopwatch.Stop();
                long responseTimeMs = stopwatch.ElapsedMilliseconds;

                // Store response time
                _responseTimes.Add(responseTimeMs);
                double medianResponseTime = CalculateMedian(_responseTimes);

                if (medianResponseTime < 3000)
                {
                    ExtentReportManager.GetInstance().LogToReport(test, Status.Pass, $"Median response time is below 3 seconds and actual response time is : {medianResponseTime} milli scenods");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(test, Status.Fail, $"Median response time is not below 3 seconds and actual response time is : {medianResponseTime} milli scenods");
                }

                return response;
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, $"{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calculates the median value from a list of doubles.
        /// </summary>
        /// <param name="responseTimes">The list of response times.</param>
        /// <returns>The median response time.</returns>
        private double CalculateMedian(List<double> responseTimes)
        {
            if (responseTimes.Count == 0)
            {
                return 0;  // Prevents division errors
            }
            responseTimes.Sort();
            int count = responseTimes.Count;
            if (count % 2 == 0)
            {
                // Even number of elements
                double midElement1 = responseTimes[(count / 2) - 1];
                double midElement2 = responseTimes[count / 2];
                return (midElement1 + midElement2) / 2.0;
            }
            else
            {
                // Odd number of elements
                return responseTimes[count / 2];
            }
        }
    }
}
