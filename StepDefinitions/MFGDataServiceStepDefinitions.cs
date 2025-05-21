using System;
using System.Net;
using AventStack.ExtentReports;
using NUnit.Framework;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using RestSharp;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class MFGDataServiceStepDefinitions
    {
        private readonly MFGDataServicePage _mfgDataServicePage;
        private RestResponse? _response;
        private readonly ScenarioContext _scenarioContext;
        public MFGDataServiceStepDefinitions(ScenarioContext scenarioContext)
        {
            _mfgDataServicePage = new MFGDataServicePage();
            _scenarioContext = scenarioContext;
        }

        [When("Send a request to MFG data service with a compressed JSON below {int} kb using baseUrl {string} and apiKey {string}")]
        public async Task WhenSendARequestToMFGDataServiceWithACompressedJSONBelowKbUsingBaseUrlAndApiKeyAsync(int p0, string baseUrl, string apiKey)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        [When("Send a request to MFG data service with an uncompressed JSON using baseUrl {string} and apiKey {string}")]
        public async Task WhenSendARequestToMFGDataServiceWithAnUncompressedJSONUsingBaseUrlAndApiKeyAsync(string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string mfgDataFile = row["MFGDataFile"];
                _response = await _mfgDataServicePage.PostMFGData(step, apiEndpoint, mfgDataFile, baseUrl, apiKey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [When("Send a request to the MFG Data Service using an invalid API key {string} with baseUrl {string}")]
        public async Task WhenSendARequestToTheMFGDataServiceUsingAnInvalidAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKey = "123456789"; // Invalid API key
            }

            _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        [When("Send a request to the MFG Data Service without API key {string} with baseUrl {string}")]
        public async Task WhenSendARequestToTheMFGDataServiceWithoutAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKey = ""; // Invalid API key
            }
            _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        [Then("Verify the response with the compressed data")]
        [Then("Verify the response")]
        public void ThenVerifyTheResponseWithTheCompressedData()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("Verify the response with the uncompressed data")]
        public void ThenVerifyTheResponseWithTheUncompressedData()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.BadRequest, _response?.StatusCode, "Expected 400 BadRequest status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("Verify the response with the compressed JSON below {int} kb")]
        public void ThenVerifyTheResponseWithTheCompressedJSONBelowKb(int p0)
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("Verify the response with the compressed JSON more {int} kb")]
        public void ThenVerifyTheResponseWithTheCompressedJSONMoreKb(int p0)
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.BadRequest, _response?.StatusCode, "Expected 400 BadRequest status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [Then("Verify the API response")]
        public void ThenVerifyTheAPIResponse()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.Unauthorized, _response?.StatusCode, "Expected 401 Unauthorized status code");
                var responseContent = _response?.Content; // Safely access Content using null conditional operator
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }

        [When("Send a request to MFG data service with an unsupported format using baseUrl {string} and apiKey {string} and verify the response with the unsupported format")]
        public async Task WhenSendARequestToMFGDataServiceWithAnUnsupportedFormatUsingBaseUrlAndApiKeyAndVerifyTheResponseWithTheUnsupportedFormatAsync(string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string mfgDataFile = row["MFGDataFile"];
                _response = await _mfgDataServicePage.PostMFGData(step, apiEndpoint, mfgDataFile, baseUrl, apiKey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
                try
                {
                    Assert.NotNull(_response, "Response should not be null");
                    Assert.AreEqual(HttpStatusCode.BadRequest, _response?.StatusCode, "Expected 400 BadRequest status code");
                    var responseContent = _response?.Content; // Safely access Content using null conditional operator
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");
                }
                catch (Exception ex)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
                }
            }
        }
        [When("Send a request to MFG data service with a compressed JSON more {int} kb using baseUrl {string} and apiKey {string}")]
        public async Task WhenSendARequestToMFGDataServiceWithACompressedJSONMoreKbUsingBaseUrlAndApiKeyAsync(int p0, string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string mfgDataFile = row["MFGDataFile"];
                _response = await _mfgDataServicePage.PostMFGData(step, apiEndpoint, mfgDataFile, baseUrl, apiKey);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                    throw new Exception("POST request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                }
            }
        }

        [When("Send a request to MFG data service using baseUrl {string} and a valid API Key {string}")]
        public async Task WhenSendARequestToMFGDataServiceUsingBaseUrlAndAValidAPIKeyAsync(string baseUrl, string apiKey)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        [When("Send a request to the MFG Data Service {string} Environment and {string} Region using baseUrl {string} and apiKey {string}")]
        [When("Send a request to the MFG Data Service {string} Environment and {string} cloud Region using baseUrl {string} and apiKey {string}")]
        public async Task WhenSendARequestToTheMFGDataServiceEnvironmentAndRegionUsingBaseUrlAndApiKeyAsync(string environment, string region, string baseUrl, string apiKey)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        [When("Send a request to the MFG Data Service {string} Environment and {string} cloud Region using baseUrl {string} and apiKey {string} from a different cloud region and verify the response")]
        public async Task WhenSendARequestToTheMFGDataServiceEnvironmentAndCloudRegionUsingBaseUrlAndApiKeyFromADifferentCloudRegionAndVerifyTheResponseAsync(string environment, string region, string baseUrl, string apiKey)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var mfgDataServiceAPIkeyConfig = _scenarioContext.Get<MFGDataServiceAPIKeysDTO>("mfgdataserviceapikeys");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (apiEndpoint?.apiEndpoint == null || string.IsNullOrEmpty(apiEndpoint.apiEndpoint.storeTestDataRoute))
                FailStep(step, "API endpoint or storeTestDataRoute is null or empty.");

            if (mfgDataServiceAPIkeyConfig?.mfgDataServiceapiKeys == null)
                FailStep(step, "MFGDataServiceAPIKeys configuration is null.");

            var allKeys = mfgDataServiceAPIkeyConfig?.mfgDataServiceapiKeys;
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

                _response = await _mfgDataServicePage.PostRequestMFGDataFile(step, apiEndpoint, baseUrl, apiKeyToUse);

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
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Status Code: {_response.StatusCode}");
                }
                catch (Exception ex)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message: {ex.Message}");
                }
            }
        }

        [When("Send a request to the MFG data service under normal system load using baseUrl {string} and apiKey {string} and verify the median response time")]
        public async Task WhenSendARequestToTheMFGDataServiceUnderNormalSystemLoadUsingBaseUrlAndApiKeyAndVerifyTheMedianResponseTimeAsync(string baseUrl, string apiKey)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            _response = await _mfgDataServicePage.PostRequestMFGDataFileResponseTime(step, apiEndpoint, baseUrl, apiKey);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, "POST request failed: Response is null");
                throw new Exception("POST request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Sent POST request Successfully");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
            }
        }

        // Helper methods
        private static string Capitalize(string input) =>
            char.ToUpperInvariant(input[0]) + input.Substring(1).ToLowerInvariant();

        private void FailStep(ExtentTest step, string message)
        {
            ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, message);
            throw new Exception(message);
        }
    }
}
