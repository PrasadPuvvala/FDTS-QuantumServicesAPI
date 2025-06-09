using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    /// <summary>
    /// Provides methods for interacting with the MFG Data Service API endpoints.
    /// </summary>
    public class MFGDataServicePage
    {
        private readonly APIHelperClass _APIHelper;
        private static List<double> _responseTimes = new List<double>();  // Stores response times

        /// <summary>
        /// Initializes a new instance of the <see cref="MFGDataServicePage"/> class.
        /// </summary>
        public MFGDataServicePage()
        {
            _APIHelper = new APIHelperClass();
        }

        /// <summary>
        /// Posts MFG data to the specified API endpoint.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint information.</param>
        /// <param name="mfgDataFile">The name of the MFG data file.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>The RestResponse from the API, or null if an error occurred.</returns>
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

            if (apiEndpointsDTO.apiEndpoint == null || string.IsNullOrEmpty(apiEndpointsDTO.apiEndpoint.storeTestDataRoute))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, "API endpoint or storeTestDataRoute is null or empty.");
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

        /// <summary>
        /// Posts a request containing an MFG data file (JSON) compressed as a ZIP archive to the specified API endpoint.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint information.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>The RestResponse from the API, or null if an error occurred.</returns>
        public async Task<RestResponse?> PostRequestMFGDataFile(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRootDirectory = Directory.GetParent(baseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string mfgDataFilesDirectory = Path.Combine(projectRootDirectory, "MFGDataFiles");

            // Find the first JSON file in the folder
            var firstJsonFilePath = Directory.GetFiles(mfgDataFilesDirectory, "*.json").FirstOrDefault();

            if (string.IsNullOrEmpty(firstJsonFilePath) || !File.Exists(firstJsonFilePath))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, $"File not found: {mfgDataFilesDirectory}");
                return null;
            }

            // Generate ZIP file name based on JSON name + current timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string jsonFileName = Path.GetFileName(firstJsonFilePath);
            string jsonFileNameWithoutExtension = Path.GetFileNameWithoutExtension(firstJsonFilePath);
            string zipFileName = $"{jsonFileNameWithoutExtension}{timestamp}.zip";
            string zipFilePath = Path.Combine(mfgDataFilesDirectory, zipFileName);

            // Create ZIP file and add the JSON file
            using (FileStream zipFileStream = new FileStream(zipFilePath, FileMode.Create))
            using (ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
            {
                var zipEntry = zipArchive.CreateEntry($"{jsonFileNameWithoutExtension}{timestamp}.json");
                using (var zipEntryStream = zipEntry.Open())
                using (var originalJsonFileStream = File.OpenRead(firstJsonFilePath))
                {
                    originalJsonFileStream.CopyTo(zipEntryStream);
                }
            }

            if (apiEndpointsDTO.apiEndpoint == null || string.IsNullOrEmpty(apiEndpointsDTO.apiEndpoint.storeTestDataRoute))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, "API endpoint or storeTestDataRoute is null or empty.");
                return null;
            }

            var client = await _APIHelper.MFGDataUrl(baseUrl, apiEndpointsDTO.apiEndpoint.storeTestDataRoute, zipFileName);
            var request = await _APIHelper.CreatePostRequest(apikey);
            try
            {
                // Read ZIP as Byte Array
                byte[] zipFileBytes = await File.ReadAllBytesAsync(zipFilePath);

                // Add ZIP as Binary Body
                request.AddParameter("application/octet-stream", zipFileBytes, ParameterType.RequestBody);

                // Execute Request
                var response = await client.ExecuteAsync(request);
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
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
        /// Posts a request containing an MFG data file (JSON) compressed as a ZIP archive to the specified API endpoint and measures the response time.
        /// </summary>
        /// <param name="test">The ExtentTest instance for reporting.</param>
        /// <param name="apiEndpointsDTO">The DTO containing API endpoint information.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apikey">The API key for authentication.</param>
        /// <returns>The RestResponse from the API, or null if an error occurred.</returns>
        public async Task<RestResponse?> PostRequestMFGDataFileResponseTime(ExtentTest test, APIEndpointsDTO apiEndpointsDTO, string baseUrl, string apikey)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRootDirectory = Directory.GetParent(baseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string mfgDataFilesDirectory = Path.Combine(projectRootDirectory, "MFGDataFiles");

            // Find the first JSON file in the folder
            var firstJsonFilePath = Directory.GetFiles(mfgDataFilesDirectory, "*.json").FirstOrDefault();

            if (string.IsNullOrEmpty(firstJsonFilePath) || !File.Exists(firstJsonFilePath))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, $"File not found: {mfgDataFilesDirectory}");
                return null;
            }

            // Generate ZIP file name based on JSON name + current timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string jsonFileName = Path.GetFileName(firstJsonFilePath);
            string jsonFileNameWithoutExtension = Path.GetFileNameWithoutExtension(firstJsonFilePath);
            string zipFileName = $"{jsonFileNameWithoutExtension}{timestamp}.zip";
            string zipFilePath = Path.Combine(mfgDataFilesDirectory, zipFileName);

            // Create ZIP file and add the JSON file
            using (FileStream zipFileStream = new FileStream(zipFilePath, FileMode.Create))
            using (ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
            {
                var zipEntry = zipArchive.CreateEntry($"{jsonFileNameWithoutExtension}{timestamp}.json");
                using (var zipEntryStream = zipEntry.Open())
                using (var originalJsonFileStream = File.OpenRead(firstJsonFilePath))
                {
                    originalJsonFileStream.CopyTo(zipEntryStream);
                }
            }

            if (apiEndpointsDTO.apiEndpoint == null || string.IsNullOrEmpty(apiEndpointsDTO.apiEndpoint.storeTestDataRoute))
            {
                ExtentReportManager.GetInstance().LogToReport(test, Status.Fail, "API endpoint or storeTestDataRoute is null or empty.");
                return null;
            }

            var client = await _APIHelper.MFGDataUrl(baseUrl, apiEndpointsDTO.apiEndpoint.storeTestDataRoute, zipFileName);
            var request = await _APIHelper.CreatePostRequest(apikey);
            try
            {
                // Read ZIP as Byte Array
                byte[] zipFileBytes = await File.ReadAllBytesAsync(zipFilePath);

                // Add ZIP as Binary Body
                request.AddParameter("application/octet-stream", zipFileBytes, ParameterType.RequestBody);

                // Measure response time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Execute Request
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

                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
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
