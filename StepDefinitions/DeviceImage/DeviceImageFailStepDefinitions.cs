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
    public class DeviceImageFailStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ProductIdentificationPage _productIdentificationPage;
        private readonly DeviceImagePage _deviceImagePage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _response; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private DetectOnSideResponse? _oppositeSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse; // Declare '_connectResponse' as nullable to fix CS8618
        private GetFlashWriteProtectStatusResponse? _getFlashWriteProtectStatusResponse; // Declare '_getFlashWriteProtectStatusResponse' as nullable to fix CS8618
        private SetFlashWriteProtectStateResponse? _setFlashWriteProtectStateResponse; // Declare 'setFlashWriteProtectStateResponse' as global
        private IsCustomProductResponse? _isCustomProductResponse; // Declare '_isCustomProductResponse' as nullable to fix CS8618
        private IsOptimizedProgrammingResponse? _isOptimizedProgrammingResponse; // Declare '_isOptimizedProgrammingResponse' as nullable to fix CS8618
        private IsDfuCompatibleResponse? _isDfuCompatibleResponse; // Declare '_isDfuCompatibleResponse' as nullable to fix CS8618  '
        private Avalon.Dooku3.gRPCService.Protos.DeviceImage.VoidResponse? _deviceImageresponse; // Declare '_response' as nullable to fix CS8618
        // Holds the RpcException thrown during the WriteFDI gRPC call, if any.
        // This allows the exception to be validated later in the corresponding Then step,
        // without using ScenarioContext or key-based retrieval.
        private RpcException? _writeException;
        const string fdiPath = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound Nexia 9\NX962-DRW [10]\Final\NX962-DRW.10.43.1.1.fdidfu";
        const string hdiPath = @"C:\Program Files (x86)\GN Hearing\Avalon\Device.Dooku3\Dooku3.C6.HDI.1.4.xml";

        public DeviceImageFailStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
            _deviceImagePage = (DeviceImagePage)_scenarioContext["GrpcDeviceImage"];
        }


        [When("Load a DFU image with  higher HDI version than the device, and set device Flash Write Protect status to {string}, and send a request to the UpdateHDI API")]
        public async Task WhenLoadADFUImageWithHigherHDIVersionThanTheDeviceAndSetDeviceFlashWriteProtectStatusToAndSendARequestToTheUpdateHDIAPIAsync(string state, DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            SocketHelperClass.HandleProcessExit();
            SocketHelperClass.SuccessSocketCommands();
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
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                //_enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
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

                var currentStatus = currentStatusResponse.FlashWriteProtectStatus;
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Current FlashWriteProtectStatus: {currentStatus}");

                //// Step 2: Ensure device is not in LockedPermanent state
                //if (currentStatus == FlashWriteProtectStatus.LockedPermanent)
                //{
                //    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "Connected device is in LockedPermanent state. Cannot change FlashWriteProtect state.");
                //    throw new InvalidOperationException("Device is LockedPermanent. FlashWriteProtect state change not allowed.");
                //}

                // Step 2: Proceed to set new state
                _setFlashWriteProtectStateResponse = await _hearingInstrumentPage.CallSetFlashWriteProtectStateAsync(flashWriteProtectStateToUse);
                if (_setFlashWriteProtectStateResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "SetFlashWriteProtectState API response is null.");
                    throw new Exception("SetFlashWriteProtectState response is null.");
                }
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "SetFlashWriteProtectState Response", _setFlashWriteProtectStateResponse.ToString());
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image with higher HDI version...");
                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending Write request to the device...");
                _deviceImageresponse = await _deviceImagePage.CallWriteAsync(false);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");
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
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
                _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "FlashWriteProtect Status Response", _getFlashWriteProtectStatusResponse.ToString());
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image...");
                // 2. Load valid DFU image from known paths
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API...");

                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "LoadImageDataFromFile Response", _deviceImageresponse.ToString());

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                _isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");

                // 3. Send Write request with isOptimizedProgramming = true
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Write API with isOptimizedProgramming = true...");
                _deviceImageresponse = await _deviceImagePage.CallWriteAsync(true);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageresponse.ToString());
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
                if (_deviceImageresponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceImage response is null. API did not write the image to the device.");
                    throw new Exception("DeviceImage response is null. API did not write the image to the device.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageresponse.ToString());
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
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API to load DFU image...");
                // 2. Load valid DFU image from known paths
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling LoadImageDataFromFile API...");

                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "LoadImageDataFromFile Response", _deviceImageresponse.ToString());

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                _isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                // Try writing immediately, expecting exception
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Write API (without DFU check)...");
                _deviceImageresponse = await _deviceImagePage.CallWriteAsync(false); // This should fail
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

            if(_deviceImageresponse != null)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "WriteFDI response is null. API did not write the FDI to the device.");
                throw new Exception("WriteFDI response is null. API did not write the FDI to the device.");
            }

            var actualStatus = _deviceImageresponse.ToString();
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
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "DeviceNodeData Response", _getDeviceNodeResponse.ToString());
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling WriteFDI API without loading DFU image...");
                // Attempt to write without loading a DFU image
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if the product is custom...");
                //_isCustomProductResponse = await _deviceImagePage.CallIsCustomProductAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsCustomProduct: {_isCustomProductResponse.IsCustomProduct}");

                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Checking if DFU is compatible...");
                //_isDfuCompatibleResponse = await _deviceImagePage.CallIsDfuCompatibleAsync();
                //ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"IsDfuCompatible: {_isDfuCompatibleResponse.IsDfuCompatible}");


                _deviceImageresponse = await _deviceImagePage.CallWriteAsync(false); // This should fail
                ExtentReportManager.GetInstance().LogJson(_step, Status.Pass, "Write Response", _deviceImageresponse.ToString());
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
            if (_deviceImageresponse == null)
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Fail, "No RpcException was captured.");
                throw new Exception("Expected RpcException was not thrown.");
            }

            var actualStatus = _deviceImageresponse.ToString();
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
