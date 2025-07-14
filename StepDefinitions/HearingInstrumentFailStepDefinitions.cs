using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class HearingInstrumentFailStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private VoidResponse? _response; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        public HearingInstrumentFailStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
        }
        [When("Send a request to the DetectBySerialNumber API with a valid serial number that does not match any device.")]
        public async Task WhenSendARequestToTheDetectBySerialNumberAPIWithAValidSerialNumberThatDoesNotMatchAnyDevice_Async(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _response = await _hearingInstrumentPage.CallInitializeAsync();
            _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
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

        [Then("API returns null for device node data and status {string}")]
        public void ThenAPIReturnsNullForDeviceNodeDataAndStatus(string expectedStatus)
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
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "Response", $"{_detectBySerialNumberResponse?.ToString()}");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' and device node data.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_detectBySerialNumberResponse?.ToString()}");
        }

        [When("Send a request to the DetectClosest API when no devices are nearby")]
        public async Task WhenSendARequestToTheDetectClosestAPIWhenNoDevicesAreNearbyAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
            if (_detectClosestResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                throw new Exception("GET request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            }
        }

        [Then("DetectClosest API returns null for device node data and status {string}")]
        public void ThenDetectClosestAPIReturnsNullForDeviceNodeDataAndStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_detectClosestResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Response is null");
                throw new Exception("Response is null");
            }

            // Validate status
            var actualStatus = _detectClosestResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"Response : {_detectClosestResponse.AvalonStatus.ToString()}");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' and device node data.");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Response : {_detectClosestResponse.AvalonStatus.ToString()}");
        }
    }
}
