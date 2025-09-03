using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace QuantumServicesAPI.StepDefinitions.ProductionTestData
{
    [Binding]
    public class ProductionTestDataSuccessStepDefinitions : BaseResponsePage
    {
        private readonly FeatureContext _featureContext;
        private gRPCDeviceInfo _gRPCDeviceInfo;
        public ProductionTestDataSuccessStepDefinitions(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext)
        {
            _featureContext = featureContext;
            _gRPCDeviceInfo = _featureContext.Get<gRPCDeviceInfo>("gRPCDeviceInfo");
        }

        [When("Send a request to the ProductionTestData API to read test date, site, station, TPI release code, and verification flags from the device")]
        public async Task WhenSendARequestToTheProductionTestDataAPIToReadTestDateSiteStationTPIReleaseCodeAndVerificationFlagsFromTheDeviceAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                var serialNumber = _gRPCDeviceInfo?.deviceSerialNumber?.SerialNumber;
                if (string.IsNullOrEmpty(serialNumber))
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Serial number is null or empty. Cannot call DetectBySerialNumber API.");
                    throw new ArgumentNullException(nameof(serialNumber), "Serial number must not be null or empty.");
                }
                await SetupGrpcPreconditionsAsync(serialNumber, _step);

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
        public async Task WhenSendARequestToTheProductionTestDataAPIToWriteTestDateSiteStationTPIReleaseCodeAndVerificationFlagsToTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                var testSite = _gRPCDeviceInfo?.productionTestDataInformation?.TestSite;
                var testStation = _gRPCDeviceInfo?.productionTestDataInformation?.TestStation;
                var tpiReleaseCode = _gRPCDeviceInfo?.productionTestDataInformation?.TPIReleaseCode;
                var year = _gRPCDeviceInfo?.productionTestDataInformation?.Year;
                var month = _gRPCDeviceInfo?.productionTestDataInformation?.Month;
                var day = _gRPCDeviceInfo?.productionTestDataInformation?.Day;
                var hour = _gRPCDeviceInfo?.productionTestDataInformation?.Hour;
                var minute = _gRPCDeviceInfo?.productionTestDataInformation?.Minute;
                var second = _gRPCDeviceInfo?.productionTestDataInformation?.Second;
                var modelVerificationId = _gRPCDeviceInfo?.productionTestDataInformation?.ModelVerificationId;

                if (string.IsNullOrEmpty(testSite) || string.IsNullOrEmpty(testStation) || string.IsNullOrEmpty(tpiReleaseCode) ||
                    string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(day) ||
                    string.IsNullOrEmpty(hour) || string.IsNullOrEmpty(minute) || string.IsNullOrEmpty(second) ||
                    string.IsNullOrEmpty(modelVerificationId))
                {
                    var missingFields = new System.Collections.Generic.List<string>();
                    if (string.IsNullOrEmpty(testSite)) missingFields.Add(nameof(testSite));
                    if (string.IsNullOrEmpty(testStation)) missingFields.Add(nameof(testStation));
                    if (string.IsNullOrEmpty(tpiReleaseCode)) missingFields.Add(nameof(tpiReleaseCode));
                    if (string.IsNullOrEmpty(year)) missingFields.Add(nameof(year));
                    if (string.IsNullOrEmpty(month)) missingFields.Add(nameof(month));
                    if (string.IsNullOrEmpty(day)) missingFields.Add(nameof(day));
                    if (string.IsNullOrEmpty(hour)) missingFields.Add(nameof(hour));
                    if (string.IsNullOrEmpty(minute)) missingFields.Add(nameof(minute));
                    if (string.IsNullOrEmpty(second)) missingFields.Add(nameof(second));
                    if (string.IsNullOrEmpty(modelVerificationId)) missingFields.Add(nameof(modelVerificationId));

                    var errorMessage = $"The following required production test data fields are missing or empty: {string.Join(", ", missingFields)}.";
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Setting test site = {testSite}");
                _productionTestDataResponse = await _productionTestDataPage.CallSetTestSiteAsync(testSite);
                if (_productionTestDataResponse == null)
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
                _productionTestDataResponse = await _productionTestDataPage.CallWriteAsync();
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
