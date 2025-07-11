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

        [Then("API returns device node data and AvalonStatus {string}")]
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
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "Response", $"{_detectBySerialNumberResponse?.ToString()}");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' and device node data.");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_detectBySerialNumberResponse?.ToString()}");
        }

        [When("Send a request to the DetectClosest API when one device is found")]
        public async Task WhenSendARequestToTheDetectClosestAPIWhenOneDeviceIsFoundAsync()
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

        [Then("API returns the correct device node data and AvalonStatus {string}")]
        public void ThenAPIReturnsTheCorrectDeviceNodeDataAndStatus(string expectedStatus)
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

        [When("Send a request to the FittingSide API to read the current fitting side from the device")]
        public async Task WhenSendARequestToTheFittingSideAPIToReadTheCurrentFittingSideFromTheDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
            var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Left side response: {left}");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Right side response: {right}");
            if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                connectedSide = ChannelSide.Both;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully.");
            }
            else if (left.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = left;
                connectedSide = ChannelSide.Left;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Left side detected successfully.");
            }
            else if (right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = right;
                connectedSide = ChannelSide.Right;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Right side detected successfully.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side found.");
                throw new InvalidOperationException("No connected side found.");
            }
        }

        [Then("API returns the fitting side of the connected device \\(Ex: Left or Right)")]
        public void ThenAPIReturnsTheFittingSideOfTheConnectedDeviceExLeftOrRight()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_detectOnSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectOnSide response is null");
                throw new Exception("DetectOnSide response is null");
            }

            var fittingSide = connectedSide.ToString();
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned fitting side: {fittingSide}");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_detectOnSideResponse}");
        }

        [When("Connect a supported device Send a request to the DeviceNodeData API")]
        public async Task WhenConnectASupportedDeviceSendARequestToTheDeviceNodeDataAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(false);
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                throw new Exception("GET request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            }
        }

        [Then("API returns complete device node data")]
        public void ThenAPIReturnsCompleteDeviceNodeData()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Response is null");
                throw new Exception("Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "API returned complete device node data.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_getDeviceNodeResponse.ToString()}");
            }
        }

        [When("Send a request to the ConnectToDevice API with valid and detected device node data")]
        public async Task WhenSendARequestToTheConnectToDeviceAPIWithValidAndDetectedDeviceNodeDataAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                throw new Exception("GET request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            }
        }

        [Then("API successfully connects to the device and returns status {string}")]
        public void ThenAPISuccessfullyConnectsToTheDeviceAndReturnsStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            var actualStatus = _connectResponse!.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "Response", $"{_detectClosestResponse?.ToString()}");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}'");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{_connectResponse?.ToString()}");
        }

        [When("Connect a supported device Send a request to the CheckBootMode API")]
        public async Task WhenConnectASupportedDeviceSendARequestToTheCheckBootModeAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _getBootModeResponse = await _hearingInstrumentPage.CallGetBootModeAsync();
            if (_getBootModeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                throw new Exception("GET request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            }
        }

        [Then("API returns the current boot mode of the connected device")]
        public void ThenAPIReturnsTheCurrentBootModeOfTheConnectedDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_getBootModeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Boot mode response is null.");
                throw new Exception("Boot mode response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "API successfully returned the current boot mode of the connected device.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Boot mode Response", $"{_getBootModeResponse?.ToString()}");
            }
        }

        [When("Send a request to the BootDevice API with any boot type\\(Ex: DspRunning, DfuMode, ServiceMode) and reconnect flag set to True")]
        public async Task WhenSendARequestToTheBootDeviceAPIWithAnyBootTypeExDspRunningDfuModeServiceModeAndReconnectFlagSetToTrueAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
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
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GET request failed: Response is null");
                throw new Exception("GET request failed: Response is null");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Sent GET request Successfully");
            }
        }

        [Then("Device boots and API reconnects to the device successfully.")]
        public void ThenDeviceBootsAndAPIReconnectsToTheDeviceSuccessfully_()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_response == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Boot mode response is null.");
                throw new Exception("Boot mode response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device booted successfully and API reconnected without errors.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Boot API Response", $"{_response?.ToString()}");
            }
        }
    }
}
