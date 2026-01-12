using AventStack.ExtentReports;
using NUnit.Framework;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using RestSharp;
using System;
using System.Net;

namespace QuantumServicesAPI.StepDefinitions.VivianeRecommendationServices
{
    [Binding]
    public class VivianeRecommendationServicesStepDefinitions
    {
        private readonly VivianeRecommendationServicePage _vivianeRecommendationServicePage;
        private RestResponse? _response;
        private readonly ScenarioContext _scenarioContext;
        public VivianeRecommendationServicesStepDefinitions(ScenarioContext scenarioContext)
        {
            _vivianeRecommendationServicePage = new VivianeRecommendationServicePage();
            _scenarioContext = scenarioContext;
        }

        [When("Send POST request to the {string} endpoint with valid headers and body {string} and {string} with endpoint PostCorrectiveAction")]
        public async Task WhenSendPOSTRequestToTheEndpointWithValidHeadersAndBodyAndWithEndpointPostCorrectiveActionAsync(string region, string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {          
                foreach (var row in dataTable.Rows)
                {
                    string vivianeRecommandationData = row["PostCorrectiveActionData"];
                    _response = await _vivianeRecommendationServicePage.PostCorrectiveAction(step, apiEndpoint, baseUrl, apiKey, vivianeRecommandationData);
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
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"{ex.Message}");
            }
        }

        [Then("verify the API response of the Viviane Recommendation Service")]
        public void ThenVerifyTheAPIResponseOfTheVivianeRecommendationService()
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
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Response content is null or empty");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"<span style='color:red;'>Error Message: {ex.Message}</span>");
            }
        }

        [When("Send POST request to the {string} endpoint with valid FDTS metadata and payload {string} and {string} with endpoint PostFdtsDataForRecommendation")]
        public async Task WhenSendPOSTRequestToTheEndpointWithValidFDTSMetadataAndPayloadAndWithEndpointPostFdtsDataForRecommendationAsync(string region, string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string vivianeRecommandationData = row["PostFdtsDataForRecommendationData"];
                    _response = await _vivianeRecommendationServicePage.PostFdtsDataForRecommendation(step, apiEndpoint, baseUrl, apiKey, vivianeRecommandationData);
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
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"{ex.Message}");
            }          
        }

        [When("Send POST request to the {string} endpoint with valid feedback {string} and {string} with endpoint PostRecommendationFeedback")]
        public async Task WhenSendPOSTRequestToTheEndpointWithValidFeedbackAndWithEndpointPostRecommendationFeedbackAsync(string region, string baseUrl, string apiKey, DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string vivianeRecommandationData = row["PostRecommendationFeedbackData"];
                    _response = await _vivianeRecommendationServicePage.PostRecommendationFeedback(step, apiEndpoint, baseUrl, apiKey, vivianeRecommandationData);
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
            catch (Exception ex) 
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"{ex.Message}");
            }
        }
    }
}
