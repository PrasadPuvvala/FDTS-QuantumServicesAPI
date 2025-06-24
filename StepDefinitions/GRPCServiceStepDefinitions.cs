using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using Grpc.Net.Client;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class GRPCServiceStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ScenarioContext _scenarioContext;
        private VoidResponse? _response; // Declare '_response' as nullable to fix CS8618
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618    

        public GRPCServiceStepDefinitions(ScenarioContext scenarioContext) // Fix IDE0290 by using primary constructor
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
        }

        [When("I call the Initialize method on the GRPC service")]
        public async Task WhenICallTheInitializeMethodOnTheGRPCServiceAsync()
        {
            _response = await _hearingInstrumentPage.CallInitializeAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the Initialize method on the GRPC service")]
        public void ThenVerifyTheResponseForTheInitializeMethodOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_response?.ToString()}"); // Use null-conditional operator to handle nullable '_response'
        }

        [When("I call the ConfigureProduct on the GRPC service with folder path {string}")]
        public async Task WhenICallTheConfigureProductOnTheGRPCServiceWithFolderPath(string folderpath)
        {
            _response = await _hearingInstrumentPage.CallConfigureProductAsync(folderpath);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the ConfigureProduct on the GRPC service")]
        public void ThenVerifyTheResponseForTheConfigureProductOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_response?.ToString()}");
        }

        [When("I call the DetectBySerialNumber on the GRPC service with serialnumber {string}")]
        public async Task WhenICallTheDetectBySerialNumberOnTheGRPCServiceWithSerialnumber(string serialnumber)
        {
            _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialnumber);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the DetectBySerialNumber on the GRPC service")]
        public void ThenVerifyTheResponseForTheDetectBySerialNumberOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_detectBySerialNumberResponse?.ToString()}");
        }

        [When("I call the DetectClosest on the GRPC service")]
        public async Task WhenICallTheDetectClosestOnTheGRPCService()
        {
            _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the DetectClosest on the GRPC service")]
        public void ThenVerifyTheResponseForTheDetectClosestOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_detectClosestResponse?.AvalonStatus.ToString()}");
        }

        [When("I call the DetectOnSide on the GRPC service")]
        public async Task WhenICallTheDetectOnSideOnTheGRPCService()
        {
            var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
            var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);
            if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                connectedSide = ChannelSide.Both;
            }
            else if (left.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = left;
                connectedSide = ChannelSide.Left;
            }
            else if (right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = right;
                connectedSide = ChannelSide.Right;
            }
            else
            {
                throw new InvalidOperationException("No connected side found.");
            }
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the DetectOnSide on the GRPC service")]
        public void ThenVerifyTheResponseForTheDetectOnSideOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{connectedSide}");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_detectOnSideResponse?.AvalonStatus.ToString()}");
        }
        [When("I call the EnableMasterConnect on the GRPC service")]
        public async Task WhenICallTheEnableMasterConnectOnTheGRPCService()
        {
            _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the EnableMasterConnect on the GRPC service")]
        public void ThenVerifyTheResponseForTheEnableMasterConnectOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_enableMasterConnectResponse?.ToString()}");
        }

        [When("I call the EnableFittingMode on the GRPC service")]
        public async Task WhenICallTheEnableFittingModeOnTheGRPCService()
        {
            _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the EnableFittingMode on the GRPC service")]
        public void ThenVerifyTheResponseForTheEnableFittingModeOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_enableFittingModeResponse?.ToString()}");
        }

        [When("I call the GetDeviceNode on the GRPC service")]
        public async Task WhenICallTheGetDeviceNodeOnTheGRPCService()
        {
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetDeviceNode on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetDeviceNodeOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass,"Device Node Information" ,$"{_getDeviceNodeResponse?.ToString()}");
        }
    }
}
