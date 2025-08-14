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

namespace QuantumServicesAPI.StepDefinitions
{
    [Binding]
    public class SecurityCertificatesFailStepDefinitions
    {

        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly ProductIdentificationPage _productIdentificationPage;
        private readonly SecurityCertificatesPage _securityCertificatesPage;
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
        private ConnectResponse? _connectResponse;
        private VerifyModelInPricePointCertificateResponse? _verifyModelInPricePointCertificateResponse;
        private Avalon.Dooku3.gRPCService.Protos.DeviceImage.VoidResponse? _deviceImageresponse; // Declare '_response' as nullable to fix CS8618
        private readonly DeviceImagePage _deviceImagePage;
        private IsFamilyCertificateValidResponse _isFamilyCertificateValidResponse;
        private PricePointCertValidation _pricePointCertValidation;
        const string fdiPath = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound Vivia 9\VI962-DRW [10]\Final\VI962-DRW.10.43.1.1.fdidfu";
        const string hdiPath = @"C:\Program Files (x86)\GN Hearing\Avalon\Device.Dooku3\Dooku3.C6.HDI.1.4.xml";

        public SecurityCertificatesFailStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _productIdentificationPage = (ProductIdentificationPage)_scenarioContext["GrpcProductIdentification"];
            _securityCertificatesPage = (SecurityCertificatesPage)_scenarioContext["GrpcSecurityCertificates"];
            _deviceImagePage = (DeviceImagePage)_scenarioContext["GrpcDeviceImage"];
        }

        [When("Load a valid image with a BleId that differs from the device’s BleId, and send the request to the ModelInPricePointCertificate API")]
        public async Task WhenLoadAValidImageWithABleIdThatDiffersFromTheDeviceSBleIdAndSendTheRequestToTheModelInPricePointCertificateAPIAsync(DataTable dataTable)
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
                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");
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
        [Then("API returns {string}")]
        public void ThenAPIReturns(string expectedStatus)
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
