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
    [Binding]
    public class ProcessControlServiceStepDefinitions
    {
        private readonly ProcessControlServicePage _processControlServicePage;
        private RestResponse? _response;
        private readonly ScenarioContext _scenarioContext;
        public ProcessControlServiceStepDefinitions(ScenarioContext scenarioContext)
        {
            _processControlServicePage = new ProcessControlServicePage();
            _scenarioContext = scenarioContext;
        }

        [When("Send a request to the Process Control Service using a valid APIkey {string} and baseUrl {string} for its respective cloud region")]
        public async Task WhenSendARequestToTheProcessControlServiceUsingAValidAPIkeyAndBaseUrlForItsRespectiveCloudRegionAsync(string apiKey, string baseUrl)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _response = await _processControlServicePage.PostEventData(step, apiEndpoint, baseUrl, apiKey);
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

        [Then("Verify the API response for each cloud region")]
        public void ThenVerifyTheAPIResponseForEachCloudRegion()
        {
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                Assert.NotNull(_response, "Response should not be null");
                Assert.AreEqual(HttpStatusCode.OK, _response?.StatusCode, "Expected 200 OK status code");
                ExtentReportManager.GetInstance().LogStatusCode(step, Status.Pass, $"Statuscode : {_response?.StatusCode}");
                ExtentReportManager.GetInstance().LogJson(step, Status.Pass, "Response Body", _response.Content);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"<span style='color:red;'>Error Message: {ex.Message}</span>");
            }
        }

    }
}
