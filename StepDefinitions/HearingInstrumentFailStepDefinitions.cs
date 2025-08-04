using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using Newtonsoft.Json;
using QuantumServicesAPI.APIHelper;
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
                SocketHelperClass.HandleProcessExit();
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

        [When("Send a request to the DetectClosest API when no devices are nearby.")]
        public async Task WhenSendARequestToTheDetectClosestAPIWhenNoDevicesAreNearby_Async(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                SocketHelperClass.HandleProcessExit();
                SocketHelperClass.FailureSocketCommands();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DetectClosest API to find nearby devices...");
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                if (_detectClosestResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectClosest API returned null response.");
                    throw new Exception("DetectClosest response is null");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "DetectClosest API succeeded.");
                }
            }
            catch(Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
                throw;
            }
            SocketHelperClass.HandleProcessExit();

        }

        [Then("API returns null for device node data and status of DetectClosest as {string}")]
        public void ThenAPIReturnsNullForDeviceNodeDataAndStatusOfDetectClosestAs(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_detectClosestResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectClosest response is null");
                throw new Exception("DetectClosest response is null");
            }
            // Validate status
            var actualStatus = _detectClosestResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' and device node data.");
        }

        [When("Send a request to the DetectWired API with a valid monoaural side \\(e.g., {string}) when no device is connected")]
        public async Task WhenSendARequestToTheDetectWiredAPIWithAValidMonoauralSideE_G_WhenNoDeviceIsConnectedAsync(string p0, DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for DetectWired API.");
            
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DetectClosest API succeeded.");

                SocketHelperClass.HandleProcessExit();

                SocketHelperClass.FailureSocketCommands();

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DetectWired API to detect monoaural side...");

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

            // Dynamically call the OPPOSITE side
            ChannelSide oppositeSide = connectedSide switch
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Info, $"An error occurred during device initialization or product configuration: {ex.Message}");
                throw;
            }
            SocketHelperClass.HandleProcessExit();
        }

        [Then("API returns null for device node data and status for DetectWired {string}")]
        public void ThenAPIReturnsNullForDeviceNodeDataAndStatusForDetectWired(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());

            if (_detectOnSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DetectOnSide response is null");
                throw new Exception("DetectOnSide response is null");
            }
            // Validate status
            var actualStatus = _detectOnSideResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "DetectOnSide Response", _detectOnSideResponse.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{actualStatus}'");
        }

        [When("Ensure that no device is connected and Send a request to the DeviceNodeData API")]
        public async Task WhenEnsureThatNoDeviceIsConnectedAndSendARequestToTheDeviceNodeDataAPIAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for DeviceNodeData API.");
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully"); 

                //var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                //var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                //ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
                //ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

                //if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                //{
                //    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                //    connectedSide = ChannelSide.Both;
                //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                //}
                //else if (left.AvalonStatus == AvalonStatus.Success)
                //{
                //    _detectOnSideResponse = left;
                //    connectedSide = ChannelSide.Left;
                //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
                //}
                //else if (right.AvalonStatus == AvalonStatus.Success)
                //{
                //    _detectOnSideResponse = right;
                //    connectedSide = ChannelSide.Right;
                //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
                //}
                //else
                //{
                //    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                //    throw new InvalidOperationException("No connected side found.");
                //}
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                SocketHelperClass.HandleProcessExit();
                SocketHelperClass.FailureSocketCommands();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DeviceNodeData API to retrieve device node data...");
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                if (_getDeviceNodeResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetDeviceNode API returned null response.");
                    throw new Exception("GetDeviceNode response is null");
                }
               ExtentReportManager.GetInstance().LogToReport(_step,Status.Info, "Sent request to the DeviceNodeData API");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
                throw;
            }
            SocketHelperClass.HandleProcessExit();
        }

        [Then("API returns null for device node data")]
        public void ThenAPIReturnsNullForDeviceNodeData()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_getDeviceNodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetDeviceNode response is null");
                throw new Exception("GetDeviceNode response is null");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
        }

        [When("Simulate where a valid device node is detected but the connection to the device and Send a request to the ConnectToDevice API")]
        public async Task WhenSimulateWhereAValidDeviceNodeIsDetectedButTheConnectionToTheDeviceAndSendARequestToTheConnectToDeviceAPIAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

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
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);

                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                SocketHelperClass.HandleProcessExit();
                SocketHelperClass.FailureSocketCommands();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConnectToDevice API to connect to the device...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);

                if (_connectResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice API returned null response.");
                    throw new Exception("ConnectToDevice response is null");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "ConnectToDevice API called successfully.");

            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
                throw;
            }
            SocketHelperClass.HandleProcessExit();
        }

        [Then("API returns status {string}")]
        public void ThenAPIReturnsStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice response is null");
                throw new Exception("ConnectToDevice response is null");
            }
            // Validate status
            var actualStatus = _connectResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Conect Response", _connectResponse.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}'.");
        }

        [When("Send a request to the ConnectToDevice API with a valid node data where the device model not listed in the product configuration database")]
        public async Task WhenSendARequestToTheConnectToDeviceAPIWithAValidNodeDataWhereTheDeviceModelNotListedInTheProductConfigurationDatabaseAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

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
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(false);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(false);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                SocketHelperClass.HandleProcessExit();
                SocketHelperClass.FailureSocketCommands();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConnectToDevice API to connect to the device with invalid model...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                if (_connectResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice API returned null response.");
                    throw new Exception("ConnectToDevice response is null");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "ConnectToDevice API called successfully with invalid model.");
                SocketHelperClass.HandleProcessExit();
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
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
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice response is null");
                throw new Exception("ConnectToDevice response is null");
            }
            // Validate status
            var actualStatus = _connectResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Connect Response", _connectResponse.ToString());
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Connect Response", _connectResponse.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{expectedStatus}' for ConnectToDevice API with invalid model.");
        }


        [When("Select a device that has been powered on for more than {int} minutes, then attempt to connect using the ConnectToDevice API")]
        public async Task WhenSelectADeviceThatHasBeenPoweredOnForMoreThanMinutesThenAttemptToConnectUsingTheConnectToDeviceAPIAsync(int p0, DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _response = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                    _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                    if (_detectBySerialNumberResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                        throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
                }
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

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
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                SocketHelperClass.HandleProcessExit();
                SocketHelperClass.FailureSocketCommands();
                Thread.Sleep(240000);
                // Simulate a device that has been powered on for more than the specified minutes
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConnectToDevice API to connect to the device...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                if (_connectResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice API returned null response.");
                    throw new Exception("ConnectToDevice response is null");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "ConnectToDevice API called successfully.");
                SocketHelperClass.HandleProcessExit();
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
                throw;
            }
        }

        [Then("API returns status for ConnectToDevice {string}")]
        public void ThenAPIReturnsStatusForConnectToDevice(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            if (_connectResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ConnectToDevice response is null");
                throw new Exception("ConnectToDevice response is null");
            }
            // Validate status
            var actualStatus = _connectResponse.AvalonStatus.ToString();
            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Connect Response", _connectResponse.ToString());
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status '{expectedStatus}', but got '{actualStatus}'");
                throw new Exception($"Expected status '{expectedStatus}', but got '{actualStatus}'");
            }
            ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Connect Response", _connectResponse.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status '{actualStatus}' for ConnectToDevice API with powered on device.");
        }

        //[When("Send a request to the BootDevice API with any boot type\\(Ex: DspRunning, DfuMode, ServiceMode) and reconnect flag set to False")]
        //public async Task WhenSendARequestToTheBootDeviceAPIWithAnyBootTypeExDspRunningDfuModeServiceModeAndReconnectFlagSetToFalseAsync(DataTable dataTable)
        //{
        //    _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
        //    _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
        //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");

        //    // Ensure any previous sockets are handled
        //    SocketHelperClass.HandleProcessExit();
        //    SocketHelperClass.SuccessSocketCommands();

        //    try
        //    {
        //        // Initialize the device
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
        //        _response = await _hearingInstrumentPage.CallInitializeAsync();
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

        //        // Configure product
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
        //        _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");

        //        // Reuse serial number for rows with blank entries
        //        string lastSerialNumber = string.Empty;
        //        foreach (var row in dataTable.Rows)
        //        {
        //            string serialNumber = row["SerialNumber"];
        //            if (!string.IsNullOrEmpty(serialNumber))
        //            {
        //                lastSerialNumber = serialNumber;

        //                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
        //                _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

        //                if (_detectBySerialNumberResponse == null)
        //                {
        //                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
        //                    throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
        //                }

        //                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
        //            }
        //        }

        //        // Detect closest and connected side
        //        _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

        //        var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
        //        var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

        //        ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
        //        ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

        //        if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
        //        {
        //            _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
        //            connectedSide = ChannelSide.Both;
        //            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
        //        }
        //        else if (left.AvalonStatus == AvalonStatus.Success)
        //        {
        //            _detectOnSideResponse = left;
        //            connectedSide = ChannelSide.Left;
        //            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
        //        }
        //        else if (right.AvalonStatus == AvalonStatus.Success)
        //        {
        //            _detectOnSideResponse = right;
        //            connectedSide = ChannelSide.Right;
        //            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
        //        }
        //        else
        //        {
        //            ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
        //            throw new InvalidOperationException("No connected side found.");
        //        }

        //        // Enable MasterConnect and FittingMode
        //        _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
        //        _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);

        //        // Get Device Node and Connect
        //        _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");

        //        _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called ConnectToDevice successfully");

        //        // Simulate BootDevice API calls with reconnect = false
        //        //SocketHelperClass.HandleProcessExit();
        //        //SocketHelperClass.FailureSocketCommands();

        //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling BootDevice API to boot the device with specified boot type and reconnect flag set to false...");
        //        foreach (var row in dataTable.Rows)
        //        {
        //            string type = row["BootType"];
        //            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Initiating boot sequence for boot type '{type}' with reconnect = false...");

        //            BootType bootTypeToUse = type switch
        //            {
        //                "ServiceMode" => BootType.ServiceMode,
        //                "DspRunning" => BootType.DspRunning,
        //                "DfuMode" => BootType.DfuMode,
        //                "DspRunningNotInHostMode" => BootType.DspRunningNotInHostMode,
        //                "DspRunningNotInHostModeAsync" => BootType.DspRunningNotInHostModeAsync,
        //                _ => BootType.DspStopped
        //            };

        //            // ✅ Corrected: reconnect flag set to false
        //            _response = await _hearingInstrumentPage.CallBootAsync(bootTypeToUse, false);

        //            if (_response == null)
        //            {
        //                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"BootDevice API returned null response for boot type '{type}'.");
        //                throw new Exception($"BootDevice API response is null for boot type: '{type}'");
        //            }

        //            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Device booted successfully in '{type}' mode without reconnecting.");
        //            ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, $"BootDevice API Response - {type}", _response.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"An error occurred during device initialization or product configuration: {ex.Message}");
        //        throw;
        //    }
        //}


        //[Then("Device boots and API does not reconnects to the device")]
        //public void ThenDeviceBootsAndAPIDoesNotReconnectsToTheDevice()
        //{
            
        //}


    }
}
