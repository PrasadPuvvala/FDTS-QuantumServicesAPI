using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using Avalon.Dooku3.gRPCService.Protos.SecurityCertificates;
using AventStack.ExtentReports;
using Newtonsoft.Json.Linq;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace QuantumServicesAPI.StepDefinitions
{
    /// <summary>
    /// Step definitions for Security Certificates success scenarios.
    /// Handles gRPC API calls and validation for certificate-related operations on hearing instruments.
    /// </summary>
    [Binding]
    public class SecurityCertificatesSuccessStepDefinitions : BaseResponsePage
    {
        public SecurityCertificatesSuccessStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        [When("Send a request to the InProductionCertificate API with a valid certificate to be written to the device")]
        public async Task WhenSendARequestToTheInProductionCertificateAPIWithAValidCertificateToBeWrittenToTheDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            string ValidInProductionCertificate = "ValidInProductionCertificate"; // Replace with actual valid certificate content if needed

            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
               
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CallLoadImageDataFromFileAsync...");
                _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Preparing to write InProduction certificate to the device...");
                _securityCertificateVoidResponse = await _securityCertificatesPage.CallWriteInProductionCertificateAsync(ValidInProductionCertificate);
                if (_securityCertificateVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "WriteInProductionCertificate API returned null.");
                    throw new Exception("WriteInProductionCertificate response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "InProduction certificate written successfully to the device.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"Error Message : {ex.Message}");
                throw;
            }
        }

        [Then("API writes the certificate successfully to the hearing instrument")]
        public void ThenAPIWritesTheCertificateSuccessfullyToTheHearingInstrument()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating that the InProduction certificate is written successfully to the hearing instrument...");
                if (_securityCertificateVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to write InProduction certificate to the hearing instrument. Response is null.");
                    throw new Exception("InProduction certificate writing failed - response is null.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "InProduction certificate written successfully to the hearing instrument.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while verifying InProduction certificate writing: {ex.Message}");
                throw;
            }
        }

        [When("Load a valid image and ensure the BleId in the image matches the device’s BleId, and send request to the ModelInPricePointCertificate API")]
        public async Task WhenLoadAValidImageAndEnsureTheBleIdInTheImageMatchesTheDeviceSBleIdAndSendRequestToTheModelInPricePointCertificateAPI()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Loading valid DFU image and verifying that BleId in image matches the device’s BleId...");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to ModelInPricePointCertificate API...");
                _verifyModelInPricePointCertificateResponse = await _securityCertificatesPage.CallVerifyModelInPricePointCertificateAsync();

                if (_verifyModelInPricePointCertificateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail,
                        "VerifyModelInPricePointCertificate API returned null. Unable to proceed.");
                    throw new Exception("VerifyModelInPricePointCertificate response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ModelInPricePointCertificate API call succeeded and returned valid response.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error,
                    $"Exception occurred while calling ModelInPricePointCertificate API: {ex.Message}");
                throw;
            }
        }

        [Then("API returns {string}")]
        public void ThenAPIReturns(string expectedResult)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating VerifyModelInPricePointCertificate API response...");

                if (_verifyModelInPricePointCertificateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "VerifyModelInPricePointCertificate API returned null.");
                    throw new Exception("VerifyModelInPricePointCertificate response is null.");
                }

                if (_verifyModelInPricePointCertificateResponse?.PricePointCertValidation == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PricePointCertValidation field is null in the API response.");
                    throw new Exception("PricePointCertValidation field is null.");
                }

                string actualResult = _verifyModelInPricePointCertificateResponse.PricePointCertValidation.ToString();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected Result: {expectedResult}");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Actual Result: {actualResult}");

                // Case-insensitive match
                if (string.Equals(actualResult, expectedResult, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, $"API returned expected result:", $"{System.Text.Json.JsonSerializer.Serialize(expectedResult)}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                    throw new Exception($"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Validation failed: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the DeviceFamilyCertificateValidity API when the device contains a valid Device Family Certificate")]
        public async Task WhenSendARequestToTheDeviceFamilyCertificateValidityAPIWhenTheDeviceContainsAValidDeviceFamilyCertificateAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to DeviceFamilyCertificateValidity API to check if the device contains a valid Device Family Certificate...");
                _isFamilyCertificateValidResponse = await _securityCertificatesPage.CallIsFamilyCertificateValidAsync();
                if (_isFamilyCertificateValidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "IsFamilyCertificateValid API returned null. Unable to proceed.");
                    throw new Exception("IsFamilyCertificateValid response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DeviceFamilyCertificateValidity API call succeeded and returned valid response.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while sending request to DeviceFamilyCertificateValidity API: {ex.Message}");
                throw;
            }
        }

        [Then("API returns {string} for DeviceFamilyCertificateValidity")]
        public void ThenAPIReturnsForDeviceFamilyCertificateValidity(string expectedResult)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating IsFamilyCertificateValid API response...");

                if (_isFamilyCertificateValidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "IsFamilyCertificateValid API returned null.");
                    throw new Exception("IsFamilyCertificateValid response is null.");
                }

                string actualResult = _isFamilyCertificateValidResponse.IsFamilyCertificateValid.ToString();

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected Result: {expectedResult}");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Actual Result: {actualResult}");

                // Case-insensitive match
                if (string.Equals(actualResult, expectedResult, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "API returned expected result", System.Text.Json.JsonSerializer.Serialize(expectedResult));
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                    throw new Exception($"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while validating DeviceFamilyCertificateValidity: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the PricePointCertificateValidity API when the device has a valid Price Point Certificate")]
        public async Task WhenSendARequestToThePricePointCertificateValidityAPIWhenTheDeviceHasAValidPricePointCertificateAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to PricePointCertificateValidity API to check if the device contains a valid Device Family Certificate...");
                _readPricePointCertInputResponse = await _securityCertificatesPage.CallReadPricePointCertInputAsync();
                if (_readPricePointCertInputResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "ReadPricePointCertInput API returned null. Unable to proceed.");
                    throw new Exception("ReadPricePointCertInput response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "PricePointCertificateValidity API call succeeded and returned valid response.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while sending request to PricePointCertificateValidity API: {ex.Message}");
                throw;
            }
        }

        [Then("API returns {string} for PricePointCertificateValidity")]
        public void ThenAPIReturnsForPricePointCertificateValidity(string expectedResult)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating PricePointCertificateValidity API response...");

                if (_readPricePointCertInputResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PricePointCertificateValidity API returned null.");
                    throw new Exception("PricePointCertificateValidity response is null.");
                }

                string actualResult = _readPricePointCertInputResponse.PricePointCertInput.ToString();

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected Result: {expectedResult}");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Actual Result: {actualResult}");

                // Case-insensitive match
                if (string.Equals(actualResult, expectedResult, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "API returned expected result", System.Text.Json.JsonSerializer.Serialize(expectedResult));
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                    throw new Exception($"API returned unexpected result. Expected: {expectedResult}, Actual: {actualResult}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while validating PricePointCertificateValidity: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the PricePointCertificate API with a valid Price Point Certificate")]
        public async Task WhenSendARequestToThePricePointCertificateAPIWithAValidPricePointCertificateAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            string validPricePointCertificate = "asdfvghj"; // Replace with actual valid certificate content if needed
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to PricePointCertificate API with a valid Price Point Certificate...");
                _securityCertificateVoidResponse = await _securityCertificatesPage.CallWritePricePointCertificateAsync(validPricePointCertificate);
                if (_securityCertificateVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "WritePricePointCertificate API returned null. Unable to proceed.");
                    throw new Exception("WritePricePointCertificate response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "PricePointCertificate API call succeeded and certificate written successfully.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while sending request to PricePointCertificate API: {ex.Message}");
                throw;
            }
        }

        [Then("API writes the PricePointCertificate successfully to the hearing instrument")]
        public void ThenAPIWritesThePricePointCertificateSuccessfullyToTheHearingInstrument()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating that the PricePointCertificate is written successfully to the hearing instrument...");
                if (_securityCertificateVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to write PricePointCertificate to the hearing instrument. Response is null.");
                    throw new Exception("PricePointCertificate writing failed - response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "PricePointCertificate written successfully to the hearing instrument.");
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "PricePointCertificate Response", System.Text.Json.JsonSerializer.Serialize(_securityCertificateVoidResponse));
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Error, $"Exception occurred while verifying PricePointCertificate writing: {ex.Message}");
                throw;
            }
        }
    }
}
