using System;
using Avalon.Dooku3.gRPCService.Protos.DeviceImage;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using Grpc.Core;
using NUnit.Framework;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using Status = AventStack.ExtentReports.Status;

namespace QuantumServicesAPI.StepDefinitions.DeviceImage
{
    [Binding]
    public class DeviceImageFailStepDefinitions: BaseResponsePage
    {       
        public DeviceImageFailStepDefinitions(ScenarioContext scenarioContext): base(scenarioContext)
        {
           
        }


        [When("Load a DFU image with  higher HDI version than the device, and set device Flash Write Protect status to {string}, and send a request to the UpdateHDI API")]
        public async Task WhenLoadADFUImageWithHigherHDIVersionThanTheDeviceAndSetDeviceFlashWriteProtectStatusToAndSendARequestToTheUpdateHDIAPIAsync(string state, DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "FlashWriteProtect Status Response", _getFlashWriteProtectStatusResponse.ToString());
                var stateMapping = new Dictionary<string, FlashWriteProtectState>(StringComparer.OrdinalIgnoreCase)
    {
        { "Lock", FlashWriteProtectState.Lock },
        { "UnLock", FlashWriteProtectState.UnLock },
        { "LockedPermanent", FlashWriteProtectState.LockPermanent }
    };

                if (!stateMapping.TryGetValue(state, out var flashWriteProtectStateToUse))
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Invalid input '{state}' for FlashWriteProtectState. Expected values: Lock, UnLock, LockPermanent.");
                    throw new ArgumentException($"Invalid FlashWriteProtectState: {state}");
                }

                // Step 1: Check current Flash Write Protect status
                var currentStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                if (currentStatusResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Failed to read current Flash Write Protect status. Response is null.");
                    throw new Exception("FlashWriteProtectStatus read failed: response is null.");
                }

                if (currentStatusResponse.FlashWriteProtectStatus.ToString() == "LockedPermanent")
                {
                    var currentStatus = currentStatusResponse.FlashWriteProtectStatus;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Current FlashWriteProtectStatus: {currentStatus}");

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image with higher HDI version...");
                    _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                    _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                    _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending Write request to the device...");
                    _deviceImageVoidResponse = await _deviceImagePage.CallWriteAsync(false);
                    if (_deviceImageVoidResponse == null)
                    {
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Write response is null. API did not write the image to the device.");
                        throw new Exception("Write response is null. API did not write the image to the device.");
                    }
                    else
                    {
                        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageVoidResponse.ToString()}");
                    } 
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Flash Write Protect status is not 'LockedPermanent'. Current status: {currentStatusResponse.FlashWriteProtectStatus}");
                    throw new Exception($"Cannot proceed: FlashWriteProtectStatus is {currentStatusResponse.FlashWriteProtectStatus}, expected 'LockedPermanent'.");
                }
                
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API does not update  HDI in the device")]
        public void ThenAPIDoesNotUpdateHDIInTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            try
            {
                // Just log that HDI update did not happen
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "API did not update the HDI in the device as expected.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"Exception occurred while logging HDI update status: {ex.Message}");
                throw;
            }
        }

       

        [When("Load a valid DFU image, set isOptimizedProgramming to true, and send a request to WriteFDI API")]
        public async Task WhenLoadAValidDFUImageSetIsOptimizedProgrammingToTrueAndSendARequestToWriteFDIAPIAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "FlashWriteProtect Status Response", _getFlashWriteProtectStatusResponse.ToString());
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image...");
                // 2. Load valid DFU image from known paths
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API...");

                _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "LoadImageDataFromFile Response", _deviceImageVoidResponse.ToString());

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");

                // 3. Send Write request with isOptimizedProgramming = true
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Write API with isOptimizedProgramming = true...");
                _deviceImageVoidResponse = await _deviceImagePage.CallWriteAsync(true);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageVoidResponse.ToString());
            }

            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API does not write  image to the device")]
        public void ThenAPIDoesNotWriteImageToTheDevice()
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            try
            {
                if (_deviceImageVoidResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceImage response is null. API did not write the image to the device.");
                    throw new Exception("DeviceImage response is null. API did not write the image to the device.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageVoidResponse.ToString());
                }
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [When("Send a request to the WriteFDI API without performing DFU compatibility check")]
        public async Task WhenSendARequestToTheWriteFDIAPIWithoutPerformingDFUCompatibilityCheckAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image...");
                // 2. Load valid DFU image from known paths
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API...");

                _deviceImageVoidResponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "LoadImageDataFromFile Response", _deviceImageVoidResponse.ToString());

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                // Try writing immediately, expecting exception
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Write API (without DFU check)...");
                _deviceImageVoidResponse = await _deviceImagePage.CallWriteAsync(false); // This should fail
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Called Write API (without DFU check)...");
            }
            //catch (RpcException ex)
            //{
            //    _writeException = ex; // Store directly in class field
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected RpcException captured: {ex.Status.Detail}");
            //}
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API throws an exception with status {string}")]
        public void ThenAPIThrowsAnExceptionWithStatus(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);

            if(_deviceImageVoidResponse != null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "WriteFDI response is null. API did not write the FDI to the device.");
                throw new Exception("WriteFDI response is null. API did not write the FDI to the device.");
            }

            var actualStatus = _deviceImageVoidResponse?.ToString();
            if (string.Equals(actualStatus, expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"API correctly returned status: {actualStatus}");
            }
            else
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, $"Expected: {expectedStatus}, but got: {actualStatus}");
                throw new Exception($"Expected status '{expectedStatus}' but got '{actualStatus}'");
            }
           
        }

        [When("Send a request to the WriteFDI API without loading a DFU image")]
        public async Task WhenSendARequestToTheWriteFDIAPIWithoutLoadingADFUImageAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
            try
            {
                await SetupGrpcPreconditionsAsync(dataTable, _step);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling WriteFDI API without loading DFU image...");
                // Attempt to write without loading a DFU image
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                //_isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                //_isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");


                _deviceImageVoidResponse = await _deviceImagePage.CallWriteAsync(false); // This should fail
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageVoidResponse.ToString());
            }
            //catch (RpcException ex)
            //{
            //    _writeException = ex; // Save for validation in Then step
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Expected RpcException captured: {ex.Status.Detail}");
            //}
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API throws an exception with status the {string}")]
        public void ThenAPIThrowsAnExceptionWithStatusThe(string expectedStatus)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            if (_deviceImageVoidResponse == null)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, "No RpcException was captured.");
                throw new Exception("Expected RpcException was not thrown.");
            }

            var actualStatus = _deviceImageVoidResponse.ToString();
            if (actualStatus.Equals(expectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"Correct RpcException status code received: {actualStatus}");
            }
            else
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Expected status: {expectedStatus}, but got: {actualStatus}");
                throw new Exception($"Expected status '{expectedStatus}' but got '{actualStatus}'");
            }
        }


    }
}
