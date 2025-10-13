using Avalon.Dooku3.gRPCService.Protos.Communication;
using Avalon.Dooku3.gRPCService.Protos.DeviceImage;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using Avalon.Dooku3.gRPCService.Protos.SecurityCertificates;
using AventStack.ExtentReports;
using QuantumServicesAPI.ExtentReport;

namespace QuantumServicesAPI.Pages
{
    public abstract class BaseResponsePage
    {
        protected readonly ScenarioContext _scenarioContext;
        protected ExtentTest? _test;
        protected ExtentTest? _step;
        protected readonly HearingInstrumentPage _hearingInstrumentPage;
        protected readonly ProductIdentificationPage _productIdentificationPage;
        protected readonly DeviceImagePage _deviceImagePage;
        protected readonly ProductionTestDataPage _productionTestDataPage;
        protected readonly SecurityCertificatesPage _securityCertificatesPage;
        protected readonly CommunicationPage _communicationPage;

        // Response from the gRPC service for hearing instrument operations
        protected Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _hearingInstrumentVoidResponse;
        protected ProductNameResponse? _productNameResponse; // Declare '_productNameResponse' as nullable to fix CS8618
        protected DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        protected DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        protected DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        protected ChannelSide connectedSide; // Declare 'connectedSide' as global
        protected EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        protected EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        protected GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618
        protected ConnectResponse? _connectResponse;
        protected IsDeviceConnectedResponse? _isDeviceConnectedResponse;
        protected GetBootModeResponse? _getBootModeResponse;
        protected GetFlashWriteProtectStatusResponse? _getFlashWriteProtectStatusResponse; // Declare '_getFlashWriteProtectStatusResponse' as nullable to fix CS8618
        protected SetFlashWriteProtectStateResponse? _setFlashWriteProtectStateResponse; // Declare 'setFlashWriteProtectStateResponse' as global
        protected IsRechargeableResponse? _isRechargeableResponse; // Declare '_isRechargeableResponse' as nullable to fix CS8618
        protected GetBatteryLevelResponse? _getBatteryLevelResponse; // Declare '_getBatteryLevelResponse' as nullable to fix CS8618
        protected ShouldVerifyMfiChipResponse? _shouldVerifyMfiChipResponse;
        protected GetBatteryTypeResponse? _getBatteryTypeResponse;
        protected GetBatteryVoltageResponse? _getBatteryVoltageResponse;

        // Response from the gRPC service for product identification operations
        protected Avalon.Dooku3.gRPCService.Protos.ProductIdentification.VoidResponse? _productIdentificationVoidResponse;
        protected ReadPcbaPartNumberResponse? _readPcbaPartNumberResponse; // Declare 'readPcbaPartNumberResponse' as nullable to fix CS8618
        protected ReadInProductionCertInputResponse? _readInProductionCertInputResponse; // Declare 'readInProductionCertInputResponse' as nullable to fix CS8618
        protected GetPlatformNameResponse? _getPlatformNameResponse;
        protected GetSerialNumberResponse? _getSerialNumberResponse; // Declare 'getSerialNumberResponse' as nullable to fix CS8618
        protected GetSideResponse? _getSideResponse; // Declare 'getSideResponse' as nullable to fix CS8618
        protected GetNetworkAddressResponse? _getNetworkAddressResponse;
        protected VerifyProductResponse? _verifyProductResponse; // Declare 'verifyProductResponse' as nullable to fix CS8618
        protected ReadCloudRegistrationInputResponse? _readCloudRegistrationInputResponse;
        protected GetDateModifiedResponse? _getDateModifiedResponse; // Declare 'getDateModifiedResponse' as nullable to fix CS8618
        protected GetOptionsForDeviceResponse? _getOptionsForDeviceResponse; // Declare 'getOptionsForDeviceResponse' as nullable to fix CS8618
        protected GetPrivateLabelCodeResponse? _getPrivateLabelCodeResponse; // Declare 'getPrivateLabelCodeResponse' as nullable to fix CS8618

        // Response from the gRPC service for device image operations
        protected Avalon.Dooku3.gRPCService.Protos.DeviceImage.VoidResponse? _deviceImageVoidResponse;
        protected IsCustomProductResponse? _isCustomProductResponse; // Declare '_isCustomProductResponse' as nullable to fix CS8618
        protected IsOptimizedProgrammingResponse? _isOptimizedProgrammingResponse; // Declare '_isOptimizedProgrammingResponse' as nullable to fix CS8618
        protected IsDfuCompatibleResponse? _isDfuCompatibleResponse; // Declare '_isDfuCompatibleResponse' as nullable to fix CS8618
        protected WriteResponse? _writeResponse; // Declare '_writeResponse' as nullable to fix CS8618

        // Response from the gRPC service for production test data operations
        protected Avalon.Dooku3.gRPCService.Protos.ProductionTestData.VoidResponse? _productionTestDataResponse;
        protected GetTestSiteResponse? _getTestSiteResponse;
        protected GetTestStationResponse? _getTestStationResponse;
        protected GetTpiReleaseCodeResponse? _getTpiReleaseCodeResponse;
        protected GetTestDateResponse? _getTestDateResponse;
        protected GetModelVerificationIdResponse? _getModelVerificationIdResponse;

        // Response from the gRPC service for security certificates operations
        protected Avalon.Dooku3.gRPCService.Protos.SecurityCertificates.VoidResponse? _securityCertificateVoidResponse;
        protected VerifyModelInPricePointCertificateResponse? _verifyModelInPricePointCertificateResponse; // Declare 'verifyModelInPricePointCertificateResponse' as nullable to fix CS8618
        protected IsFamilyCertificateValidResponse? _isFamilyCertificateValidResponse; // Declare 'isFamilyCertificateValidResponse' as nullable to fix CS8618
        protected ReadPricePointCertInputResponse? _readPricePointCertInputResponse; // Declare 'readPricePointCertInputResponse' as nullable to fix CS8618

        // Response from the gRPC service for Communication operations
        protected Avalon.Dooku3.gRPCService.Protos.Communication.VoidResponse? _communicationVoidResponse;
        protected DeviceIdList? _deviceIdListResponse; // Declare '_deviceIdListResponse' as nullable to fix CS8618
        protected GetCommunicationDeviceIdResponse? _getCommunicationDeviceIdResponse; // Declare '_getCommunicationDeviceIdResponse' as nullable to fix CS8618

        protected string fdiPath = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound Nexia 9\NX962-DRW [10]\Final\NX962-DRW.10.43.1.1.fdidfu";
        protected string hdiPath = @"C:\Program Files (x86)\GN Hearing\Avalon\Device.Dooku3\Dooku3.C6.HDI.1.4.xml";
        protected BaseResponsePage(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
            _deviceImagePage = (DeviceImagePage)_scenarioContext["GrpcDeviceImage"];
            _productionTestDataPage = (ProductionTestDataPage)_scenarioContext["GrpcProductionTestData"];
            _securityCertificatesPage = (SecurityCertificatesPage)_scenarioContext["GrpcSecurityCertificates"];
            _communicationPage = (CommunicationPage)_scenarioContext["GrpcCommunication"];
        }
        protected async Task SetupGrpcPreconditionsAsync(string serialNumber, ExtentTest step)
        {
            ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Starting device initialization and product configuration for serial number detection.");

            try
            {
                _deviceIdListResponse = await _communicationPage.CallGetAvailableCommunicationDevicesAsync();

                var deviceId = _deviceIdListResponse.DeviceIds.ToList().FirstOrDefault();

                _communicationVoidResponse = await _communicationPage.CallSetCommunicationDeviceIdAsync(deviceId!);
                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Calling Initialize API to initialize the device...");
                _hearingInstrumentVoidResponse = await _hearingInstrumentPage.CallInitializeAsync(deviceId!);
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _hearingInstrumentVoidResponse = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Product configured successfully using FDTS file.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
                _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

                if (_detectBySerialNumberResponse == null)
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
                    throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
                }
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Calling DetectClosest API to find the nearest RHI device...");
                _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "DetectClosest API call succeeded.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Calling DetectOnSide API for Left and Right channels...");
                var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
                var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

                ExtentReportManager.GetInstance().LogJson(step, Status.Info, "DetectOnSide Left Response", left.ToString());
                ExtentReportManager.GetInstance().LogJson(step, Status.Info, "DetectOnSide Right Response", right.ToString());

                if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
                {
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Both Left and Right sides detected. Calling DetectOnSide API for Both sides...");
                    _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
                    connectedSide = ChannelSide.Both;
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
                }
                else if (left.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = left;
                    connectedSide = ChannelSide.Left;
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Only Left side detected successfully. Using 'Left' as fitting side.");
                }
                else if (right.AvalonStatus == AvalonStatus.Success)
                {
                    _detectOnSideResponse = right;
                    connectedSide = ChannelSide.Right;
                    ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Only Right side detected successfully. Using 'Right' as fitting side.");
                }
                else
                {
                    ExtentReportManager.GetInstance().LogError(step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
                    throw new InvalidOperationException("No connected side found.");
                }

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Enabling Master Connect mode...");
                _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Master Connect mode enabled.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Enabling Fitting Mode...");
                _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Fitting Mode enabled.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Requesting device node data from GetDeviceNode API...");
                _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Device node data received.");

                ExtentReportManager.GetInstance().LogToReport(step, Status.Info, "Attempting to connect to device using Connect API...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogToReport(step, Status.Pass, "Connected to device successfully.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(step, Status.Fail, $"{ex.Message}");
                throw;
            }
        }
    }
}
