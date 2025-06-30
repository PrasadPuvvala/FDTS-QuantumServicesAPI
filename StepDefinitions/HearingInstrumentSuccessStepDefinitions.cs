using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Buffers.Text;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class HearingInstrumentSuccessStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        public HearingInstrumentSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
        }

        [When("Send a request to the DetectBySerialNumber API with a valid serial number that matches an existing device")]
        public async Task WhenSendARequestToTheDetectBySerialNumberAPIWithAValidSerialNumberThatMatchesAnExistingDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var row in dataTable.Rows)
            {
                string serialnumber = row["SerialNumber"];
                _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialnumber);
                if (_detectBySerialNumberResponse == null) 
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                    throw new Exception("GET request failed: Response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
                }               
            }
        }

        [Then("API returns device node data and status {string}")]
        public void ThenAPIReturnsDeviceNodeDataAndStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_detectBySerialNumberResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Response is null");
                throw new Exception("Response is null");
            }

            // Validate status
            var actualStatus = _detectBySerialNumberResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' and device node data.");
        }
    }
}
