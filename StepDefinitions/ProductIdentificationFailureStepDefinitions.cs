using System;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using AventStack.ExtentReports;
using Newtonsoft.Json;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class ProductIdentificationFailureStepDefinitions
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
        private VerifyProductResponse? _verifyProductResponse;
        private VerifyProductRequest? _verifyProductRequest;

        public ProductIdentificationFailureStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
        }

        [When("Send a request with valid BleId, Brand, and private label code as Non-Zero")]
        public async Task WhenSendARequestWithValidBleIdBrandAndPrivateLabelCodeAsNon_ZeroAsync(DataTable dataTable)
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

                 Thread.Sleep(10000); // Wait for 10 seconds to ensure connection is established

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Preparing to send VerifyProduct request with valid BleId, Brand, and private label code.");

                foreach (var row in dataTable.Rows)
                {
                    // Parse values from DataTable
                    int bleId = int.Parse(row["bleId"]);
                    string brand = row["brand"];
                    int privateLabel = int.Parse(row["privateLabelCode"]);

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
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Initialization or configuration failed: {ex.Message}");
                throw;
            }
        }

        [Then("API skips Brand verification")]
        public void ThenAPISkipsBrandVerification()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_verifyProductResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "❌ VerifyProduct API response is null.");
                throw new Exception("VerifyProduct API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{JsonConvert.SerializeObject(_verifyProductResponse, Formatting.Indented)}");
        }

        [When("Send a request with valid BleId, Brand, private label code as zero, and isGenericFaceplate as true.")]
        public async Task WhenSendARequestWithValidBleIdBrandPrivateLabelCodeAsZeroAndIsGenericFaceplateAsTrue_Async(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            foreach(var row in dataTable.Rows)
            {
                int bleId = int.Parse(row["bleId"]);
                string brand = row["brand"];
                int privateLabelCode = int.Parse(row["privateLabelCode"]);
                bool isGenericFaceplate = bool.Parse(row["isGenericFaceplate"]);

                _verifyProductRequest = new VerifyProductRequest
                {
                    BleId = bleId,
                    Brand = brand,
                    PrivateLabel = privateLabelCode
                    // isGenericFaceplate not used in proto
                };
                ExtentReportManager.GetInstance().LogToReport(_step,Status.Info ,$"BleId: {bleId}, Brand: {brand}, PrivateLabel: {privateLabelCode}, IsGenericFaceplate: {isGenericFaceplate}");
            }
            _verifyProductResponse = await _productIdentificationPage.VerifyProductAsync(_verifyProductRequest);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request with valid BleId, correct Brand, private label code as zero, and isGenericFaceplate as true.");
        }
        [Then("API returns an error indicating that the BleId is invalid")]
        public void ThenAPIReturnsAnErrorIndicatingThatTheBleIdIsInvalid()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_verifyProductResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "❌ VerifyProduct API response is null.");
                throw new Exception("VerifyProduct API response is null.");
            }
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{JsonConvert.SerializeObject(_verifyProductResponse, Formatting.Indented)}");
        }

    }
}
