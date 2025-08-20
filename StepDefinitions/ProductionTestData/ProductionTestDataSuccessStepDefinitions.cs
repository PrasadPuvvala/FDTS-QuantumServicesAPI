using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using AventStack.ExtentReports;
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
        public ProductionTestDataSuccessStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        [When("Send a request to the ProductionTestData API to read test date, site, station, TPI release code, and verification flags from the device")]
        public async Task WhenSendARequestToTheProductionTestDataAPIToReadTestDateSiteStationTPIReleaseCodeAndVerificationFlagsFromTheDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);

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
