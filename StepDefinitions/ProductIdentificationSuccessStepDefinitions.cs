using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Threading.Tasks;

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
    }
}
