using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class ProductionTestDataSuccessStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ProductionTestDataPage _productionTestDataPage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _response;
        private Avalon.Dooku3.gRPCService.Protos.ProductionTestData.VoidResponse? _productionTestDataResponse;
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse;
        private GetTestSiteResponse? _getTestSiteResponse;
        private GetTestStationResponse? _getTestStationResponse;
        private GetTpiReleaseCodeResponse? _getTpiReleaseCodeResponse;
        private GetTestDateResponse? _getTestDateResponse;
        private GetModelVerificationIdResponse? _getModelVerificationIdResponse;
        public ProductionTestDataSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productionTestDataPage = (ProductionTestDataPage)_scenarioContext["GrpcProductionTestData"];
        }

        [When("Send a request to the ProductionTestData API to read test date, site, station, TPI release code, and verification flags from the device")]
        public async Task WhenSendARequestToTheProductionTestDataAPIToReadTestDateSiteStationTPIReleaseCodeAndVerificationFlagsFromTheDeviceAsync(DataTable dataTable)
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
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_getDeviceNodeResponse.ToString()}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Attempting to connect to device using Connect API...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Connected to device successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Reading production test data to the device...");
                _productionTestDataResponse = await _productionTestDataPage.CallReadAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Production test data read successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Get test site...");
                _getTestSiteResponse = await _productionTestDataPage.CallGetTestSiteAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test site retrieved successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Get test station...");
                _getTestStationResponse = await _productionTestDataPage.CallGetTestStationAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test station retrieved successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Get TPI release code...");
                _getTpiReleaseCodeResponse = await _productionTestDataPage.CallGetTpiReleaseCodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "TPI release code retrieved successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Get test date...");
                _getTestDateResponse = await _productionTestDataPage.CallGetTestDateAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test date retrieved successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Get model verification ID...");
                _getModelVerificationIdResponse = await _productionTestDataPage.CallGetModelVerificationIdAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Model verification ID retrieved successfully.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }

        [Then("API returns all stored production test metadata correctly from the hearing instrument")]
        public void ThenAPIReturnsAllStoredProductionTestMetadataCorrectlyFromTheHearingInstrument()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                if (_getTestSiteResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetTestSiteResponse is null.");
                    throw new Exception("GetTestSiteResponse is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Verifying test metadata values...");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Test Site:", $"{System.Text.Json.JsonSerializer.Serialize(_getTestSiteResponse)}");
                if (string.IsNullOrEmpty(_getTestStationResponse?.TestStation))
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Test Station is empty.");
                    throw new Exception("Test Station is empty.");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Test Station:", $"{System.Text.Json.JsonSerializer.Serialize(_getTestStationResponse)}");
                if (string.IsNullOrEmpty(_getTpiReleaseCodeResponse?.TpiReleaseCode))
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "TPI Release Code is empty.");
                    throw new Exception("TPI Release Code is empty.");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "TPI Release Code:", $"{System.Text.Json.JsonSerializer.Serialize(_getTpiReleaseCodeResponse)}");
                if (_getTestDateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetTestDateResponse is null.");
                    throw new Exception("GetTestDateResponse is null.");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Test Date:", $"{System.Text.Json.JsonSerializer.Serialize(_getTestDateResponse)}");
                if (_getModelVerificationIdResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "GetModelVerificationIdResponse is null.");
                    throw new Exception("GetModelVerificationIdResponse is null.");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Model Verification Id:", $"{System.Text.Json.JsonSerializer.Serialize(_getModelVerificationIdResponse)}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Error, $"An error occurred while verifying the test metadata: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the ProductionTestData API to write test date, site, station, TPI release code, and verification flags to the device")]
        public async Task WhenSendARequestToTheProductionTestDataAPIToWriteTestDateSiteStationTPIReleaseCodeAndVerificationFlagsToTheDevice(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                foreach (var row in dataTable.Rows)
                {
                    string testSite = row["TestSite"];
                    string testStation = row["TestStation"];
                    string tpiReleaseCode = row["TPIReleaseCode"];
                    string year = row["Year"];
                    string month = row["Month"];
                    string day = row["Day"];
                    string hour = row["Hour"];
                    string minute = row["Minute"];
                    string second = row["Second"];
                    string modelVerificationId = row["ModelVerificationId"];
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting test site = {testSite}");
                    _productionTestDataResponse = await _productionTestDataPage.CallSetTestSiteAsync(testSite);
                    if(_productionTestDataResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetTestSiteResponse is null.");
                        throw new Exception("SetTestSiteResponse is null.");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test site set successfully.");
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting test station = {testStation}");
                    _productionTestDataResponse = await _productionTestDataPage.CallSetTestStationAsync(testStation);
                    if (_productionTestDataResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetTestStationResponse is null.");
                        throw new Exception("SetTestStationResponse is null.");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test station set successfully.");
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting TPI release code = {tpiReleaseCode}");
                    _productionTestDataResponse = await _productionTestDataPage.CallSetTpiReleaseCodeAsync(tpiReleaseCode);
                    if (_productionTestDataResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetTpiReleaseCodeResponse is null.");
                        throw new Exception("SetTestStationResponse is null.");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "TPI release code set successfully.");
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting test date = {year}-{month}-{day} {hour}:{minute}:{second}");
                    _productionTestDataResponse = await _productionTestDataPage.CallSetTestDataAsync(year, month, day, hour, minute, second);
                    if (_productionTestDataResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetTestDataResponse is null.");
                        throw new Exception("SetTestDataResponse is null.");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Test date set successfully.");
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting Model Verification ID = {modelVerificationId}");
                    _productionTestDataResponse = await _productionTestDataPage.CallSetModelVerificationIdAsync(modelVerificationId);
                    if (_productionTestDataResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetModelVerificationIdResponse is null.");
                        throw new Exception("SetModelVerificationIdResponse is null.");
                    }
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Model Verification ID set successfully.");
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }

        [Then("API writes all provided test metadata values successfully to the hearing instrument")]
        public async Task ThenAPIWritesAllProvidedTestMetadataValuesSuccessfullyToTheHearingInstrument()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Verifying written test metadata values...");
                _productionTestDataResponse =await _productionTestDataPage.CallWriteAsync();
                if (_productionTestDataResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Write response is null.");
                    throw new Exception("Write response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Write operation completed successfully.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Response", $"{System.Text.Json.JsonSerializer.Serialize(_productionTestDataResponse)}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Error, $"An error occurred while verifying the written test metadata: {ex.Message}");
                throw;
            }
        }
    }
}
