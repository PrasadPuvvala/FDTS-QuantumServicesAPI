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
        public async Task WhenSendARequestToMFGDataServiceWithACompressedJSONBelowKbUsingBaseUrlAndApiKeyAsync(int p0, string baseUrl, string apiKey, DataTable dataTable)
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
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
                }
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
        public async Task WhenSendARequestToTheMFGDataServiceUsingAnInvalidAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl, DataTable dataTable)
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
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
                }
            }
        }

        [When("Send a request to the MFG Data Service without API key {string} with baseUrl {string}")]
        public async Task WhenSendARequestToTheMFGDataServiceWithoutAPIKeyWithBaseUrlAsync(string apiKey, string baseUrl, DataTable dataTable)
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
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Response Body: {_response.Content}");
                }
            }
        }

        [Then("Verify the response with the compressed data")]
        public void ThenVerifyTheResponseWithTheCompressedData()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                var responseContent = _response.Content;
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
                var responseContent = _response.Content;
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");

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
                var responseContent = _response.Content;
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Response Body: {responseContent}");

            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(step, Status.Fail, $"Error Message : {ex.Message}");
            }
        }
    }
}
