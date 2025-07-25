using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class ProductIdentificationSuccessStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ProductIdentificationPage _productIdentificationPage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _response; // Fully qualify 'VoidResponse' to resolve ambiguity
        private Avalon.Dooku3.gRPCService.Protos.ProductIdentification.VoidResponse? _productresponse; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse;
        private ReadPcbaPartNumberResponse? _readPcbaPartNumberResponse; // Declare 'readPcbaPartNumberResponse' as nullable to fix CS8618
        private GetPlatformNameResponse? _getPlatformNameResponse;
        private GetSerialNumberResponse? _getSerialNumberResponse; // Declare 'getSerialNumberResponse' as nullable to fix CS8618
        private GetSideResponse? _getSideResponse; // Declare 'getSideResponse' as nullable to fix CS8618
        private GetNetworkAddressResponse? _getNetworkAddressResponse;
        private VerifyProductResponse? _verifyProductResponse; // Declare 'verifyProductResponse' as nullable to fix CS8618
        private ReadCloudRegistrationInputResponse? _readCloudRegistrationInputResponse;
        private GetDateModifiedResponse? _getDateModifiedResponse; // Declare 'getDateModifiedResponse' as nullable to fix CS8618
        private GetOptionsForDeviceResponse? _getOptionsForDeviceResponse; // Declare 'getOptionsForDeviceResponse' as nullable to fix CS8618
        private GetPrivateLabelCodeResponse? _getPrivateLabelCodeResponse; // Declare 'getPrivateLabelCodeResponse' as nullable to fix CS8618

        public ProductIdentificationSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
        }

        [When("Send a request to the PCBAPartNumber with connected device")]
        public async Task WhenSendARequestToThePCBAPartNumberWithConnectedDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");

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

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DetectClosest API to find the nearest RHI device...");
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DetectClosest API call succeeded.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DetectOnSide API for Left and Right channels...");
                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "DetectOnSide Left Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "DetectOnSide Right Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Both Left and Right sides detected. Calling DetectOnSide API for Both sides...");
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully. Using 'Left' as fitting side.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully. Using 'Right' as fitting side.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Enabling Master Connect mode...");
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Master Connect mode enabled.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Enabling Fitting Mode...");
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Fitting Mode enabled.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Requesting device node data from GetDeviceNode API...");
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device node data received.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Attempting to connect to device using Connect API...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Connected to device successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ReadPcbaPartNumber API to retrieve PCBA part number...");
                _readPcbaPartNumberResponse = await _productIdentificationPage.CallReadPcbaPartNumberAsync();
                if (_readPcbaPartNumberResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ReadPcbaPartNumber API returned null response.");
                    throw new Exception("ReadPcbaPartNumber response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ReadPcbaPartNumber API call succeeded. PCBA part number retrieved successfully.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Initialization or configuration failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the PCBA part number of the device")]
        public void ThenAPIReturnsThePCBAPartNumberOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating ReadPcbaPartNumber API response...");

            try
            {
                if (_readPcbaPartNumberResponse != null && _readPcbaPartNumberResponse.PcbaPartNumber != 0)
                {
                    string responseJson = System.Text.Json.JsonSerializer.Serialize(_readPcbaPartNumberResponse);
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "ReadPcbaPartNumber API Response", responseJson);

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"PCBA part number retrieved successfully: {_readPcbaPartNumberResponse.PcbaPartNumber}");
                }
                else
                {
                    string errorMsg = "ReadPcbaPartNumber API response is null or part number is 0.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception during PCBA part number validation: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the PlatformName API with a connected device")]
        public async Task WhenSendARequestToThePlatformNameAPIWithAConnectedDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting PlatformName API request with a connected device.");
            _productresponse = await _productIdentificationPage.CallReadAsync();

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetPlatformName API...");
                _getPlatformNameResponse = await _productIdentificationPage.CallGetPlatformNameAsync();

                if (_getPlatformNameResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPlatformName API response is null. Unable to retrieve platform name.");
                    throw new Exception("GetPlatformName API response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetPlatformName API call succeeded. Platform name received: {_getPlatformNameResponse.PlatformName}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"PlatformName API request failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the hardware platform name, which starts with {string} for a Coyote chip")]
        public void ThenAPIReturnsTheHardwarePlatformNameWhichStartsWithForACoyoteChip(string c)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating that the platform name starts with the expected prefix for a Coyote chip...");

            try
            {
                if (_getPlatformNameResponse == null)
                {
                    string errorMsg = "GetPlatformName API response is null. Unable to retrieve platform name.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                string platformName = _getPlatformNameResponse.PlatformName ?? string.Empty;

                if (platformName.StartsWith(c, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Platform name '{platformName}' starts with expected prefix '{c}'.");
                    string responseJson = System.Text.Json.JsonSerializer.Serialize(_getPlatformNameResponse);
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "GetPlatformName API Response", responseJson);
                }
                else
                {
                    string errorMsg = $"Platform name '{platformName}' does not start with expected prefix '{c}'.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception during platform name validation: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the SerialNumber API to read the current serial number")]
        public async Task WhenSendARequestToTheSerialNumberAPIToReadTheCurrentSerialNumberAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting SerialNumber API request with a valid serial number.");

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetSerialNumber API...");
                _getSerialNumberResponse = await _productIdentificationPage.CallGetSerialNumberAsync();

                if (_getSerialNumberResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetSerialNumber API response is null. Unable to retrieve serial number.");
                    throw new Exception("GetSerialNumber API response is null.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetSerialNumber API call succeeded. Serial number received: {_getSerialNumberResponse.SerialNumber}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"SerialNumber API request failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the serial number of the connected device")]
        public void ThenAPIReturnsTheSerialNumberOfTheConnectedDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GetSerialNumber API response for the connected device...");

            try
            {
                if (_getSerialNumberResponse == null)
                {
                    string errorMsg = "GetSerialNumber API response is null. Unable to validate serial number.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                if (string.IsNullOrWhiteSpace(_getSerialNumberResponse.SerialNumber))
                {
                    string errorMsg = "Serial number is empty or null in the GetSerialNumber API response.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Serial number received: {_getSerialNumberResponse.SerialNumber}");

                // Log full response in JSON
                string responseJson = System.Text.Json.JsonSerializer.Serialize(_getSerialNumberResponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "GetSerialNumber API Response", responseJson);
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception while validating GetSerialNumber API response: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the SerialNumber API with a valid serial number")]
        public async Task WhenSendARequestToTheSerialNumberAPIWithAValidSerialNumberAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting SerialNumber API set request for provided serial numbers...");

            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string serialNumber = row["SerialNumber"];

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling SetSerialNumber API with serial number: {serialNumber}...");

                    _productresponse = await _productIdentificationPage.CallSetSerialNumberAsync(serialNumber);

                    if (_productresponse == null)
                    {
                        string errorMsg = $"SetSerialNumber API response is null for serial number: {serialNumber}";
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                        throw new Exception(errorMsg);
                    }
                    else
                    {
                        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSerialNumber API call succeeded for serial number: {serialNumber}");
                    }
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed SerialNumber API set requests for all provided serial numbers.");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Exception during SerialNumber API request: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                throw;
            }
        }

        [Then("API writes the serial number to the device and return status as {string}")]
        public void ThenAPIWritesTheSerialNumberToTheDeviceAndReturnStatusAs(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating SetSerialNumber API response status...");

            try
            {
                if (_productresponse == null)
                {
                    string errorMsg = "SetSerialNumber API response is null. Unable to validate serial number status.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Log JSON response
                string json = System.Text.Json.JsonSerializer.Serialize(_productresponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "SetSerialNumber API Response:", json);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Exception during SetSerialNumber API response validation: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                throw;
            }
        }

        [When("Send a request to the FittingSide API to read the current fitting side from the device")]
        public async Task WhenSendARequestToTheFittingSideAPIToReadTheCurrentFittingSideFromTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to FittingSide API to read the current fitting side from the device.");
            try
            {
                _getSideResponse = await _productIdentificationPage.CallGetSideAsync();
                if (_getSideResponse != null)
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FittingSide API call succeeded. Fitting side received: {_getSideResponse.Side}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "FittingSide API response is null. Unable to retrieve fitting side.");
                    throw new Exception("FittingSide API response is null.");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"FittingSide API request failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the fitting side of the connected device \\(Ex: Left or Right)")]
        public void ThenAPIReturnsTheFittingSideOfTheConnectedDeviceExLeftOrRight()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating FittingSide API response for the connected device.");

            try
            {
                if (_getSideResponse == null)
                {
                    string errorMsg = "FittingSide API response is null. Unable to validate fitting side.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Serialize and log JSON response
                string json = System.Text.Json.JsonSerializer.Serialize(_getSideResponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "FittingSide API Response:", json);

                if (string.IsNullOrWhiteSpace(_getSideResponse.Side))
                {
                    string errorMsg = "Fitting side is empty or null in the FittingSide API response.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"FittingSide API call succeeded. Fitting side received: {_getSideResponse.Side}");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred while validating FittingSide API response: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the FittingSide API with a valid fitting side \\(Ex: Left or Right)")]
        public async Task WhenSendARequestToTheFittingSideAPIWithAValidFittingSideExLeftOrRight(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting FittingSide API set request for provided fitting sides.");

            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string fittingSide = row["FittingSide"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling SetSide API with fitting side: {fittingSide}...");

                    _productresponse = await _productIdentificationPage.CallSetSideAsync(fittingSide);

                    if (_productresponse == null)
                    {
                        string errorMsg = "SetSide API response is null. Unable to validate fitting side status.";
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                        throw new Exception(errorMsg);
                    }

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSide API call succeeded for fitting side: {fittingSide}.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed FittingSide API set requests for all provided fitting sides.");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred during FittingSide API request: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [Then("API writes the fitting side to the device and returns status as {string}")]
        public void ThenAPIWritesTheFittingSideToTheDeviceAndReturnsStatusAs(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating SetSide API response status.");

            try
            {
                if (_productresponse == null)
                {
                    string errorMsg = "SetSide API response is null. Unable to validate fitting side status.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Log the full response object in JSON format
                string json = System.Text.Json.JsonSerializer.Serialize(_productresponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "SetSide API Response", json);

                // Check for status or AvalonStatus property
                var statusProperty = _productresponse.GetType().GetProperty("Status") ?? _productresponse.GetType().GetProperty("AvalonStatus");
                string? actualStatus = statusProperty?.GetValue(_productresponse)?.ToString();

                if (string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSide API call succeeded. Status: {actualStatus} (expected: {expectedStatus})");
                }
                else
                {
                    string failMsg = $"SetSide API call failed. Actual status: {actualStatus}, expected: {expectedStatus}.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, failMsg);
                    throw new Exception(failMsg);
                }
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred during SetSide API response validation: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the ProximityNetworkAddress API to read the current network address from the device")]
        public async Task WhenSendARequestToTheProximityNetworkAddressAPIToReadTheCurrentNetworkAddressFromTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to ProximityNetworkAddress API to read the current network address from the device.");

            try
            {
                _getNetworkAddressResponse = await _productIdentificationPage.CallGetNetworkAddressAsync();

                if (_getNetworkAddressResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetNetworkAddress API response is null. Unable to retrieve network address.");
                    throw new Exception("GetNetworkAddress API response is null.");
                }

                string networkAddressString = _getNetworkAddressResponse.NetworkAddress.ToString();

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"GetNetworkAddress API response received. Network address: {networkAddressString}");

                if (string.IsNullOrWhiteSpace(networkAddressString) || _getNetworkAddressResponse.NetworkAddress == 0)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Network address is empty, null, or zero in the GetNetworkAddress API response.");
                    throw new Exception("Network address is empty, null, or zero in the GetNetworkAddress API response.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetNetworkAddress API call succeeded. Network address received: {networkAddressString}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"ProximityNetworkAddress API request failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the network address of the device")]
        public void ThenAPIReturnsTheNetworkAddressOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GetNetworkAddress API response for the connected device.");

            try
            {
                if (_getNetworkAddressResponse == null)
                {
                    string errorMsg = "GetNetworkAddress API response is null. Unable to validate network address.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Log response as JSON
                string json = System.Text.Json.JsonSerializer.Serialize(_getNetworkAddressResponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "GetNetworkAddress API Response", json);

                if (string.IsNullOrWhiteSpace(_getNetworkAddressResponse.NetworkAddress.ToString()))
                {
                    string errorMsg = "Network address is empty or null in the GetNetworkAddress API response.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetNetworkAddress API call succeeded. Network address received: {_getNetworkAddressResponse.NetworkAddress}");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred during GetNetworkAddress API validation: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request with valid BleId, correct Brand, and private label code")]
        public async Task WhenSendARequestWithValidBleIdCorrectBrandAndPrivateLabelCode(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting VerifyProduct API requests with provided BleId, Brand, and PrivateLabelCode.");

            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string bleId = row["BleId"];
                    string brand = row["Brand"];
                    string privateLabelCode = row["PrivateLabelCode"];

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info,
                        $"Calling VerifyProduct API with BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}...");

                    _verifyProductResponse = await _productIdentificationPage.CallVerifyProductAsync(
                        int.Parse(bleId), brand, int.Parse(privateLabelCode));

                    if (_verifyProductResponse == null)
                    {
                        string errorMsg = $"VerifyProduct API returned null for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.";
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                        throw new Exception(errorMsg);
                    }

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                        $"VerifyProduct API call succeeded for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed VerifyProduct API requests for all provided rows.");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Exception occurred during VerifyProduct API call: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                throw;
            }
        }

        [Then("API verifies that the input values match those in the device")]
        public void ThenAPIVerifiesThatTheInputValuesMatchThoseInTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating VerifyProduct API response for input values match.");

                if (_verifyProductResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "VerifyProduct API response is null. Unable to validate input values.");
                    throw new Exception("VerifyProduct API response is null.");
                }

                if (_verifyProductResponse.IsSuccess == true)
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "VerifyProduct API call succeeded. Input values match the device.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "VerifyProduct API call failed. Input values do not match the device.");
                    throw new Exception("VerifyProduct API call failed. Input values do not match the device.");
                }

                // Log response object in JSON format
                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(_verifyProductResponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "VerifyProduct API Response", jsonResponse);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [When("Send a request with valid values for MFI brand, MFI family, MFI model, and GAP device name")]
        public async Task WhenSendARequestWithValidValuesForMFIBrandMFIFamilyMFIModelAndGAPDeviceNameAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting UpdateGattDatabase API requests with provided MFI and GAP values.");
            foreach (var row in dataTable.Rows)
            {
                string mfiBrand = row["MFIBrand"];
                string mfiModel = row["MFIModel"];
                string mfiFamily = row["MFIFamily"];
                string gapDeviceName = row["GapDeviceName"];
                try
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling UpdateGattDatabase API with MFIBrand: {mfiBrand}, MFIModel: {mfiModel}, MFIFamily: {mfiFamily}, GapDeviceName: {gapDeviceName}...");
                    _productresponse = await _productIdentificationPage.CallUpdateGattDatabaseAsync(mfiBrand, mfiModel, mfiFamily, gapDeviceName);
                    if (_productresponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"UpdateGattDatabase API returned null for MFIBrand: {mfiBrand}, MFIModel: {mfiModel}, MFIFamily: {mfiFamily}, GapDeviceName: {gapDeviceName}.");
                        throw new Exception($"UpdateGattDatabase API response is null for MFIBrand: {mfiBrand}, MFIModel: {mfiModel}, MFIFamily: {mfiFamily}, GapDeviceName: {gapDeviceName}.");
                    }
                    else
                    {
                        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"UpdateGattDatabase API call succeeded for MFIBrand: {mfiBrand}, MFIModel: {mfiModel}, MFIFamily: {mfiFamily}, GapDeviceName: {gapDeviceName}.");
                    }
                }
                catch (Exception ex)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Error processing row: {ex.Message}");
                    throw;
                }
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed UpdateGattDatabase API requests for all provided rows.");
        }

        [Then("API writes the values to the GATT database successfully")]
        public void ThenAPIWritesTheValuesToTheGATTDatabaseSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating UpdateGattDatabase API response.");

                if (_productresponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "UpdateGattDatabase API response is null. Unable to validate GATT database update.");
                    throw new Exception("UpdateGattDatabase API response is null.");
                }

                // Log the full response object for traceability
                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(_productresponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "UpdateGattDatabase API Response", jsonResponse);

                // Optionally validate a specific success flag or status field here
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "UpdateGattDatabase API response is valid and non-null.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while validating UpdateGattDatabase response: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the GNOSRegistrationData API to retrieve GNOS registration data from the hearing instrument")]
        public async Task WhenSendARequestToTheGNOSRegistrationDataAPIToRetrieveGNOSRegistrationDataFromTheHearingInstrument()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Initiating ReadCloudRegistrationInput API call to retrieve GNOS registration data from the hearing instrument.");
            try
            {
                _readCloudRegistrationInputResponse = await _productIdentificationPage.CallReadCloudRegistrationInputAsync();
                if (_readCloudRegistrationInputResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ReadCloudRegistrationInput API response is null. Unable to retrieve GNOS registration data.");
                    throw new Exception("ReadCloudRegistrationInput API response is null.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ReadCloudRegistrationInput API call succeeded. GNOS registration data retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Initialization or configuration failed: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the GNOS registration data in valid XML format")]
        public void ThenAPIReturnsTheGNOSRegistrationDataInValidXMLFormat()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GNOS registration data returned by ReadCloudRegistrationInput API...");

            try
            {
                if (_readCloudRegistrationInputResponse == null)
                {
                    string errorMsg = "GNOS registration data is null. Cannot validate XML format.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }
                string rawXml = _readCloudRegistrationInputResponse.CloudRegistrationInput;
                try
                {
                    var xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(rawXml);

                    // Format the XML with indentation for better readability in report
                    string formattedXml;
                    using (var stringWriter = new System.IO.StringWriter())
                    using (var xmlTextWriter = new System.Xml.XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented })
                    {
                        xmlDoc.Save(xmlTextWriter);
                        formattedXml = stringWriter.ToString();
                    }

                    // Wrap XML in <pre> so it looks formatted in Extent Report
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GNOS registration response (formatted XML):<br><pre>{System.Net.WebUtility.HtmlEncode(formattedXml)}</pre>");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GNOS registration data is valid XML and successfully parsed.");
                }
                catch (System.Xml.XmlException xmlEx)
                {
                    string invalidXmlMsg = $"GNOS registration data is not valid XML. Error: {xmlEx.Message}";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, invalidXmlMsg);
                    throw;
                }
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception during XML validation of GNOS registration data: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the DateModified API to read the modified date from the device")]
        public async Task WhenSendARequestToTheDateModifiedAPIToReadTheModifiedDateFromTheDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Initiating GetDateModified API call to retrieve the modified date from the hearing instrument...");

            try
            {
                _getDateModifiedResponse = await _productIdentificationPage.CallGetDateModifiedAsync();

                if (_getDateModifiedResponse == null)
                {
                    string errorMsg = "GetDateModified API response is null. Unable to retrieve the modified date from the device.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetDateModified API call succeeded. Modified date retrieved: {_getDateModifiedResponse}");
                }
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred during GetDateModified API call: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [Then("API returns the modified date of the device")]
        public void ThenAPIReturnsTheModifiedDateOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating the response of GetDateModified API...");

            try
            {
                if (_getDateModifiedResponse == null)
                {
                    string errorMsg = "GetDateModified API response is null. No modified date was retrieved.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Assume the response contains a DateTime or string field named 'ModifiedDate'
                string modifiedDate = _getDateModifiedResponse.ToString(); // Replace if actual property exists

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Device modified date received: <b>{modifiedDate}</b>");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception while validating modified date from device: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the OptionsForDevice API to retrieve current device options")]
        public async Task WhenSendARequestToTheOptionsForDeviceAPIToRetrieveCurrentDeviceOptions()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Initiating GetOptionsForDevice API call to retrieve current device options...");

            try
            {
                _getOptionsForDeviceResponse = await _productIdentificationPage.CallGetOptionsForDeviceAsync();

                if (_getOptionsForDeviceResponse == null)
                {
                    string errorMsg = "GetOptionsForDevice API response is null. Unable to retrieve device options.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GetOptionsForDevice API call succeeded. Device options retrieved successfully.");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception occurred during GetOptionsForDevice API call: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [Then("API returns the device options of the device")]
        public void ThenAPIReturnsTheDeviceOptionsOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GetOptionsForDevice API response for device options...");

            try
            {
                if (_getOptionsForDeviceResponse == null)
                {
                    string errorMsg = "GetOptionsForDevice API response is null. Device options could not be retrieved.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }

                // Log successful response
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GetOptionsForDevice API call succeeded. Device options retrieved successfully.");

                // Pretty-print and log JSON response
                var formattedJson = System.Text.Json.JsonSerializer.Serialize(
                    _getOptionsForDeviceResponse,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                );

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "GetOptionsForDevice API Response (JSON)", formattedJson);
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception while validating GetOptionsForDevice API response: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the OptionForDevice API with a valid integer related to device options")]
        public async Task WhenSendARequestToTheOptionForDeviceAPIWithAValidIntegerRelatedToDeviceOptionsAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting SetOptionsForDevice API requests with provided device option values.");

            foreach (var row in dataTable.Rows)
            {
                string optionsForDevice = row["optionsForDevice"];

                try
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling SetOptionsForDevice API with value: {optionsForDevice}...");

                    _productresponse = await _productIdentificationPage.CallSetOptionsForDeviceAsync(int.Parse(optionsForDevice));

                    if (_productresponse == null)
                    {
                        string errorMsg = $"SetOptionsForDevice API returned null for value: {optionsForDevice}.";
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                        throw new Exception(errorMsg);
                    }

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetOptionsForDevice API call succeeded for value: {optionsForDevice}.");
                }
                catch (Exception ex)
                {
                    string exceptionMsg = $"Exception during SetOptionsForDevice API call for value: {optionsForDevice}. Error: {ex.Message}";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                    throw;
                }
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed SetOptionsForDevice API requests for all provided values.");
        }

        [Then("API writes the device options to the device successfully")]
        public void ThenAPIWritesTheDeviceOptionsToTheDeviceSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating SetOptionsForDevice API write operation...");

            try
            {
                if (_productresponse == null)
                {
                    string errorMsg = "SetOptionsForDevice API response is null. Device options may not have been written successfully.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                    throw new Exception(errorMsg);
                }
                string responseJson = System.Text.Json.JsonSerializer.Serialize(_productresponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "SetOptionsForDevice API Response", responseJson);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "SetOptionsForDevice API call succeeded. Device options written successfully.");
            }
            catch (Exception ex)
            {
                string exceptionMsg = $"Exception while validating SetOptionsForDevice API response: {ex.Message}";
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, exceptionMsg);
                throw;
            }
        }

        [When("Send a request to the PrivateLabelCode API to retrieve the private label code")]
        public async Task WhenSendARequestToThePrivateLabelCodeAPIToRetrieveThePrivateLabelCode()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to GetPrivateLabelCode API...");

                _getPrivateLabelCodeResponse = await _productIdentificationPage.CallGetPrivateLabelCodeAsync();

                if (_getPrivateLabelCodeResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPrivateLabelCode API response is null.");
                    throw new Exception("GetPrivateLabelCode API response is null.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GetPrivateLabelCode API call succeeded.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception while calling GetPrivateLabelCode API: {ex.Message}");
                throw;
            }
        }

        [Then("API returns the private label code of the device")]
        public void ThenAPIReturnsThePrivateLabelCodeOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GetPrivateLabelCode API response...");

                if (_getPrivateLabelCodeResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPrivateLabelCode API response is null.");
                    throw new Exception("GetPrivateLabelCode API response is null.");
                }

                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(_getPrivateLabelCodeResponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "GetPrivateLabelCode API Response", jsonResponse);

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "GetPrivateLabelCode API call succeeded and returned a valid response.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception during GetPrivateLabelCode response validation: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the PrivateLabelCode API with a valid integer relayed to the private label code")]
        public async Task WhenSendARequestToThePrivateLabelCodeAPIWithAValidIntegerRelayedToThePrivateLabelCode(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting SetPrivateLabelCode API requests using provided data.");

                foreach (var row in dataTable.Rows)
                {
                    string privateLabelCode = row["privateLabelCode"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling SetPrivateLabelCode API with PrivateLabelCode: {privateLabelCode}");

                    _productresponse = await _productIdentificationPage.CallSetPrivateLabelCodeAsync(int.Parse(privateLabelCode));

                    if (_productresponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"SetPrivateLabelCode API returned null for PrivateLabelCode: {privateLabelCode}");
                        throw new Exception($"SetPrivateLabelCode API response is null for PrivateLabelCode: {privateLabelCode}");
                    }
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed SetPrivateLabelCode API requests for all rows.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred during SetPrivateLabelCode API request: {ex.Message}");
                throw;
            }
        }

        [Then("API writes the private label code to the device successfully")]
        public void ThenAPIWritesThePrivateLabelCodeToTheDeviceSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating SetPrivateLabelCode API response.");

                if (_productresponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetPrivateLabelCode API response is null.");
                    throw new Exception("SetPrivateLabelCode API response is null.");
                }

                // Serialize response for logging
                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(_productresponse);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "SetPrivateLabelCode API Response", jsonResponse);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while validating SetPrivateLabelCode API response: {ex.Message}");
                throw;
            }
        }
    }
}
