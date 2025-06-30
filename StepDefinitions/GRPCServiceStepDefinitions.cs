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
        private ConnectResponse? _connectResponse;
        private GetBootModeResponse? _getBootModeResponse;
        private GetFlashWriteProtectStatusResponse? _getFlashWriteProtectStatusResponse; // Declare '_getFlashWriteProtectStatusResponse' as nullable to fix CS8618
        private SetFlashWriteProtectStateResponse? _setFlashWriteProtectStateResponse; // Declare 'setFlashWriteProtectStateResponse' as global
        private IsRechargeableResponse? _isRechargeableResponse; // Declare '_isRechargeableResponse' as nullable to fix CS8618
        private GetBatteryLevelResponse? _getBatteryLevelResponse; // Declare '_getBatteryLevelResponse' as nullable to fix CS8618
        private ShouldVerifyMfiChipResponse? _shouldVerifyMfiChipResponse;
        private GetBatteryTypeResponse? _getBatteryTypeResponse;
        private GetBatteryVoltageResponse? _getBatteryVoltageResponse;

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
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_getDeviceNodeResponse?.ToString()}");
        }

        [When("I call the Connect on the GRPC service")]
        public async Task WhenICallTheConnectOnTheGRPCService()
        {
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the Connect on the GRPC service")]
        public void ThenVerifyTheResponseForTheConnectOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_connectResponse?.ToString()}");
        }

        [When("I call the GetBootMode on the GRPC service")]
        public async Task WhenICallTheGetBootModeOnTheGRPCService()
        {
            _getBootModeResponse = await _hearingInstrumentPage.CallGetBootModeAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetBootMode on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetBootModeOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_getBootModeResponse?.ToString()}");
        }

        [When("I call the Boot on the GRPC service")]
        public async Task WhenICallTheBootOnTheGRPCServiceAsync()
        {
            _getBootModeResponse = await _hearingInstrumentPage.CallGetBootModeAsync();
            var bootMode = _getBootModeResponse.BootMode;                    // Get enum value
            BootType bootTypeToUse = bootMode switch                      // Map BootMode → BootType
            {
                BootMode.Dfu => BootType.DfuMode,
                BootMode.Fitting => BootType.DspRunning,
                BootMode.Service => BootType.ServiceMode,
                _ => BootType.DspStopped
            };
            _response = await _hearingInstrumentPage.CallBootAsync(bootTypeToUse, true);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Mapped BootType : {bootTypeToUse} from BootMode : {bootMode}");
        }

        [Then("Verify the response for the Boot on the GRPC service")]
        public void ThenVerifyTheResponseForTheBootOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_response}");
        }

        [When("I call the GetFlashWriteProtectStatus on the GRPC service")]
        public async Task WhenICallTheGetFlashWriteProtectStatusOnTheGRPCService()
        {
            _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetFlashWriteProtectStatus on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetFlashWriteProtectStatusOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Flash Write Protect Status received: {_getFlashWriteProtectStatusResponse?.FlashWriteProtectStatus} => ({(int)_getFlashWriteProtectStatusResponse!.FlashWriteProtectStatus})");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_getFlashWriteProtectStatusResponse?.ToString()}");
        }

        [When("I call the SetFlashWriteProtectState on the GRPC service")]
        public async Task WhenICallTheSetFlashWriteProtectStateOnTheGRPCServiceAsync()
        {
            _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            var flashWriteProtectStatus = _getFlashWriteProtectStatusResponse!.FlashWriteProtectStatus;

            // Map FlashWriteProtectStatus to FlashWriteProtectState (if needed for your logic)
            FlashWriteProtectState flashWriteProtectStateToUse = flashWriteProtectStatus switch
            {
                FlashWriteProtectStatus.LockedPermanent => FlashWriteProtectState.LockPermanent,
                FlashWriteProtectStatus.Locked => FlashWriteProtectState.Lock,
                FlashWriteProtectStatus.NotLocked => FlashWriteProtectState.UnLock,
                _ => FlashWriteProtectState.UnLock
            };
            _setFlashWriteProtectStateResponse = await _hearingInstrumentPage.CallSetFlashWriteProtectStateAsync(flashWriteProtectStateToUse);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the SetFlashWriteProtectState on the GRPC service")]
        public void ThenVerifyTheResponseForTheSetFlashWriteProtectStateOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_setFlashWriteProtectStateResponse}");
        }

        [When("I call the IsRechargeable on the GRPC service")]
        public async Task WhenICallTheIsRechargeableOnTheGRPCService()
        {
            _isRechargeableResponse = await _hearingInstrumentPage.CallIsRechargeableAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the IsRechargeable on the GRPC service")]
        public void ThenVerifyTheResponseForTheIsRechargeableOnTheGRPCService()
        {
            bool isRechargeable = _isRechargeableResponse!.IsRechargeable;
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{isRechargeable}");
        }

        [When("I call the GetBatteryLevel on the GRPC service")]
        public async Task WhenICallTheGetBatteryLevelOnTheGRPCService()
        {
            _getBatteryLevelResponse = await _hearingInstrumentPage.CallGetBatteryLevelAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetBatteryLevel on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetBatteryLevelOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_getBatteryLevelResponse!.BatteryLevel}");
        }

        [When("I call the ShouldVerifyMfiChip on the GRPC service")]
        public async Task WhenICallTheShouldVerifyMfiChipOnTheGRPCService()
        {
            _shouldVerifyMfiChipResponse = await _hearingInstrumentPage.CallShouldVerifyMfiChipAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the ShouldVerifyMfiChip on the GRPC service")]
        public void ThenVerifyTheResponseForTheShouldVerifyMfiChipOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_shouldVerifyMfiChipResponse!.IsVerifyMfiChip}");
        }

        [When("I call the VerifyMfiChipIsHealthy on the GRPC service")]
        public async Task WhenICallTheVerifyMfiChipIsHealthyOnTheGRPCService()
        {
            _response = await _hearingInstrumentPage.CallVerifyMfiChipIsHealthyAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the VerifyMfiChipIsHealthy on the GRPC service")]
        public void ThenVerifyTheResponseForTheVerifyMfiChipIsHealthyOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Response TYpe : {_response!.GetType().Name} , Value : {_response}");
        }

        [When("I call the GetBatteryType on the GRPC service")]
        public async Task WhenICallTheGetBatteryTypeOnTheGRPCService()
        {
            _getBatteryTypeResponse = await _hearingInstrumentPage.CallGetBatteryTypeAsync();
            _getBatteryVoltageResponse = await _hearingInstrumentPage.CallGetBatteryVoltageAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetBatteryType on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetBatteryTypeOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Battery Type : {_getBatteryTypeResponse!.BatteryType}");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Battery Voltage", $"{_getBatteryVoltageResponse!.ToString()}");
        }

        [When("I call the SetBatteryType on the GRPC service")]
        public async Task WhenICallTheSetBatteryTypeOnTheGRPCServiceAsync()
        {
            _getBatteryTypeResponse = await _hearingInstrumentPage.CallGetBatteryTypeAsync();
            _response = await _hearingInstrumentPage.CallSetBatteryTypeAsync(_getBatteryTypeResponse.BatteryType);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the SetBatteryType on the GRPC service")]
        public void ThenVerifyTheResponseForTheSetBatteryTypeOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_response!.ToString()}");
        }

        [When("I call the GetBatteryVoltage on the GRPC service")]
        public async Task WhenICallTheGetBatteryVoltageOnTheGRPCService()
        {
            _getBatteryVoltageResponse = await _hearingInstrumentPage.CallGetBatteryVoltageAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the GetBatteryVoltage on the GRPC service")]
        public void ThenVerifyTheResponseForTheGetBatteryVoltageOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Battery Voltage", $"{_getBatteryVoltageResponse!.ToString()}");
        }

        [When("I call the MakeDeviceFunctional on the GRPC service")]
        public async Task WhenICallTheMakeDeviceFunctionalOnTheGRPCService()
        {
            _response = await _hearingInstrumentPage.CallMakeDeviceFunctionalAsync(true);
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the MakeDeviceFunctional on the GRPC service")]
        public void ThenVerifyTheResponseForTheMakeDeviceFunctionalOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_response!.ToString()}");
        }

        [When("I call the SetPowerOff on the GRPC service")]
        public async Task WhenICallTheSetPowerOffOnTheGRPCService()
        {
            _response = await _hearingInstrumentPage.CallSetPowerOffAsync();
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
        }

        [Then("Verify the response for the SetPowerOff on the GRPC service")]
        public void ThenVerifyTheResponseForTheSetPowerOffOnTheGRPCService()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_response!.ToString()}");
        }
    }
}
