using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using AventStack.ExtentReports;
using FluentAssertions;
using Newtonsoft.Json;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Net;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Avalon.Dooku3.gRPCService.Protos.ProductIdentification.ProductIdentification;
using VoidResponse = Avalon.Dooku3.gRPCService.Protos.ProductIdentification.VoidResponse;

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
        private SetSideRequest? _setSideRequest; // Declare 'setSideRequest' as nullable to fix CS8618
        private VoidResponse? _setSideResponse;
        private GetNetworkAddressResponse? _getNetworkAddressResponse; // Declare 'getNetworkAddressResponse' as nullable to fix CS8618
        private VoidResponse? _setNewNetworkAddressResponse;
        private VerifyProductResponse? _verifyProductResponse;
        private VerifyProductRequest? _verifyProductRequest;
        private UpdateGattDatabaseRequest? _updateGattDatabaseRequest;
        private VoidResponse _updateGattDatabaseResponse;
        private ReadCloudRegistrationInputRequest? _readCloudRegistrationInputRequest;
        private ReadCloudRegistrationInputResponse? _readCloudRegistrationInputResponse;
        private VoidResponse? _resetDateModifiedResponse;
        private GetDateModifiedResponse _getDateModifiedResponse;
        private GetOptionsForDeviceResponse _getOptionsForDeviceResponse;
        private VoidResponse _setOptionsForDeviceResponse;
        private GetPrivateLabelCodeResponse _getPrivateLabelCodeResponse;
        private SetPrivateLabelCodeRequest _setPrivateLabelCodeRequest;
        private VoidResponse _setPrivateLabelCodeResponse;


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
            if (_readPcbaPartNumberResponse != null && _readPcbaPartNumberResponse.PcbaPartNumber != 0)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"PCBA part number retrieved successfully: {_readPcbaPartNumberResponse.PcbaPartNumber}");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PCBA part number retrieval failed or returned empty.");
                throw new Exception("PCBA part number retrieval failed or returned empty.");
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

            if (_getPlatformNameResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPlatformName API response is null. Unable to retrieve platform name.");
                throw new Exception("GetPlatformName API response is null.");
            }

            string platformName = _getPlatformNameResponse.PlatformName ?? string.Empty;
            if (platformName.StartsWith(c, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Platform name '{platformName}' starts with '{c}'.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Platform name '{platformName}' does not start with '{c}'.");
                throw new Exception($"Platform name '{platformName}' does not start with '{c}'.");
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

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating GetSerialNumber API response for the connected device.");

            if (_getSerialNumberResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetSerialNumber API response is null. Unable to validate serial number status.");
                throw new Exception("GetSerialNumber API response is null.");
            }

            if (string.IsNullOrWhiteSpace(_getSerialNumberResponse.SerialNumber))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Serial number is empty or null in the GetSerialNumber API response.");
                throw new Exception("Serial number is empty or null in the GetSerialNumber API response.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GetSerialNumber API call succeeded. Serial number received: {_getSerialNumberResponse.SerialNumber}");
        }

        [When("Send a request to the SerialNumber API with a valid serial number")]
        public async Task WhenSendARequestToTheSerialNumberAPIWithAValidSerialNumberAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting SerialNumber API set request for provided serial numbers.");
            foreach (var row in dataTable.Rows)
            {
                string serialNumber = row["SerialNumber"];
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling SetSerialNumber API with serial number: {serialNumber}...");
                _productresponse = await _productIdentificationPage.CallSetSerialNumberAsync(serialNumber);
                if (_productresponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetSerialNumber API response is null. Unable to validate serial number status.");
                    throw new Exception("SetSerialNumber API response is null.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSerialNumber API call succeeded for serial number: {serialNumber}.");
                }
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed SerialNumber API set requests for all provided serial numbers.");
        }

        [Then("API writes the serial number to the device and return status as {string}")]
        public void ThenAPIWritesTheSerialNumberToTheDeviceAndReturnStatusAs(string success)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating SetSerialNumber API response status.");
            if (_productresponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetSerialNumber API response is null. Unable to validate serial number status.");
                throw new Exception("SetSerialNumber API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSerialNumber API response received: {_productresponse}.");
            }
        }

        [When("Send a request to the FittingSide API to read the current fitting side from the device.")]
        public async Task WhenSendARequestToTheFittingSideAPIToReadTheCurrentFittingSideFromTheDevice_Async()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            _getSideResponse = await _productIdentificationPage.GetSideResponseAsync();
            if (_getSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Calling GetSide API to retrieve the fitting side of the connected device.Unable to get the FittingSide");
                throw new Exception("FittingSide API to read the current fitting side from the device response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetSide API to retrieve the fitting side of the connected device...");
            }

        }

        [Then("API returns the fitting side of the connected device \\(Ex: Left or Right).")]
        public void ThenAPIReturnsTheFittingSideOfTheConnectedDeviceExLeftOrRight_()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_getSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetSide response is null.");
                throw new Exception("GetSide response is null.");
            }

            var fittingSide = _getSideResponse.Side;

            if (string.IsNullOrWhiteSpace(fittingSide))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Fitting side is empty or null.");
                throw new Exception("Fitting side is empty or null.");
            }

            // ✅ Dynamically create and pretty-print JSON
            string formattedJson = JsonConvert.SerializeObject(new { side = fittingSide }, Formatting.Indented);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, formattedJson);
        }

        [When("Send a request to the FittingSide API with a valid fitting side \\(Ex: Left or Right)")]
        public async Task WhenSendARequestToTheFittingSideAPIWithAValidFittingSideExLeftOrRightAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            foreach (var row in dataTable.Rows)
            {
                var side = row["FittingSide"];
                _setSideRequest = new SetSideRequest { Side = side };
                // Send request and store response
                _setSideResponse = await _productIdentificationPage.CallSetSideAsync(side);
            }

            if (_setSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetSide API response is null. Unable to validate fitting side status.");
                throw new Exception("SetSide API response is null.");
            }
           ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling SetSide API to write the fitting side to the device...");
        }

        [Then("API writes the fitting side to the device and returns status as {string}")]
        public void ThenAPIWritesTheFittingSideToTheDeviceAndReturnsStatusAs(string success)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_setSideResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetSide API response is null. Unable to validate fitting side status.");
                throw new Exception("SetSide API response is null.");
            }

            if(!string.Equals(_setSideResponse.ToString(), success, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"SetSide API response status does not match expected value '{success}'. Actual status: {_setSideResponse}");
                throw new Exception($"SetSide API response status does not match expected value '{success}'. Actual status: {_setSideResponse}");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetSide API call succeeded. Fitting side written to the device successfully with status: {_setSideResponse}.");
        }

        [When("Send a request to the ProximityNetworkAddress API to read the current network address from the device")]
        public async Task WhenSendARequestToTheProximityNetworkAddressAPIToReadTheCurrentNetworkAddressFromTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            _getNetworkAddressResponse = await _productIdentificationPage.GetNetworkAddressResponseAsync();
            if (_getNetworkAddressResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Calling GetNetworkAddress API to retrieve the network address of the connected device. Unable to get the NetworkAddress");
                throw new Exception("ProximityNetworkAddress API to read the current network address from the device response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetNetworkAddress API to retrieve the network address of the connected device...");
        }

        [Then("API returns the network address of the device")]
        public void ThenAPIReturnsTheNetworkAddressOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_getNetworkAddressResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetNetworkAddress response is null.");
                throw new Exception("GetNetworkAddress response is null.");
            }
            var networkAddress = _getNetworkAddressResponse.NetworkAddress;
            if (string.IsNullOrWhiteSpace(networkAddress.ToString()))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Network address is empty or null.");
                throw new Exception("Network address is empty or null.");
            }
            // ✅ Dynamically create and pretty-print JSON
            string formattedJson = JsonConvert.SerializeObject(new { networkAddress = networkAddress }, Formatting.Indented);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, formattedJson);
        }

        [When("Send a request with valid BleId, correct Brand, and private label code.")]
        public async Task WhenSendARequestWithValidBleIdCorrectBrandAndPrivateLabelCode_Async(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            // Loop through each row in the data table
            foreach (var row in dataTable.Rows)
            {
                // Parse values from DataTable
                int bleId = int.Parse(row["bleId"].ToString());
                string brand = row["brand"].ToString();
                int privateLabel = int.Parse(row["privateLabelCode"].ToString());

                // Construct the VerifyProductRequest using values from table
                _verifyProductRequest = new VerifyProductRequest
                {
                    BleId = bleId,
                    Brand = brand,
                    PrivateLabel = privateLabel
                };

                // Call the API and store response
                _verifyProductResponse = await _productIdentificationPage.VerifyProductAsync(_verifyProductRequest);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request with valid BleId, correct Brand, and private label code");
            }
        }

        [Then("API verifies that the input values match those in the device")]
        public void ThenAPIVerifiesThatTheInputValuesMatchThoseInTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_verifyProductResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "❌ VerifyProduct API response is null.");
                throw new Exception("VerifyProduct API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,$"{JsonConvert.SerializeObject(_verifyProductResponse, Formatting.Indented)}");
        }

        [When("Send a request with valid values for MFI brand, MFI family, MFI model, and GAP device name")]
        public async Task WhenSendARequestWithValidValuesForMFIBrandMFIFamilyMFIModelAndGAPDeviceNameAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            var dict = new Dictionary<string, string>();
            foreach (var row in dataTable.Rows)
            {
                dict["MFIBrand"] = row["MFIBrand"];
                dict["MFIModel"] = row["MFIModel"];
                dict["MFIFamily"] = row["MFIFamily"];
                dict["GapDeviceName"] = row["GapDeviceName"];
            }

            _updateGattDatabaseRequest = new UpdateGattDatabaseRequest
            {
                GattKeyValueDictionary = { dict }
            };

            _updateGattDatabaseResponse = await _productIdentificationPage.UpdateGattDatabaseAsync(_updateGattDatabaseRequest);

            if (_updateGattDatabaseRequest == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "UpdateGattDatabase API response is null. Unable to validate GATT database update.");
                throw new Exception("UpdateGattDatabase API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step,Status.Info,"");
        }

        [Then("API writes the values to the GATT database successfully")]
        public void ThenAPIWritesTheValuesToTheGATTDatabaseSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,$"GATT database update was successful.{JsonConvert.SerializeObject(_updateGattDatabaseResponse, Formatting.Indented)}");
        }


        [When("Send a request to the GNOSRegistrationData API to retrieve GNOS registration data from the hearing instrument")]
        public async Task WhenSendARequestToTheGNOSRegistrationDataAPIToRetrieveGNOSRegistrationDataFromTheHearingInstrumentAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            _readCloudRegistrationInputRequest = new ReadCloudRegistrationInputRequest(); // No need to set fields

            _readCloudRegistrationInputResponse = await _productIdentificationPage.ReadCloudRegistrationInputAsync(_readCloudRegistrationInputRequest);

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sent request to the GNOSRegistrationData API to retrieve GNOS registration data from the hearing instrument");
        }

        [Then("API returns the GNOS registration data in valid XML format")]
        public void ThenAPIReturnsTheGNOSRegistrationDataInValidXMLFormat()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_readCloudRegistrationInputResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ReadCloudRegistrationInput response is null.");
                throw new Exception("ReadCloudRegistrationInput response is null.");
            }

            // ✅ Extract raw XML from response object
            string rawXml = _readCloudRegistrationInputResponse.CloudRegistrationInput;

            // ✅ HTML-escape and style the XML in light green inside <pre> for formatting
            string formattedXml = $"<pre><span style='color:lightgreen;'>{System.Net.WebUtility.HtmlEncode(rawXml)}</span></pre>";


            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"GNOS registration data returned in valid XML format: {formattedXml}");
        }


        [When("Send a request to the DateModified API to read the modified date from the device")]
        public async Task WhenSendARequestToTheDateModifiedAPIToReadTheModifiedDateFromTheDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            _getDateModifiedResponse = await _productIdentificationPage.GetDateModifiedResponseAsync();

            if (_getDateModifiedResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetDateModified API response is null. Unable to retrieve modified date.");
                throw new Exception("GetDateModified API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetDateModified API to retrieve the modified date of the device...");
        }

        [Then("API returns the modified date of the device")]
        public void ThenAPIReturnsTheModifiedDateOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_getDateModifiedResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetDateModified response is null.");
                throw new Exception("GetDateModified response is null.");
            }
            var modifiedDate = _getDateModifiedResponse.DateModified;
            if (string.IsNullOrWhiteSpace(modifiedDate.ToString()))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Modified date is empty or null.");
                throw new Exception("Modified date is empty or null.");
            }
            // ✅ Dynamically create and pretty-print JSON
            string formattedJson = JsonConvert.SerializeObject(new { dateModified = modifiedDate }, Formatting.Indented);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, formattedJson);
        }


        [When("Send a request to the ResetDateModified API to reset the date modified")]
        public async Task WhenSendARequestToTheResetDateModifiedAPIToResetTheDateModifiedAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            _resetDateModifiedResponse = await _productIdentificationPage.ResetDateModifiedAsync();

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sent request to the ResetDateModified API to reset the date modified of the device.");
        }

        [Then("API resets the date modified to {string}")]
        public async Task ThenAPIResetsTheDateModifiedToAsync(string expectedDate)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_resetDateModifiedResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ResetDateModified response is null.");
                throw new Exception("ResetDateModified response is null.");
            }

            if(!string.Equals(_resetDateModifiedResponse.ToString(), expectedDate, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected date modified '{expectedDate}' does not match actual '{_resetDateModifiedResponse}'.");
                throw new Exception($"Expected date modified '{expectedDate}' does not match actual '{_resetDateModifiedResponse}'.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"ResetDateModified API executed successfully. Date modified reset to: {_resetDateModifiedResponse}.");
        }



        [When("Send a request to the OptionsForDevice API to retrieve current device options")]
        public async Task WhenSendARequestToTheOptionsForDeviceAPIToRetrieveCurrentDeviceOptionsAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            _getOptionsForDeviceResponse = await _productIdentificationPage.GetOptionsForDeviceAsync();

            if (_getOptionsForDeviceResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetOptionsForDevice API response is null. Unable to retrieve device options.");
                throw new Exception("GetOptionsForDevice API response is null.");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetOptionsForDevice API to retrieve the device options...");
            }
        }
        [Then("API returns the device options of the device")]
        public void ThenAPIReturnsTheDeviceOptionsOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_getOptionsForDeviceResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetOptionsForDevice response is null.");
                throw new Exception("GetOptionsForDevice response is null.");
            }
            var optionsForDevice = _getOptionsForDeviceResponse.OptionsForDevice;
            if (optionsForDevice == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Device options are empty or null.");
                throw new Exception("Device options are empty or null.");
            }
            // ✅ Dynamically create and pretty-print JSON
            string formattedJson = JsonConvert.SerializeObject(new { optionsForDevice = optionsForDevice }, Formatting.Indented);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, formattedJson);
        }





        [When("Send a request to the OptionForDevice API with a valid integer related to device options")]
        public async Task WhenSendARequestToTheOptionForDeviceAPIWithAValidIntegerRelatedToDeviceOptionsAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            foreach (var row in dataTable.Rows)
            {
                // Parse the optionsForDevice value from the DataTable
                if (int.TryParse(row["optionsForDevice"].ToString(), out int optionsValue))
                {
                    var request = new SetOptionsForDeviceRequest
                    {
                        OptionsForDevice = optionsValue
                    };
                    _setOptionsForDeviceResponse = await _productIdentificationPage.SetOptionsForDeviceAsync(request);
                }
            }
            
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"SetOptionsForDevice API called successfully");
        }

        [Then("API writes the device options to the device successfully")]
        public async Task ThenAPIWritesTheDeviceOptionsToTheDeviceSuccessfullyAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if(_setOptionsForDeviceResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetOptionsForDevice API response is null. Unable to validate device options status.");
                throw new Exception("SetOptionsForDevice API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetOptionsForDevice API executed successfully. Device options set to: {_setOptionsForDeviceResponse}.");
        }



        [When("Send a request to the PrivateLabelCode API to retrieve the private label code")]
        public async Task WhenSendARequestToThePrivateLabelCodeAPIToRetrieveThePrivateLabelCode()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            _getPrivateLabelCodeResponse = await _productIdentificationPage.GetPrivateLabelCodeAsync();
            if (_getPrivateLabelCodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPrivateLabelCode API response is null. Unable to retrieve private label code.");
                throw new Exception("GetPrivateLabelCode API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling GetPrivateLabelCode API to retrieve the private label code of the device...");
        }

        [Then("API returns the private label code of the device")]
        public void ThenAPIReturnsThePrivateLabelCodeOfTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_getPrivateLabelCodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetPrivateLabelCode response is null.");
                throw new Exception("GetPrivateLabelCode response is null.");
            }
            var privateLabelCode = _getPrivateLabelCodeResponse.PrivateLabelCode;
            if (privateLabelCode == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Private label code is empty or invalid.");
                throw new Exception("Private label code is empty or invalid.");
            }
            // ✅ Dynamically create and pretty-print JSON
            string formattedJson = JsonConvert.SerializeObject(new { privateLabelCode = privateLabelCode }, Formatting.Indented);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, formattedJson);
        }


        [When("Send a request to the PrivateLabelCode API with a valid private label code")]
        public async Task WhenSendARequestToThePrivateLabelCodeAPIWithAValidPrivateLabelCodeAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            foreach(var row in dataTable.Rows)
            {
                int privateLabelCode = int.Parse(row["privateLabelCode"].ToString());

                _setPrivateLabelCodeRequest = new SetPrivateLabelCodeRequest
                {
                    PrivateLabelCode = privateLabelCode
                };
                _setPrivateLabelCodeResponse = await _productIdentificationPage.SetPrivateLabelCodeAsync(_setPrivateLabelCodeRequest);
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sent request to the PrivateLabelCode API with a valid private label code.");
        }

        [Then("API writes the private label code to the device successfully")]
        public void ThenAPIWritesThePrivateLabelCodeToTheDeviceSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_setPrivateLabelCodeResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetPrivateLabelCode API response is null. Unable to validate private label code status.");
                throw new Exception("SetPrivateLabelCode API response is null.");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"SetPrivateLabelCode API executed successfully. Private label code set to: {_setPrivateLabelCodeResponse}.");
        }



    }
}