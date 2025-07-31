using System;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using AventStack.ExtentReports;
using QuantumServicesAPI.APIHelper;
using QuantumServicesAPI.ExtentReport;
using QuantumServicesAPI.Pages;
using Reqnroll;
using VoidResponse = Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse;

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class ProductIdentificationFailStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private VoidResponse? _response; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private DetectOnSideResponse? _oppositeSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse; // Declare '_connectResponse' as nullable to fix CS8618
        private ReadPcbaPartNumberResponse? _readPcbaPartNumberResponse; // Declare 'readPcbaPartNumberResponse' as nullable to fix CS8618
        private GetPlatformNameResponse? _getPlatformNameResponse;
        private GetSerialNumberResponse? _getSerialNumberResponse; // Declare 'getSerialNumberResponse' as nullable to fix CS8618
        private GetSideResponse? _getSideResponse; // Declare 'getSideResponse' as nullable to fix CS8618
        private GetNetworkAddressResponse? _getNetworkAddressResponse;
        private VerifyProductResponse? _verifyProductResponse; // Declare 'verifyProductResponse' as nullable to fix CS8618
        private ReadCloudRegistrationInputResponse? _readCloudRegistrationInputResponse;
        private GetDateModifiedResponse? _getDateModifiedResponse; // Declare 'getDateModifiedResponse' as nullable to fix CS8618
        private GetOptionsForDeviceResponse? _getOptionsForDeviceResponse; // Declare 'getOptionsForDeviceResponse' as nullable to fix CS8618
        private GetPrivateLabelCodeResponse? _getPrivateLabelCodeResponse; // Declare 'getPrivateLabelCodeResponse' as nullable to fix CS8618
        private Avalon.Dooku3.gRPCService.Protos.ProductIdentification.VoidResponse? _productresponse; // Declare '_response' as nullable to fix CS8618
        private readonly ProductIdentificationPage _productIdentificationPage;


        public ProductIdentificationFailStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
        }

        [Given("Send a request with valid BleId, Brand, and private label code as Non-Zero \\(Ex: {string})")]
        public async Task GivenSendARequestWithValidBleIdBrandAndPrivateLabelCodeAsNon_ZeroExAsync(string p0, DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");
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
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called ConnectToDevice successfully");
                _readPcbaPartNumberResponse = await _productIdentificationPage.CallReadPcbaPartNumberAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ReadPcbaPartNumber API call succeeded. PCBA part number retrieved successfully.");
                _productresponse = await _productIdentificationPage.CallReadAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Read API call succeeded. Product information retrieved successfully.");
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting VerifyProduct API requests with provided BleId, Brand, and PrivateLabelCode.");
                foreach (var row in dataTable.Rows)
                {
                    string bleId = row["BleId"];
                    string brand = row["Brand"];
                    string privateLabelCode = row["PrivateLabelCode"];

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info,
                        $"Calling VerifyProduct API with BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}...");

                    _verifyProductResponse = await _productIdentificationPage.CallVerifyProductAsync(
                        int.Parse(bleId), brand, int.Parse(privateLabelCode));

                    if (_verifyProductResponse == null)
                    {
                        string errorMsg = $"VerifyProduct API returned null for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.";
                        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
                        throw new Exception(errorMsg);
                    }

                    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
                        $"VerifyProduct API call succeeded for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.");
                }

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed VerifyProduct API requests for all provided rows.");
            }
            catch (Exception ex)
            {
                ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
                throw;
            }
        }

        [Then("API skips Brand verification")]
        public void ThenAPISkipsBrandVerification()
        {
            
        }

        [When("Send a request with valid BleId, Brand, private label code as {string}, and isGenericFaceplate as {string}")]
        public async Task WhenSendARequestWithValidBleIdBrandPrivateLabelCodeAsAndIsGenericFaceplateAsAsync(string p0, string @true, DataTable dataTable)
        {
            //_test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            //_step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text.ToString());
            //ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for ConnectToDevice API.");
            //SocketHelperClass.HandleProcessExit();
            //SocketHelperClass.SuccessSocketCommands();
            //try
            //{
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
            //    _response = await _hearingInstrumentPage.CallInitializeAsync();
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
            //    _response = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Product configured successfully using FDTS file.");
            //    foreach (var row in dataTable.Rows)
            //    {
            //        string serialNumber = row["SerialNumber"];
            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, $"Calling DetectBySerialNumber API for serial number: {serialNumber}...");
            //        _detectBySerialNumberResponse = await _hearingInstrumentPage.CallDetectBySerialNumberAsync(serialNumber);

            //        if (_detectBySerialNumberResponse == null)
            //        {
            //            ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"DetectBySerialNumber API returned null for serial number: {serialNumber}");
            //            throw new Exception($"DetectBySerialNumber response is null for serial number: {serialNumber}");
            //        }
            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"DetectBySerialNumber API succeeded for serial number: {serialNumber}.");
            //    }
            //    _detectClosestResponse = await _hearingInstrumentPage.CallDetectClosestAsync();
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called DetectClosest successfully");

            //    var left = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Left);
            //    var right = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Right);

            //    ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Left Side Response", left.ToString());
            //    ExtentReportManager.GetInstance().LogJson(_step, Status.Info, "Right Side Response", right.ToString());

            //    if (left.AvalonStatus == AvalonStatus.Success && right.AvalonStatus == AvalonStatus.Success)
            //    {
            //        _detectOnSideResponse = await _hearingInstrumentPage.CallDetectOnSideAsync(ChannelSide.Both);
            //        connectedSide = ChannelSide.Both;
            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Both sides detected successfully. Using 'Both' as fitting side.");
            //    }
            //    else if (left.AvalonStatus == AvalonStatus.Success)
            //    {
            //        _detectOnSideResponse = left;
            //        connectedSide = ChannelSide.Left;
            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Left side detected successfully.");
            //    }
            //    else if (right.AvalonStatus == AvalonStatus.Success)
            //    {
            //        _detectOnSideResponse = right;
            //        connectedSide = ChannelSide.Right;
            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Only Right side detected successfully.");
            //    }
            //    else
            //    {
            //        ExtentReportManager.GetInstance().LogError(_step, Status.Fail, "No connected side detected. Device may not be connected or powered.");
            //        throw new InvalidOperationException("No connected side found.");
            //    }
            //    _enableMasterConnectResponse = await _hearingInstrumentPage.CallEnableMasterConnectAsync(true);
            //    _enableFittingModeResponse = await _hearingInstrumentPage.CallEnableFittingModeAsync(true);
            //    _getDeviceNodeResponse = await _hearingInstrumentPage.CallGetDeviceNodeAsync();
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called GetDeviceNode successfully");
            //    _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Called ConnectToDevice successfully");
            //    _readPcbaPartNumberResponse = await _productIdentificationPage.CallReadPcbaPartNumberAsync();
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "ReadPcbaPartNumber API call succeeded. PCBA part number retrieved successfully.");
            //    _productresponse = await _productIdentificationPage.CallReadAsync();
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Read API call succeeded. Product information retrieved successfully.");
            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting VerifyProduct API requests with provided BleId, Brand, and PrivateLabelCode.");
            //    foreach (var row in dataTable.Rows)
            //    {
            //        string bleId = row["BleId"];
            //        string brand = row["Brand"];
            //        string privateLabelCode = row["PrivateLabelCode"];
            //        string IsGenericFaceplate = row["IsGenericFaceplate"];

            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Info,
            //            $"Calling VerifyProduct API with BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}...");

            //        _verifyProductResponse = await _productIdentificationPage.CallVerifyProductAsync(
            //            int.Parse(bleId), brand, int.Parse(privateLabelCode),bool.Parse(IsGenericFaceplate));

            //        if (_verifyProductResponse == null)
            //        {
            //            string errorMsg = $"VerifyProduct API returned null for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.";
            //            ExtentReportManager.GetInstance().LogError(_step, Status.Fail, errorMsg);
            //            throw new Exception(errorMsg);
            //        }

            //        ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass,
            //            $"VerifyProduct API call succeeded for BleId: {bleId}, Brand: {brand}, PrivateLabelCode: {privateLabelCode}.");
            //    }

            //    ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Completed VerifyProduct API requests for all provided rows.");
            //}
            //catch (Exception ex)
            //{
            //    ExtentReportManager.GetInstance().LogError(_step, Status.Fail, $"Exception occurred while verifying input values: {ex.Message}");
            //    throw;
            //}
        }

        [Then("API skips the Brand verification")]
        public void ThenAPISkipsTheBrandVerification()
        {
            
        }

    }
}
