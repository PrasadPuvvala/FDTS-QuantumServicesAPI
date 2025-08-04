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
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private DetectOnSideResponse? _oppositeSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse;
        
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

        [When("Send a request to the DetectWired API with a valid monoaural side \\(e.g., {string}) when no device is connected.")]
        public async Task WhenSendARequestToTheDetectWiredAPIWithAValidMonoauralSideE_G_WhenNoDeviceIsConnected_Async(string p0, DataTable dataTable)
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
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending DetectOnSide requests for Left and Right channels...");

            var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
            var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

            if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                connectedSide = ChannelSide.Both;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, "Both sides detected successfully. Using 'Both' as fitting side.");
                throw new InvalidOperationException("No connected side found.");
            }
            else if (left.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = left;
                connectedSide = ChannelSide.Left;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, "Only Left side detected successfully.");
                throw new InvalidOperationException("No connected side found.");
            }
            else if (right.AvalonStatus == AvalonStatus.Success)
            {
                _detectOnSideResponse = right;
                connectedSide = ChannelSide.Right;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, "Only Right side detected successfully.");
                throw new InvalidOperationException("No connected side found.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Pass, "No connected side detected. Device may not be connected or powered.");
            }

            // Dynamically call the OPPOSITE side
            ChannelSide oppositeSide = connectedSide switch
            {
                ChannelSide.Left => ChannelSide.Right,
                ChannelSide.Right => ChannelSide.Left,
                _ => ChannelSide.Left // default if unknown
            };

            _oppositeSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(oppositeSide);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info,$"Sent DetectOnSide request\n" + $"Connected Side: {connectedSide}\n" + $"Sent Side: {oppositeSide}\n" + $"Response: {_oppositeSideResponse.AvalonStatus}");
        }


        [Then("API returns null for device node data and status for DetectWired {string}")]
        public void ThenAPIReturnsNullForDeviceNodeDataAndStatusForDetectWired(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_oppositeSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectOnSide (opposite side) response is null.");
                throw new Exception("DetectOnSide (opposite side) response is null.");
            }

            // Validate AvalonStatus only
            var actualStatus = _oppositeSideResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, _oppositeSideResponse.ToString());
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            // Passed
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}'.");
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, _oppositeSideResponse.ToString());
        }



        [When("Ensure that no device is connected Send a request to the DeviceNodeData API")]
        public async Task WhenEnsureThatNoDeviceIsConnectedSendARequestToTheDeviceNodeDataAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Requesting device node data from DeviceNodeData API...");
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
        }

        [Then("API returns null for device node data")]
        public void ThenAPIReturnsNullForDeviceNodeData()
        {
            //throw new PendingStepException();
        }

        [When("Simulate where a valid device node is detected but the connection to the device and Send a request to the ConnectToDevice API")]
        public async Task WhenSimulateWhereAValidDeviceNodeIsDetectedButTheConnectionToTheDeviceAndSendARequestToTheConnectToDeviceAPIAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            // Enable master connect and fitting mode
            await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            await _hearingInstrumentPage.CallEnableFittingModeAsync(true);

            // Get device node
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();

            if (_getDeviceNodeResponse == null || _getDeviceNodeResponse.DeviceNode == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "❌ DeviceNode is null. Cannot proceed to Connect.");
                throw new Exception("DeviceNode is null from GetDeviceNode.");
            }

            var deviceNode = _getDeviceNodeResponse.DeviceNode;

            // Attempt to connect
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Attempting to connect to detected device...");
            _connectResponse = await _hearingInstrumentPage.CallConnectAsync(deviceNode);
        }


        [Then("API returns status {string}")]
        public void ThenAPIReturnsStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice response is null.");
                throw new Exception("ConnectToDevice response is null.");
            }

            var actualStatus = _connectResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status: '{expectedStatus}', but got: '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"✅ API returned expected status: **{expectedStatus}**");
        }

        [When("Select a device that has been powered on for more than {int} minutes, then attempt to connect using the ConnectToDevice API")]
        public async Task WhenSelectADeviceThatHasBeenPoweredOnForMoreThanMinutesThenAttemptToConnectUsingTheConnectToDeviceAPI(int minutes)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            // Enable master connect and fitting mode
            await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            await _hearingInstrumentPage.CallEnableFittingModeAsync(true);

            // Get the device node (we are not checking PoweredOnMinutes)
            _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            var deviceNode = _getDeviceNodeResponse.DeviceNode;

            // Connect to the device
            _connectResponse = await _hearingInstrumentPage.CallConnectAsync(deviceNode);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "✅ ConnectToDevice API called successfully.");
        }

        [Then("API returns a status {string}")]
        public void ThenAPIReturnsAStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "❌ ConnectToDevice response is null.");
                throw new Exception("ConnectToDevice response is null.");
            }

            var actualStatus = _connectResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"❌ Expected status: '{expectedStatus}', but got: '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"✅ API returned expected status: **{expectedStatus}**");
        }



        [When("Send a request to the ConnectToDevice API with a valid node data where the device model not listed in the product configuration database")]
        public async Task WhenSendARequestToTheConnectToDeviceAPIWithAValidNodeDataWhereTheDeviceModelNotListedInTheProductConfigurationDatabaseAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            var serialNumber = dataTable.Rows[0]["SerialNumber"].ToString();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Preparing DeviceNode with SerialNumber: {serialNumber}");

            var deviceNode = new DeviceNode
            {
                SerialNumber = serialNumber
            };

            try
            {
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(deviceNode);

                if (_connectResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice API response is null.");
                    throw new Exception("ConnectToDevice API response is null.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ConnectToDevice API request succeeded.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "ConnectToDevice Response", _connectResponse.ToString());
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the status {string}")]
        public void ThenAPIReturnsTheStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Response is null.");
                throw new Exception("ConnectToDevice API response is null.");
            }

            string actualStatus = _connectResponse.AvalonStatus.ToString();

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status: '{expectedStatus}', but got: '{actualStatus}'");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Fail, "Response", _connectResponse.ToString());
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"ConnectToDevice API returned expected status: '{expectedStatus}'");
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", _connectResponse.ToString());
        }


        [When("Send a request to the BootDevice API with any boot type when reconnect flag is set to False and verify the response")]
        public async Task WhenSendARequestToTheBootDeviceAPIWithAnyBootTypeWhenReconnectFlagIsSetToFalseAndVerifyTheResponseAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            foreach (var row in dataTable.Rows)
            {
                string type = row["BootType"];
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Initiating boot sequence for boot type '{type}' with reconnect = false...");

                BootType bootTypeToUse = type switch
                {
                    "ServiceMode" => BootType.ServiceMode,
                    "DspRunning" => BootType.DspRunning,
                    "DfuMode" => BootType.DfuMode,
                    "DspRunningNotInHostMode" => BootType.DspRunningNotInHostMode,
                    "DspRunningNotInHostModeAsync" => BootType.DspRunningNotInHostModeAsync,
                    _ => BootType.DspStopped
                };

                _response = await _hearingInstrumentPage.CallBootAsync(bootTypeToUse, false);

                if (_response == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"BootDevice API returned null response for boot type '{type}'.");
                    throw new Exception($"BootDevice API response is null for boot type: '{type}'");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Device booted successfully in '{type}' mode and reconnected.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, $"BootDevice API Response - {type}", _response.ToString());
            }
        }


    }
}
