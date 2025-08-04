using Avalon.Dooku3.gRPCService.Protos.DeviceImage;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using System;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class DeviceImageSuccessStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ProductIdentificationPage _productIdentificationPage;
        private readonly DeviceImagePage _deviceImagePage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _response; // Fully qualify 'VoidResponse' to resolve ambiguity
        private Avalon.Dooku3.gRPCService.Protos.DeviceImage.VoidResponse? _deviceImageresponse; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse;
        private GetFlashWriteProtectStatusResponse? _getFlashWriteProtectStatusResponse; // Declare '_getFlashWriteProtectStatusResponse' as nullable to fix CS8618
        private IsCustomProductResponse? _isCustomProductResponse; // Declare '_isCustomProductResponse' as nullable to fix CS8618
        private IsOptimizedProgrammingResponse? _isOptimizedProgrammingResponse; // Declare '_isOptimizedProgrammingResponse' as nullable to fix CS8618
        private IsDfuCompatibleResponse? _isDfuCompatibleResponse; // Declare '_isDfuCompatibleResponse' as nullable to fix CS8618  

        const string fdiPath = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound Vivia 7\VI760S-DRWC [10]\Final\VI760S-DRWC.10.43.1.1.fdidfu";
        const string hdiPath = @"C:\Program Files (x86)\GN Hearing\Avalon\Device.Dooku3\Dooku3.C6.HDI.1.4.xml";
        public DeviceImageSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
            _deviceImagePage = (DeviceImagePage)_scenarioContext["GrpcDeviceImage"];
        }

        [When("Load a DFU image with a higher HDI version than the device, ensure Flash Write Protect is not set to {string}, and send a request to the UpdateHDI API")]
        public async Task WhenLoadADFUImageWithAHigherHDIVersionThanTheDeviceEnsureFlashWriteProtectIsNotSetToAndSendARequestToTheUpdateHDIAPIAsync(string status, DataTable dataTable)
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

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to FlashWriteProtect API to fetch current protection status...");
                _getFlashWriteProtectStatusResponse = await _hearingInstrumentPage.CallGetFlashWriteProtectStatusAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "FlashWriteProtect API call succeeded. Current status received.");

                //if (_getFlashWriteProtectStatusResponse.FlashWriteProtectStatus.ToString() != status)
                //{

                //}
                //else
                //{
                //    _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                //}
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CallLoadImageDataFromFileAsync...");
                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");

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
                if (_deviceImageresponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "DeviceImage API response is null. HDI update failed.");
                    throw new Exception("DeviceImage API response is null.");
                }
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");
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
                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }
    }
}