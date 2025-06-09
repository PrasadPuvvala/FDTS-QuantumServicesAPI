using AventStack.ExtentReports;
using NUnit.Framework;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using RestSharp;
using System;
using System.Net;

namespace QuantumServicesAPI.StepDefinitions
{
     /// <summary>
    /// Step definitions for Process Control Service API BDD scenarios.
    /// Handles sending requests to the Process Control Service and verifying responses
    /// for various authentication and metadata validation scenarios.
    /// </summary>
    [Binding]
    public class ProcessControlServiceStepDefinitions
    {
        private readonly ProcessControlServicePage _processControlServicePage;
        private RestResponse? _response;
        private readonly ScenarioContext _scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessControlServiceStepDefinitions"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context for sharing data between steps.</param>
        public ProcessControlServiceStepDefinitions(ScenarioContext scenarioContext)
        {
            _processControlServicePage = new ProcessControlServicePage();
            _scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Sends a POST request to the Process Control Service using the provided API key and base URL.
        /// Handles various scenarios including missing/invalid metadata fields.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the Process Control Service using a valid APIkey {string} and baseUrl {string} for its respective cloud region")]
        [When("Send a request to the API with missing required metadata fileds \\(e.g., without EventDateTime, EventName, Status, SiteId, or WorkstationId) using a valid APIkey {string} and baseUrl {string} for its respective cloud region")]
        [When("Send a request with a Invalid metadata fields such as Incorrect TimestampUtc\\/Invalid Status\\/Non-unique SiteId or WorkstationId using a valid APIkey {string} and baseUrl {string} for its respective cloud region")]
        [When("Send a request to the API with only the required metadata fields, excluding all optional fields \\(Description, OrderNumber, ProductName, ProcessName, ProcessStepName, Tags, Data) using a valid APIkey {string} and baseUrl {string} for its respective cloud region")]
        [When("Send a request to the Process Control Service using a valid APIkey {string} and baseUrl {string} for its respective cloud region \\(Ex: EastUS, WestEurope, SouthEastAsia)")]
        public async Task WhenSendARequestToTheProcessControlServiceUsingAValidAPIkeyAndBaseUrlForItsRespectiveCloudRegionAsync(string apiKey, string baseUrl, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                _response = await _processControlServicePage.PostEventData(step, apiEndpoint, baseUrl, apiKey, eventMetaData);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        /// <summary>
        /// Verifies that the API response is successful (HTTP 200 OK) and logs the response content.
        /// </summary>
        [Then("Verify the API response for each cloud region")]
        [Then("Verify the API response excludes metadata fields for each cloud region")]
        [Then("Verify the API response for specified environment")]
        public void ThenVerifyTheAPIResponseForEachCloudRegion()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");

                // Fix for CS8602 and CS8604: Ensure _response.Content is not null before passing it
                if (!string.IsNullOrEmpty(_response?.Content))
                {
                    ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Response Body", _response.Content);
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "Response content is null or empty");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"<span style='color:red;'>Error Message: {ex.Message}</span>");
            }
        }

        /// <summary>
        /// Verifies that the API response is a Bad Request (HTTP 400) for missing or invalid metadata.
        /// </summary>
        [Then("Verify the API response is missing metadata for each cloud region")]
        [Then("Verify the API response is invalid metadata for each cloud region")]
        public void ThenVerifyTheAPIResponseIsMissingMetadataForEachCloudRegion()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.BadRequest, _response?.StatusCode, "Expected 400 Bad Request status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");

                // Fix for CS8602 and CS8604: Ensure _response.Content is not null before passing it
                if (!string.IsNullOrEmpty(_response?.Content))
                {
                    ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Response Body", _response.Content);
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "Response content is null or empty");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"<span style='color:red;'>Error Message: {ex.Message}</span>");
            }
        }

        /// <summary>
        /// Sends a POST request to the Process Control Service for a specific environment and region.
        /// </summary>
        /// <param name="environment">The environment (e.g., Dev, QA, Prod).</param>
        /// <param name="region">The cloud region.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the Process Control Service {string} Environment and {string} Region using a valid APIkey {string} and baseUrl {string}")]
        public async Task WhenSendARequestToTheProcessControlServiceEnvironmentAndRegionUsingAValidAPIkeyAndBaseUrlAsync(string environment, string region, string apiKey, string baseUrl, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                _response = await _processControlServicePage.PostEventData(step, apiEndpoint, baseUrl, apiKey, eventMetaData);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        /// <summary>
        /// Sends POST requests to the Process Control Service using API keys from different cloud regions
        /// and verifies that unauthorized responses are returned.
        /// </summary>
        /// <param name="environment">The environment (e.g., Dev, QA, Prod).</param>
        /// <param name="region">The current cloud region.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apiKey">The API key (not used, replaced by region-specific keys).</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the Process Control Service {string} Environment and {string} cloud Region using baseUrl {string} and apiKey {string} from a different cloud region and verify the response")]
        public async Task WhenSendARequestToTheProcessControlServiceEnvironmentAndCloudRegionUsingBaseUrlAndApiKeyFromADifferentCloudRegionAndVerifyTheResponseAsync(string environment, string region, string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var processControlServiceAPIkeyConfig = _scenarioContext.Get<ProcessControlServiceAPIKeysDTO>("processcontrolserviceapikeys");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                if (apiEndpoint?.apiEndpoint == null || string.IsNullOrEmpty(apiEndpoint.apiEndpoint.storeTestDataRoute))
                    FailStep(step, "API endpoint or storeTestDataRoute is null or empty.");

                if (processControlServiceAPIkeyConfig?.processControlServiceAPIKeys == null)
                    FailStep(step, "MFGDataServiceAPIKeys configuration is null.");

                var allKeys = processControlServiceAPIkeyConfig?.processControlServiceAPIKeys;
                if (allKeys == null)
                {
                    FailStep(step, "MFGDataServiceAPIKeys configuration is null.");
                    return;
                }
                // Define other regions to test (excluding the current one)
                var regionsToTest = new List<string> { "Europe", "Us", "Asia" }
                    .Where(r => !r.Equals(region, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var otherRegion in regionsToTest)
                {
                    string keyPropName = $"{Capitalize(environment)}{Capitalize(otherRegion)}";

                    var apiKeyToUse = allKeys.GetType()
                        .GetProperty(keyPropName)?
                        .GetValue(allKeys, null)?
                        .ToString();

                    if (string.IsNullOrEmpty(apiKeyToUse))
                    {
                        ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"API key for {keyPropName} is missing.");
                        continue;
                    }

                    _response = await _processControlServicePage.PostEventData(step, apiEndpoint!, baseUrl, apiKeyToUse, eventMetaData);

                    if (_response == null)
                    {
                        ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"POST request failed using {keyPropName}: Response is null");
                        continue;
                    }

                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Sent POST request using {keyPropName} API Key");
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");

                    try
                    {
                        Assert.NotNull(_response, "Response should not be null");
                        Assert.AreEqual(HttpStatusCode.Unauthorized, _response?.StatusCode, "Expected 401 Unauthorized status code");
                        ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Status Code: {_response?.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Sends a POST request to the Process Control Service using an invalid API key.
        /// </summary>
        /// <param name="apiKey">The (to be replaced) API key.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the Process Control Service using an invalid API key {string} with baseUrl {string}")]
        public async Task WhenSendARequestToTheProcessControlServiceUsingAnInvalidAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKey = "123456789"; // Invalid API key
            }
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                _response = await _processControlServicePage.PostEventData(step, apiEndpoint, baseUrl, apiKey, eventMetaData);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        /// <summary>
        /// Verifies that the API response is Unauthorized (HTTP 401) for invalid or missing API key.
        /// </summary>
        [Then("Verify the API response using invalid API key")]
        [Then("Verify the API response using without API key")]
        public void ThenVerifyTheAPIResponseUsingInvalidAPIKey()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.Unauthorized, _response?.StatusCode, "Expected 401 Unauthorized status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a POST request to the Process Control Service without an API key.
        /// </summary>
        /// <param name="apiKey">The (to be cleared) API key.</param>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the Process Control Service without API key {string} with baseUrl {string}")]
        public async Task WhenSendARequestToTheProcessControlServiceWithoutAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKey = ""; // Invalid API key
            }
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                _response = await _processControlServicePage.PostEventData(step, apiEndpoint, baseUrl, apiKey, eventMetaData);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        /// <summary>
        /// Sends a POST request to the Process Control Service under normal system load
        /// and verifies the median response time.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="dataTable">Table containing event metadata.</param>
        [When("Send a request to the process control service under normal system load using baseUrl {string} and apiKey {string} and verify the median response time")]
        public async Task WhenSendARequestToTheProcessControlServiceUnderNormalSystemLoadUsingBaseUrlAndApiKeyAndVerifyTheMedianResponseTimeAsync(string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string eventMetaData = row["MetaData"];
                _response = await _processControlServicePage.PostEventDataResponseTime(step, apiEndpoint, baseUrl, apiKey, eventMetaData);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        /// <summary>
        /// Capitalizes the first character of the input string and lowercases the rest.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The capitalized string.</returns>
        private static string Capitalize(string input) =>
           char.ToUpperInvariant(input[0]) + input.Substring(1).ToLowerInvariant();

        /// <summary>
        /// Logs a failure message to the report and throws an exception to fail the step.
        /// </summary>
        /// <param name="step">The ExtentTest step instance.</param>
        /// <param name="message">The failure message.</param>
        private void FailStep(ExtentTest step, string message)
        {
            ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, message);
            throw new Exception(message);
        }
    }
}
