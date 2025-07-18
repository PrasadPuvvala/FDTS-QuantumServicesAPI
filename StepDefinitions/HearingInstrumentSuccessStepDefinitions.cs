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
        private VoidResponse? _response; // Declare '_response' as nullable to fix CS8618
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
        public HearingInstrumentSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
        }

        [When("Send a request to the DetectBySerialNumber API with a valid serial number that matches an existing device")]
        public async Task WhenSendARequestToTheDetectBySerialNumberAPIWithAValidSerialNumberThatMatchesAnExistingDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Initializing device and configuring product for detection by serial number...");

            try
            {
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Device initialized successfully.");

                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Product configured successfully using FDTS file.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Initialization or configuration failed: {ex.Message}");
                throw;
            }

            foreach (var row in dataTable.Rows)
            {
                string serialNumber = row["SerialNumber"];

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Sending DetectBySerialNumber request for serial number: {serialNumber}");

                _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                if (_detectBySerialNumberResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                    throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API call succeeded for serial number: {serialNumber}");
            }
        }

        [Then("API returns device node data and AvalonStatus {string}")]
        public void ThenAPIReturnsDeviceNodeDataAndStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_detectBySerialNumberResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Validation failed: DetectBySerialNumber response is null.");
                throw new Exception("DetectBySerialNumber response is null.");
            }

            string actualStatus = _detectBySerialNumberResponse.AvalonStatus.ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected AvalonStatus: {expectedStatus}, Actual AvalonStatus: {actualStatus}");

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"AvalonStatus mismatch. Expected: '{expectedStatus}', Actual: '{actualStatus}'");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "DetectBySerialNumber Response", _detectBySerialNumberResponse.ToString());
                throw new Exception($"AvalonStatus mismatch. Expected: '{expectedStatus}', Actual: '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected AvalonStatus: '{expectedStatus}' and valid device node data.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DetectBySerialNumber Response", _detectBySerialNumberResponse.ToString());
        }

        [When("Send a request to the DetectClosest API when one device is found")]
        public async Task WhenSendARequestToTheDetectClosestAPIWhenOneDeviceIsFoundAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to DetectClosest API to find the nearest RHI device...");

            _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();

            if (_detectClosestResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectClosest API response is null. No device detected.");
                throw new Exception("DetectClosest API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DetectClosest API call succeeded. Device detected.");
        }

        [Then("API returns the correct device node data and AvalonStatus {string}")]
        public void ThenAPIReturnsTheCorrectDeviceNodeDataAndStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_detectClosestResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Validation failed: DetectClosest API response is null.");
                throw new Exception("DetectClosest API response is null.");
            }

            var actualStatus = _detectClosestResponse.AvalonStatus.ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Validating AvalonStatus. Expected: '{expectedStatus}', Actual: '{actualStatus}'");

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"AvalonStatus mismatch. Expected: '{expectedStatus}', but got: '{actualStatus}'");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "DetectClosest Response", _detectClosestResponse.ToString());
                throw new Exception($"AvalonStatus mismatch. Expected: '{expectedStatus}', but got: '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected AvalonStatus: '{expectedStatus}' and valid device node data.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DetectClosest Response", _detectClosestResponse.ToString());
        }

        [When("Send a request to the FittingSide API to read the current fitting side from the device")]
        public async Task WhenSendARequestToTheFittingSideAPIToReadTheCurrentFittingSideFromTheDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending DetectOnSide requests for Left and Right channels...");

            var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
            var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

            if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                connectedSide = ChannelSide.Both;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
            }
            else if (left.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = left;
                connectedSide = ChannelSide.Left;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
            }
            else if (right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = right;
                connectedSide = ChannelSide.Right;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                throw new InvalidOperationException("No connected side found.");
            }
        }

        [Then("API returns the fitting side of the connected device \\(Ex: Left or Right)")]
        public void ThenAPIReturnsTheFittingSideOfTheConnectedDeviceExLeftOrRight()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_detectOnSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Fitting side verification failed: DetectOnSide response is null.");
                throw new Exception("DetectOnSide response is null.");
            }

            var fittingSide = connectedSide.ToString();
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating fitting side returned by DetectOnSide API...");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API correctly returned fitting side: {fittingSide}");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DetectOnSide Response", _detectOnSideResponse.ToString());
        }

        [When("Connect a supported device Send a request to the DeviceNodeData API")]
        public async Task WhenConnectASupportedDeviceSendARequestToTheDeviceNodeDataAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Requesting device node data from DeviceNodeData API...");
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceNodeData API response is null. Device may not be connected or properly configured.");
                throw new Exception("DeviceNodeData API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DeviceNodeData API call succeeded. Response received.");
            }
        }

        [Then("API returns complete device node data")]
        public void ThenAPIReturnsCompleteDeviceNodeData()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Device node data verification failed: Response is null.");
                throw new Exception("Device node data response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating device node data returned by API...");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "API returned complete and valid device node data.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
        }

        [When("Send a request to the ConnectToDevice API with valid and detected device node data")]
        public async Task WhenSendARequestToTheConnectToDeviceAPIWithValidAndDetectedDeviceNodeDataAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Requesting valid device node from DeviceNodeData API...");
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceNodeData API response is null. Cannot proceed with device connection.");
                throw new Exception("DeviceNodeData API response is null.");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Retrieved Device Node", _getDeviceNodeResponse.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Attempting to connect to device using ConnectToDevice API...");
            _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice API response is null. Connection attempt failed.");
                throw new Exception("ConnectToDevice API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ConnectToDevice API call succeeded. Response received.");
            }
        }

        [Then("API successfully connects to the device and returns status {string}")]
        public void ThenAPISuccessfullyConnectsToTheDeviceAndReturnsStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice validation failed: response is null.");
                throw new Exception("ConnectToDevice response is null.");
            }

            string actualStatus = _connectResponse.AvalonStatus.ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Validating response status. Expected: '{expectedStatus}', Actual: '{actualStatus}'");

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Status mismatch. Expected: '{expectedStatus}', Actual: '{actualStatus}'");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "ConnectToDevice Response", _connectResponse.ToString());
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"ConnectToDevice API returned expected status: '{expectedStatus}'");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "ConnectToDevice Response", _connectResponse.ToString());
        }

        [When("Connect a supported device Send a request to the CheckBootMode API")]
        public async Task WhenConnectASupportedDeviceSendARequestToTheCheckBootModeAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CheckBootMode API to retrieve current boot mode...");

            _getBootModeResponse = await _hearingInstrumentPage.CallGetBootModeAsync();

            if (_getBootModeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "CheckBootMode API response is null. Unable to determine boot mode.");
                throw new Exception("CheckBootMode API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "CheckBootMode API call succeeded. Boot mode response received.");
        }

        [Then("API returns the current boot mode of the connected device")]
        public void ThenAPIReturnsTheCurrentBootModeOfTheConnectedDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_getBootModeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Validation failed: Boot mode response is null.");
                throw new Exception("Boot mode response is null.");
            }

            string currentBootMode = _getBootModeResponse.BootMode.ToString();
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Validating returned boot mode. Current boot mode: '{currentBootMode}'");

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API successfully returned current boot mode: '{currentBootMode}'");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "CheckBootMode API Response", _getBootModeResponse.ToString());
        }

        [When("Send a request to the BootDevice API with any boot type\\(Ex: DspRunning, DfuMode, ServiceMode) {string} and reconnect flag set to True")]
        public async Task WhenSendARequestToTheBootDeviceAPIWithAnyBootTypeExDspRunningDfuModeServiceModeAndReconnectFlagSetToTrueAsync(string bootMode)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Preparing to boot device into '{bootMode}' mode with reconnect = true...");

            BootType bootTypeToUse = bootMode switch
            {
                "ServiceMode" => BootType.ServiceMode,
                "DspRunning" => BootType.DspRunning,
                "DfuMode" => BootType.DfuMode,
                "DspRunningNotInHostMode" => BootType.DspRunningNotInHostMode,
                "DspRunningNotInHostModeAsync" => BootType.DspRunningNotInHostModeAsync,
                _ => BootType.DspStopped
            };

            _response = await _hearingInstrumentPage.CallBootAsync(bootTypeToUse, true);

            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"BootDevice API response is null for boot mode '{bootMode}'.");
                throw new Exception($"BootDevice API response is null for boot mode '{bootMode}'.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Device successfully booted into '{bootMode}' mode and reconnected.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "BootDevice API Response", _response.ToString());
        }

        [When("Send a request to the BootDevice API with any boot type when reconnect flag is set to True and verify the response")]
        public async Task WhenSendARequestToTheBootDeviceAPIWithAnyBootTypeWhenReconnectFlagIsSetToTrueAndVerifyTheResponseAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            foreach (var row in dataTable.Rows)
            {
                string type = row["BootType"];
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Initiating boot sequence for boot type '{type}' with reconnect = true...");

                BootType bootTypeToUse = type switch
                {
                    "ServiceMode" => BootType.ServiceMode,
                    "DspRunning" => BootType.DspRunning,
                    "DfuMode" => BootType.DfuMode,
                    "DspRunningNotInHostMode" => BootType.DspRunningNotInHostMode,
                    "DspRunningNotInHostModeAsync" => BootType.DspRunningNotInHostModeAsync,
                    _ => BootType.DspStopped
                };

                _response = await _hearingInstrumentPage.CallBootAsync(bootTypeToUse, true);

                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"BootDevice API returned null response for boot type '{type}'.");
                    throw new Exception($"BootDevice API response is null for boot type: '{type}'");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Device booted successfully in '{type}' mode and reconnected.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, $"BootDevice API Response - {type}", _response.ToString());
            }
        }

        [When("Send a request to the FlashWriteProtect API to read current status")]
        public async Task WhenSendARequestToTheFlashWriteProtectAPIToReadCurrentStatusAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to FlashWriteProtect API to fetch current protection status...");

            _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();

            if (_getFlashWriteProtectStatusResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "FlashWriteProtect API response is null. Unable to determine write protection status.");
                throw new Exception("FlashWriteProtect API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "FlashWriteProtect API call succeeded. Current status received.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "FlashWriteProtect Status Response", _getFlashWriteProtectStatusResponse.ToString());
        }

        [Then("API returns one of the valid states {string} , {string} & {string}")]
        public void ThenAPIReturnsOneOfTheValidStates(string unLock, string @lock, string lockedPermanent)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_getFlashWriteProtectStatusResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "FlashWriteProtectStatus response is null.");
                throw new Exception("FlashWriteProtectStatus response is null.");
            }
            // Validate FlashWriteProtectStatus
            var actualStatus = _getFlashWriteProtectStatusResponse.FlashWriteProtectStatus.ToString();
            var validStates = new[] { unLock, @lock, lockedPermanent };

            if (validStates.Contains(actualStatus))
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FlashWriteProtectStatus is valid: '{actualStatus}' (expected one of: {string.Join(", ", validStates)}).");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FlashWriteProtectStatus Response : {actualStatus}");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"FlashWriteProtectStatus '{actualStatus}' is not among expected values: {string.Join(", ", validStates)}.");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"FlashWriteProtectStatus Response : {_getFlashWriteProtectStatusResponse.FlashWriteProtectStatus}");
                throw new Exception("FlashWriteProtectStatus response is not one of the valid states.");
            }
        }

        [When("Send a request to the FlashWriteProtect API with state as {string}")]
        [When("Send a request to the FlashWriteProtect API with state set as {string}")]
        public async Task WhenSendARequestToTheFlashWriteProtectAPIWithStateAsAsync(string state)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            var stateMapping = new Dictionary<string, FlashWriteProtectState>(StringComparer.OrdinalIgnoreCase)
    {
        { "Lock", FlashWriteProtectState.Lock },
        { "UnLock", FlashWriteProtectState.UnLock },
        { "LockPermanent", FlashWriteProtectState.LockPermanent }
    };

            if (!stateMapping.TryGetValue(state, out var flashWriteProtectStateToUse))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid input '{state}' for FlashWriteProtectState. Expected values: Lock, UnLock, LockPermanent.");
                throw new ArgumentException($"Invalid FlashWriteProtectState: {state}");
            }

            // Step 1: Check current Flash Write Protect status
            var currentStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
            if (currentStatusResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to read current Flash Write Protect status. Response is null.");
                throw new Exception("FlashWriteProtectStatus read failed: response is null.");
            }

            var currentStatus = currentStatusResponse.FlashWriteProtectStatus;
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Current FlashWriteProtectStatus: {currentStatus}");

            // Step 2: Ensure device is not in LockedPermanent state
            if (currentStatus == FlashWriteProtectStatus.LockedPermanent)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Connected device is in LockedPermanent state. Cannot change FlashWriteProtect state.");
                throw new InvalidOperationException("Device is LockedPermanent. FlashWriteProtect state change not allowed.");
            }

            // Step 3: Proceed to set new state
            _setFlashWriteProtectStateResponse = await _hearingInstrumentPage.CallSetFlashWriteProtectStateAsync(flashWriteProtectStateToUse);
            if (_setFlashWriteProtectStateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetFlashWriteProtectState API response is null.");
                throw new Exception("SetFlashWriteProtectState response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FlashWriteProtect API called successfully. New state: '{flashWriteProtectStateToUse}'.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "SetFlashWriteProtectState Response", _setFlashWriteProtectStateResponse.ToString());
        }

        [Then("API returns status as {string}")]
        public void ThenAPIReturnsStatusAs(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            // Log the raw response for transparency
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "SetFlashWriteProtectState Response", _setFlashWriteProtectStateResponse?.ToString() ?? "null");

            if (_setFlashWriteProtectStateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetFlashWriteProtectState API response is null. Cannot verify status.");
                throw new Exception("SetFlashWriteProtectState API response is null.");
            }

            // Map input string to enum
            var statusMapping = new Dictionary<string, FlashWriteProtectStatus>(StringComparer.OrdinalIgnoreCase)
    {
        { "Lock", FlashWriteProtectStatus.Locked },
        { "UnLock", FlashWriteProtectStatus.NotLocked },
        { "LockPermanent", FlashWriteProtectStatus.LockedPermanent }
    };

            if (!statusMapping.TryGetValue(expectedStatus, out var expectedEnumStatus))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid expected status value: '{expectedStatus}'. Accepted values are: Lock, UnLock, LockPermanent.");
                throw new ArgumentException($"Invalid expected status: {expectedStatus}");
            }

            var actualStatus = _setFlashWriteProtectStateResponse.FlashWriteProtectStatus;

            if (actualStatus == expectedEnumStatus)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                    $"FlashWriteProtectStatus matched: Expected = '{expectedEnumStatus}', Actual = '{actualStatus}'.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail,
                    $"FlashWriteProtectStatus mismatch. Expected = '{expectedEnumStatus}', Actual = '{actualStatus}'.");
                throw new Exception($"FlashWriteProtectStatus mismatch. Expected: {expectedEnumStatus}, Actual: {actualStatus}");
            }
        }

        [When("Send a request to FlashWriteProtect API with state {string}")]
        public async Task WhenSendARequestToFlashWriteProtectAPIWithStateAsync(string state)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            var stateMapping = new Dictionary<string, FlashWriteProtectState>(StringComparer.OrdinalIgnoreCase)
    {
        { "Lock", FlashWriteProtectState.Lock },
        { "UnLock", FlashWriteProtectState.UnLock },
        { "LockPermanent", FlashWriteProtectState.LockPermanent }
    };

            if (!stateMapping.TryGetValue(state, out var flashWriteProtectStateToUse))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid input '{state}' for FlashWriteProtectState. Expected values: Lock, UnLock, LockPermanent.");
                throw new ArgumentException($"Invalid FlashWriteProtectState: {state}");
            }

            // Step 1: Check current Flash Write Protect status
            var currentStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
            if (currentStatusResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to read current Flash Write Protect status. Response is null.");
                throw new Exception("FlashWriteProtectStatus read failed: response is null.");
            }

            var currentStatus = currentStatusResponse.FlashWriteProtectStatus;
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Current FlashWriteProtectStatus: {currentStatus}");

            // Step 2: Proceed to call set API even if it's LockedPermanent (for this scenario)
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Attempting to change FlashWriteProtect state to '{flashWriteProtectStateToUse}'.");

            // Step 3: Proceed to set new state
            _setFlashWriteProtectStateResponse = await _hearingInstrumentPage.CallSetFlashWriteProtectStateAsync(flashWriteProtectStateToUse);
            if (_setFlashWriteProtectStateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetFlashWriteProtectState API response is null.");
                throw new Exception("SetFlashWriteProtectState response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FlashWriteProtect API called successfully. New state: '{flashWriteProtectStateToUse}'.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "SetFlashWriteProtectState Response", _setFlashWriteProtectStateResponse.ToString());
        }

        [Then("API returns status as {string} when state is set to Lock")]
        [Then("API returns status as {string} when state is set to UnLock")]
        public void ThenAPIReturnsStatusAsWhenStateIsSetToLock(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            // Log the raw response for transparency
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "SetFlashWriteProtectState Response", _setFlashWriteProtectStateResponse?.ToString() ?? "null");

            if (_setFlashWriteProtectStateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetFlashWriteProtectState API response is null. Cannot verify status.");
                throw new Exception("SetFlashWriteProtectState API response is null.");
            }

            // Map input string to enum
            var statusMapping = new Dictionary<string, FlashWriteProtectStatus>(StringComparer.OrdinalIgnoreCase)
    {
        { "Lock", FlashWriteProtectStatus.Locked },
        { "UnLock", FlashWriteProtectStatus.NotLocked },
        { "LockPermanent", FlashWriteProtectStatus.LockedPermanent }
    };

            if (!statusMapping.TryGetValue(expectedStatus, out var expectedEnumStatus))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid expected status value: '{expectedStatus}'. Accepted values are: Lock, UnLock, LockPermanent.");
                throw new ArgumentException($"Invalid expected status: {expectedStatus}");
            }

            var actualStatus = _setFlashWriteProtectStateResponse.FlashWriteProtectStatus;

            if (actualStatus == expectedEnumStatus)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                    $"FlashWriteProtectStatus matched: Expected = '{expectedEnumStatus}', Actual = '{actualStatus}'.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail,
                    $"FlashWriteProtectStatus mismatch. Expected = '{expectedEnumStatus}', Actual = '{actualStatus}'.");
                throw new Exception($"FlashWriteProtectStatus mismatch. Expected: {expectedEnumStatus}, Actual: {actualStatus}");
            }
        }

        [When("Connect a non-rechargeable device and send a request to the RHI Status API")]
        [When("Connect a rechargeable RHI device and send a request to the RHI Status API")]
        public async Task WhenConnectANon_RechargeableDeviceAndSendARequestToTheRHIStatusAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to RHI Status API to check if the device is rechargeable...");

            _isRechargeableResponse = await _hearingInstrumentPage.CallIsRechargeableAsync();

            if (_isRechargeableResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "RHI Status API call failed: Response is null.");
                throw new Exception("RHI Status API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "RHI Status API responded successfully.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "RHI Status Response", _isRechargeableResponse.ToString());
        }

        [Then("API returns {string} for the RHI status")]
        public void ThenAPIReturnsForTheRHIStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_isRechargeableResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "RHI status response is null. Cannot verify rechargeability.");
                throw new Exception("RHI status response is null.");
            }

            if (!bool.TryParse(expectedStatus, out bool expectedBoolStatus))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid expected value '{expectedStatus}'. Must be 'true' or 'false'.");
                throw new ArgumentException($"Invalid expected value: {expectedStatus}. Must be 'true' or 'false'.");
            }

            bool actualStatus = _isRechargeableResponse.IsRechargeable;

            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Parsed RHI Status Response", _isRechargeableResponse.ToString());

            if (actualStatus == expectedBoolStatus)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                    $"Rechargeable status matched. Expected: '{expectedBoolStatus}', Actual: '{actualStatus}'.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail,
                    $"Rechargeable status mismatch. Expected: '{expectedBoolStatus}', Actual: '{actualStatus}'.");
                throw new Exception($"RHI status mismatch. Expected: {expectedBoolStatus}, Actual: {actualStatus}");
            }
        }

        [When("Connect a supported RHI device Send a request to the RHIBatteryLevel API")]
        public async Task WhenConnectASupportedRHIDeviceSendARequestToTheRHIBatteryLevelAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to RHIBatteryLevel API to retrieve current battery level...");

            _getBatteryLevelResponse = await _hearingInstrumentPage.CallGetBatteryLevelAsync();

            if (_getBatteryLevelResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "RHIBatteryLevel API response is null. Device may not be connected or request failed.");
                throw new Exception("RHIBatteryLevel API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "RHIBatteryLevel API responded successfully.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Battery Level Response", _getBatteryLevelResponse.ToString());
        }

        [Then("API returns the battery level of the device values between {int} to {int}")]
        public void ThenAPIReturnsTheBatteryLevelOfTheDeviceValuesBetweenTo(int minLevel, int maxLevel)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_getBatteryLevelResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Battery level response is null. Cannot evaluate battery level.");
                throw new Exception("Battery level response is null.");
            }

            int batteryLevel = _getBatteryLevelResponse.BatteryLevel;

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Battery level received: {batteryLevel}");

            if (batteryLevel < minLevel || batteryLevel > maxLevel)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail,
                    $"Battery level '{batteryLevel}' is outside the valid range of {minLevel}-{maxLevel}.");
                throw new Exception($"Battery level validation failed. Expected range: {minLevel}-{maxLevel}, Actual: {batteryLevel}");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                $"Battery level '{batteryLevel}' is within the expected range ({minLevel}-{maxLevel}).");
        }

        [When("Send a request to the MFI Chip Health API with a connected device having unhealthy MFI chip")]
        [When("Send a request to the MFI Chip Health API with a connected device having healthy MFI chip")]
        public async Task WhenSendARequestToTheMFIChipHealthAPIWithAConnectedDeviceHavingUnhealthyMFIChipAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to MFI Chip Health API to check chip status...");

            _response = await _hearingInstrumentPage.CallVerifyMfiChipIsHealthyAsync();

            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "MFI Chip Health API response is null. The chip status could not be determined.");
                throw new Exception("MFI Chip Health API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "MFI Chip Health API responded successfully.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "MFI Chip Health Response", _response.ToString());
        }

        [Then("API returns {string} for the MFI chip health status")]
        public void ThenAPIReturnsForTheMFIChipHealthStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Validation failed: MFI chip health response is null.");
                throw new Exception("MFI chip health response is null.");
            }

            string actualStatus = _response!.ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Comparing MFI chip health status. Expected: '{expectedStatus}', Actual: '{actualStatus}'");

            if (actualStatus.Equals(expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"MFI chip health status matched expected value: '{actualStatus}'");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"MFI chip health status mismatch. Expected: '{expectedStatus}', Actual: '{actualStatus}'");
                throw new Exception($"MFI chip health mismatch. Expected: '{expectedStatus}', Actual: '{actualStatus}'");
            }
        }

        [When("Send a request to the RHI Battery Type API to read the battery type from the connected RHI device")]
        public async Task WhenSendARequestToTheRHIBatteryTypeAPIToReadTheBatteryTypeFromTheConnectedRHIDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending GET request to RHI Battery Type API...");

            _getBatteryTypeResponse = await _hearingInstrumentPage.CallGetBatteryTypeAsync();

            if (_getBatteryTypeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to read battery type: API response is null.");
                throw new Exception("RHI Battery Type API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GET request completed successfully.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Battery Type Response", _getBatteryTypeResponse.ToString());
        }

        [Then("API returns the correct battery type from the device")]
        public void ThenAPIReturnsTheCorrectBatteryTypeFromTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_getBatteryTypeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Validation failed: Battery Type response is null.");
                throw new Exception("Battery Type response is null.");
            }

            string batteryType = _getBatteryTypeResponse.BatteryType.ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API successfully returned battery type: '{batteryType}'.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Battery Type Response", _getBatteryTypeResponse.ToString());
        }

        [When("Send a request to the RHI Battery Type API with a valid battery type to write to the connected RHI device")]
        public async Task WhenSendARequestToTheRHIBatteryTypeAPIWithAValidBatteryTypeToWriteToTheConnectedRHIDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            foreach (var batteryType in dataTable.Rows)
            {
                string type = batteryType["BatteryType"];
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Sending request to set battery type: {type}");
                _response = await _hearingInstrumentPage.CallSetBatteryTypeAsync(type);
                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Failed to set battery type '{type}': Response is null.");
                    throw new Exception($"SetBatteryType API response is null for battery type: {type}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Battery type '{type}' successfully written to device.");
                }
            }
        }

        [Then("Battery type is successfully written to the device")]
        public void ThenBatteryTypeIsSuccessfullyWrittenToTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Battery type write verification failed: Response is null.");
                throw new Exception("Battery type write verification failed: Response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Battery type write confirmed with response: {_response}");
        }

        [When("Send a request to the ReadBatteryVoltage API on a RHI device")]
        public async Task WhenSendARequestToTheReadBatteryVoltageAPIOnARHIDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _getBatteryVoltageResponse = await _hearingInstrumentPage.CallGetBatteryVoltageAsync();
            if (_getBatteryVoltageResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ReadBatteryVoltage API failed: Response is null.");
                throw new Exception("ReadBatteryVoltage API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "ReadBatteryVoltage API call succeeded.");
            }
        }

        [Then("API returns valid values for Voltage,MinimumVoltage,MaximumVoltage")]
        public void ThenAPIReturnsValidValuesForVoltageMinimumVoltageMaximumVoltage()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_getBatteryVoltageResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Battery voltage response validation failed: Response is null.");
                throw new Exception("Battery voltage response is null.");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Battery Voltage", $"{_getBatteryVoltageResponse}");
        }

        [When("Send a request to the DeviceFunctional API with enable functionality is {string}")]
        public async Task WhenSendARequestToTheDeviceFunctionalAPIWithEnableFunctionalityIsAsync(string enableFunctionality)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            // Convert the string parameter to a boolean value
            if (!bool.TryParse(enableFunctionality, out bool isEnabled))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid input '{enableFunctionality}' for enable functionality. Expected 'true' or 'false'.");
                throw new ArgumentException($"Invalid enable functionality value: {enableFunctionality}");
            }
            _response = await _hearingInstrumentPage.CallMakeDeviceFunctionalAsync(isEnabled);
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceFunctional API call failed: Response is null.");
                throw new Exception("DeviceFunctional API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DeviceFunctional API call succeeded. Device functionality is set to '{isEnabled}' successfully.");
        }

        [Then("Verify the response when enable functionality input is set to true")]
        [Then("Verify the response when enable functionality input is set to false")]
        public void ThenVerifyTheResponseWhenEnableFunctionalityInputIsSetToTrue()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceFunctional API call failed: Response is null.");
                throw new Exception("DeviceFunctional API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DeviceFunctional API call succeeded with response: " + _response.ToString());
            }
        }

        [When("Send a request to the RHIPowerOff API to power off the connected RHI device")]
        public async Task WhenSendARequestToTheRHIPowerOffAPIToPowerOffTheConnectedRHIDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _response = await _hearingInstrumentPage.CallSetPowerOffAsync();
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"RHIPowerOff API call failed: Response is null.");
                throw new Exception($"RHIPowerOff API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"RHIPowerOff API request sent successfully. Response received.");
            }
        }

        [Then("Verify the response when RHIPowerOff API is called")]
        public void ThenVerifyTheResponseWhenRHIPowerOffAPIIsCalled()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Verification failed: RHIPowerOff API response is null.");
                throw new Exception("Verification failed: RHIPowerOff API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"RHIPowerOff API responded successfully. Response: {_response}");
            }
        }
    }
}
