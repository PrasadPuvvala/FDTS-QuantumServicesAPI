using System;
using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using RestSharp;

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
        [When("Send a request to the Process Control Service using a valid API key for its respective cloud region")]
        public async Task WhenSendARequestToTheProcessControlServiceUsingAValidAPIKeyForItsRespectiveCloudRegionAsync(DataTable dataTable)
        {
            var apiEndpoint = _scenarioContext.Get<APIEndpointsDTO>("apiendpoints");
            var test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            var step = ExtentReportManager.GetInstance().CreateTestStep(test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string apikey = row["APIkey"];
                string env = row["Env"];
                string region = row["Region"];
                _response = await _processControlServicePage.PostEventData(step, apiEndpoint, apikey, env, region);
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
    }
}
