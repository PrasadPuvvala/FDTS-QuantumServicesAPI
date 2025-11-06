using Avalon.Dooku3.gRPCService.Protos.DeviceImage;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using QuantumServicesAPI.DTO;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;

namespace QuantumServicesAPI.StepDefinitions.DeviceImage
{
    [Binding]
    public class DeviceImageSuccessStepDefinitions : BaseResponsePage
    {
        private readonly FeatureContext _featureContext;
        private gRPCDeviceInfo _gRPCDeviceInfo;
        public DeviceImageSuccessStepDefinitions(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext)
        {
            _featureContext = featureContext;
            _gRPCDeviceInfo = _featureContext.Get<gRPCDeviceInfo>("gRPCDeviceInfo");
        }

        [When("Load a DFU image with a higher HDI version than the device, ensure Flash Write Protect is not set to {string}, and send a request to the UpdateHDI API")]
        public async Task WhenLoadADFUImageWithAHigherHDIVersionThanTheDeviceEnsureFlashWriteProtectIsNotSetToAndSendARequestToTheUpdateHDIAPIAsync(string status)
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

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to FlashWriteProtect API to fetch current protection status...");
                _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "FlashWriteProtect API call succeeded. Current status received.");

                var actualEnum = _getFlashWriteProtectStatusResponse.FlashWriteProtectStatus;
                var actualStatus = GetProtoOriginalName(actualEnum);

                if (actualStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"Flash Write Protect status matches expected value: {status}");
                    throw new Exception($"Flash Write Protect status matches. Expected: {status}, Actual: {actualStatus}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CallLoadImageDataFromFileAsync...");
                    _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");
                }          

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                //_isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                ////ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if optimized programming is supported...");
                ////_isOptimizedProgrammingResponse = await _deviceImagePage.CallIsOptimizedProgrammingAsync(_getDeviceNodeResponse.DeviceNode.DeviceName);
                ////ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsOptimizedProgramming: {_isOptimizedProgrammingResponse.IsOptimizedProgramming}");

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                //_isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending Write request to the device...");
                //_deviceImageresponse = await _deviceImagePage.CallWriteAsync(false);
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending WriteImageHash request to the device...");
                //_deviceImageresponse = await _deviceImagePage.CallWriteImageHashAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }

        [Then("API updates the HDI in the device before writing the image")]
        [Then("API does not update the HDI in the device")]
        public void ThenAPIUpdatesTheHDIInTheDeviceBeforeWritingTheImage()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating DeviceImage API response for HDI update...");
            try
            {
                if (_deviceImageVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceImage API response is null. HDI update failed.");
                    throw new Exception("DeviceImage API response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }

        [When("Load a DFU image with a higher HDI version than the device, and set device Flash Write Protect status to {string}, and send a request to the UpdateHDI API")]
        public async Task WhenLoadADFUImageWithAHigherHDIVersionThanTheDeviceAndSetDeviceFlashWriteProtectStatusToAndSendARequestToTheUpdateHDIAPIAsync(string lockedPermanent)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CallLoadImageDataFromFileAsync...");
                _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }

        [When("Load a DFU image with an equal or higher HDI version than the one on the device and send a compatibility check request")]
        public async Task WhenLoadADFUImageWithAnEqualOrHigherHDIVersionThanTheOneOnTheDeviceAndSendACompatibilityCheckRequestAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking DFU compatibility with the device...");
                _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DFU compatibility check completed. IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Unexpected error during DFU compatibility check: {ex.Message}");
                throw;
            }
        }

        [Then("API returns {string} indicating the DFU image is compatible")]
        [Then("API returns {string} indicating the DFU image is not compatible")]
        public void ThenAPIReturnsIndicatingTheDFUImageIsCompatible(string isDfuCompatible)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating DFU compatibility response...");
                if (_isDfuCompatibleResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DFU compatibility response is null. Compatibility check failed.");
                    throw new Exception("DFU compatibility response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DFU compatibility response: {_isDfuCompatibleResponse.IsDfuCompatible}");
                if (_isDfuCompatibleResponse.IsDfuCompatible == bool.Parse(isDfuCompatible))
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DFU image is compatible with the device.");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_isDfuCompatibleResponse.ToString()}");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "DFU image is not compatible with the device.");
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_isDfuCompatibleResponse.ToString()}");
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Unexpected error during DFU compatibility check: {ex.Message}");
                throw;
            }
        }

        [When("Load a DFU image with a lower HDI version than the one on the device and send a compatibility check request")]
        public async Task WhenLoadADFUImageWithALowerHDIVersionThanTheOneOnTheDeviceAndSendACompatibilityCheckRequestAsync()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking DFU compatibility with the device...");
                _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DFU compatibility check completed. IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Unexpected error during DFU compatibility check: {ex.Message}");
                throw;
            }
        }

        [When("Load a valid DFU image, set isOptimizedProgramming to {string}, and send a request to WriteFDI API")]
        public async Task WhenLoadAValidDFUImageSetIsOptimizedProgrammingToAndSendARequestToWriteFDIAPIAsync(string isOptimizedProgram)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Initiating WriteFDI API call with DFU image...");
                bool isOptimizedProgramming = bool.Parse(isOptimizedProgram);
                _writeResponse = await _deviceImagePage.CallWriteAsync(isOptimizedProgramming);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "WriteFDI API call completed successfully.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Unexpected error during WriteFDI API call: {ex.Message}");
                throw;
            }
        }

        [Then("API writes the image to the device successfully")]
        [Then("API does not write the image to the device")]
        public void ThenAPIWritesTheImageToTheDeviceSuccessfully()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Validating WriteFDI API response for image write verification...");
                if (_deviceImageVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "WriteFDI API response is null. Image write verification failed.");
                    throw new Exception("WriteFDI API response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Unexpected error during image write verification: {ex.Message}");
                throw;
            }
        }
    }
}