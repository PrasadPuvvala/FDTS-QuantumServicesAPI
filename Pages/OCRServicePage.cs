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
    public class OCRServicePage
    {
        private readonly OCRAPIHelperClass _APIHelper;
        private static List<double> _responseTimes = new List<double>();  // Stores response times
        public OCRServicePage()
        {
            _APIHelper = new OCRAPIHelperClass();
        }
        public async Task<RestResponse?> PostImageAnalyzeAsync(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string image, string env, string region, string apikey)
        {
            var client = await _APIHelper.SetUrl(env, region, apiEndpointsDTO.apiEndpoint.AnalyzeImage);
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
        public async Task<RestResponse?> PostImageMedianResponseTime(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string image, string env, string region, string apikey)
        {
            var client = await _APIHelper.SetUrl(env, region, apiEndpointsDTO.apiEndpoint.AnalyzeImage);
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
                    ExtentReportManager.GetInstance().LogWithColor(test, Status.Pass, $"Median response time is below 3 seconds and actual response time is : {medianResponseTime} milli scenods", ExtentColor.Green);
                }
                else
                {
                    ExtentReportManager.GetInstance().LogWithColor(test, Status.Info, $"Median response time is not below 3 seconds and actual response time is : {medianResponseTime} milli scenods", ExtentColor.Transparent);
                }
                return response;
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogWithColor(test, Status.Fail, $"{ex.Message}", ExtentColor.Red);
                return null;
            }
        }
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
