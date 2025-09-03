using System;
using System.Text.Json;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.SecurityCertificates;
using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using VoidResponse = Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse;

namespace QuantumServicesAPI.StepDefinitions.SecurityCertificates
{
    [Binding]
    public class SecurityCertificatesFailStepDefinitions: BaseResponsePage
    {
        protected string fdiPath1 = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound OMNIA 9\RU961-DRW [7]\Final\RU961-DRW.7.42.1.1.fdidfu";
       
        public SecurityCertificatesFailStepDefinitions(ScenarioContext scenarioContext): base(scenarioContext)
        {
           
        }

        [When("Load a valid image with a BleId that differs from the device’s BleId, and send the request to the ModelInPricePointCertificate API")]
        public async Task WhenLoadAValidImageWithABleIdThatDiffersFromTheDeviceSBleIdAndSendTheRequestToTheModelInPricePointCertificateAPIAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath1, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ModelInPricePointCertificate API when the device has an invalid Price Point Certificate...");
                _verifyModelInPricePointCertificateResponse = await _securityCertificatesPage.CallVerifyModelInPricePointCertificateAsync();

                if (_verifyModelInPricePointCertificateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PricePointCertificateValidity response is null");
                    throw new Exception("PricePointCertificateValidity response is null");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "VerifyModelInPricePointCertificate API returned:", _verifyModelInPricePointCertificateResponse.ToString());
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API returns status for ModelInPricePointCertificate {string}")]
        public void ThenAPIReturnsStatusForModelInPricePointCertificate(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_verifyModelInPricePointCertificateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "VerifyModelInPricePointCertificate response is null");
                throw new Exception("VerifyModelInPricePointCertificate response is null");
            }

            string actualStatus = _verifyModelInPricePointCertificateResponse.PricePointCertValidation.ToString();
            if (actualStatus != expectedStatus)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected {expectedStatus} but got {actualStatus}");
                throw new Exception($"Expected {expectedStatus} but got {actualStatus}");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status: {actualStatus}");
        }

        [When("Send a request to the DeviceFamilyCertificateValidity API when the device contains an invalid Device Family Certificate.")]
        public async Task WhenSendARequestToTheDeviceFamilyCertificateValidityAPIWhenTheDeviceContainsAnInvalidDeviceFamilyCertificate_Async(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling DeviceFamilyCertificateValidity API when the device contains an invalid Device Family Certificate...");

                _isFamilyCertificateValidResponse = await _securityCertificatesPage.CallIsFamilyCertificateValidAsync();
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "DeviceFamilyCertificateValidity API returned:", JsonSerializer.Serialize(_isFamilyCertificateValidResponse));
                if (_isFamilyCertificateValidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceFamilyCertificateValidity API returned null");
                    throw new Exception($"DeviceFamilyCertificateValidity API returned null");
                }
                SocketHelperClass.HandleProcessExit();
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }


        [Then("API returns status for DeviceFamilyCertificateValidity {string}")]
        public void ThenAPIReturnsStatusForDeviceFamilyCertificateValidity(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_isFamilyCertificateValidResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceFamilyCertificateValidity response is null");
                throw new Exception("DeviceFamilyCertificateValidity response is null");
            }

            string actualStatus = _isFamilyCertificateValidResponse.IsFamilyCertificateValid.ToString();

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected {expectedStatus} but got {actualStatus}");
                throw new Exception($"Expected {expectedStatus} but got {actualStatus}");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API returned expected status: {actualStatus}");
        }


        [When("Send a request to the PricePointCertificateValidity API when the device has an invalid Price Point Certificate")]
        public async Task WhenSendARequestToThePricePointCertificateValidityAPIWhenTheDeviceHasAnInvalidPricePointCertificateAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling PricePointCertificateValidity API...");

                _verifyModelInPricePointCertificateResponse = await _securityCertificatesPage.CallVerifyModelInPricePointCertificateAsync();

                if (_verifyModelInPricePointCertificateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PricePointCertificateValidity response is null");
                    throw new Exception("PricePointCertificateValidity response is null");
                }

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "PricePointCertificateValidity API returned:", _verifyModelInPricePointCertificateResponse.ToString());
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API returns status for PricePointCertificateValidity {string}")]
        public void ThenAPIReturnsStatusForPricePointCertificateValidity(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if (_verifyModelInPricePointCertificateResponse == null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "PricePointCertificateValidity response is null");
                throw new Exception("PricePointCertificateValidity response is null");
            }
            string actualStatus = (_verifyModelInPricePointCertificateResponse.PricePointCertValidation == PricePointCertValidation.ValidCertModelMatch).ToString();

            //string actualStatus = _verifyModelInPricePointCertificateResponse.PricePointCertValidation == PricePointCertValidation.ValidCertModelMatch
            //    ? "True"
            //    : "False";

            if (!string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail,$"Expected {expectedStatus} but got {actualStatus}");
                throw new Exception($"Expected {expectedStatus} but got {actualStatus}");
            }

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,$"API returned expected status: {actualStatus}");
        }
    }
}
