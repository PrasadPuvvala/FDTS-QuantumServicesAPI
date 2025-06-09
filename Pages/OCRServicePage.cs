using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using CucumberExpressions;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using RestSharp;

namespace QuantumServicesAPI.Pages
{
    /// <summary>
    /// Provides methods for interacting with the OCR service, including sending image analysis requests and measuring response times.
    /// </summary>
    public class OCRServicePage
    {
        private readonly APIHelperClass _APIHelper;
        private static List<double> _responseTimes = new List<double>();  // Stores response times

        /// <summary>
        /// Initializes a new instance of the <see cref="OCRServicePage"/> class.
        /// </summary>
        public OCRServicePage()
        {
            _APIHelper = new APIHelperClass();
        }

        /// <summary>
        /// Sends an image analysis request to the OCR service.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint information.</param>
        /// <param name="image">The name of the image file.</param>
        /// <param name="baseUrl">The base URL of the OCR service.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>The RestResponse from the API, or null if an error occurred.</returns>
        public async Task<RestResponse?> PostImageRequest(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string image, string baseUrl, string apikey)
        {
            if (apiEndpointsDTO.apiEndpoint == null || string.IsNullOrEmpty(apiEndpointsDTO.apiEndpoint.AnalyzeImage))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, "API endpoint or AnalyzeImage is null or empty.");
                return null;
            }
            var client = await _APIHelper.OCRUrl(baseUrl, apiEndpointsDTO.apiEndpoint.AnalyzeImage);
            var request = await _APIHelper.CreatePostRequest(apikey);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
            string imageFolderPath = Path.Combine(projectRoot, "OCRImages", image);

            try
            {
                // Read Image as Byte Array
                byte[] imageBytes = await File.ReadAllBytesAsync(imageFolderPath);

                // Add Image as Binary Body
                request.AddParameter("application/octet-stream", imageBytes, ParameterType.RequestBody);

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

        /// <summary>
        /// Sends an image analysis request to the OCR service and measures the median response time.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint information.</param>
        /// <param name="image">The name of the image file.</param>
        /// <param name="baseUrl">The base URL of the OCR service.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>The RestResponse from the API, or null if an error occurred.</returns>
        public async Task<RestResponse?> PostImageMedianResponseTime(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string image, string baseUrl, string apikey)
        {
            if (apiEndpointsDTO.apiEndpoint == null || string.IsNullOrEmpty(apiEndpointsDTO.apiEndpoint.AnalyzeImage))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, "API endpoint or AnalyzeImage is null or empty.");
                return null;
            }
            var client = await _APIHelper.OCRUrl(baseUrl, apiEndpointsDTO.apiEndpoint.AnalyzeImage);
            var request = await _APIHelper.CreatePostRequest(apikey);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
            string imageFolderPath = Path.Combine(projectRoot, "OCRImages", image);

            try
            {
                // Read Image as Byte Array
                byte[] imageBytes = await File.ReadAllBytesAsync(imageFolderPath);

                // Add Image as Binary Body
                request.AddParameter("application/octet-stream", imageBytes, ParameterType.RequestBody);

                // Measure response time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //// Execute Request
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
                ExtentReportManager.GetInstance().LogError(test, Status.Fail, $"Error Message : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calculates the median value from a list of doubles.
        /// </summary>
        /// <param name="responseTimes">A list of doubles representing response times.</param>
        /// <returns>The median value of the response times. Returns 0 if the list is empty.</returns>
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
