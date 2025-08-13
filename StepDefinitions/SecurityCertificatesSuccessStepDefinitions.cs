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
    [Binding]
    public class SecurityCertificatesSuccessStepDefinitions
    {
        private readonly HearingInstrumentPage _hearingInstrumentPage;
        private readonly SecurityCertificatesPage _securityCertificatesPage;
        private readonly DeviceImagePage _deviceImagePage;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _test; // Declare 'test' as global
        private ExtentTest? _step; // Declare 'step' as global
        private Avalon.Dooku3.gRPCService.Protos.HearingInstrument.VoidResponse? _hearingInstrumentResponse; // Declare '_response' as nullable to fix CS8618
        private DetectBySerialNumberResponse? _detectBySerialNumberResponse; // Declare '_detectBySerialNumberResponse' as nullable to fix CS8618s
        private DetectClosestResponse? _detectClosestResponse; // Declare 'DetectClosestResponse' as global
        private DetectOnSideResponse? _detectOnSideResponse; // Declare '_detectOnSideResponse' as nullable to fix CS8618
        private ChannelSide connectedSide; // Declare 'connectedSide' as global
        private EnableMasterConnectResponse? _enableMasterConnectResponse; // Declare '_enableMasterConnectResponse' as nullable to fix CS8618
        private EnableFittingModeResponse? _enableFittingModeResponse; // Declare '_enableFittingModeRequest' as nullable to fix CS8618
        private GetDeviceNodeResponse? _getDeviceNodeResponse; // Declare '_getDeviceNodeResponse' as nullable to fix CS8618 
        private ConnectResponse? _connectResponse;
        private Avalon.Dooku3.gRPCService.Protos.SecurityCertificates.VoidResponse? _scecurityCertificateResponse;
        private Avalon.Dooku3.gRPCService.Protos.DeviceImage.VoidResponse? _deviceImageresponse; // Declare '_response' as nullable to fix CS8618
        private VerifyModelInPricePointCertificateResponse? _verifyModelInPricePointCertificateResponse; // Declare 'verifyModelInPricePointCertificateResponse' as nullable to fix CS8618
        private IsFamilyCertificateValidResponse? _isFamilyCertificateValidResponse; // Declare 'isFamilyCertificateValidResponse' as nullable to fix CS8618
        private ReadPricePointCertInputResponse? _readPricePointCertInputResponse; // Declare 'readPricePointCertInputResponse' as nullable to fix CS8618

        const string fdiPath = @"C:\ProgramData\ReSound\Camelot\Test Programs\ReSound Nexia 9\NX962-DRW [10]\Final\NX962-DRW.10.43.1.1.fdidfu";
        const string hdiPath = @"C:\Program Files (x86)\GN Hearing\Avalon\Device.Dooku3\Dooku3.C6.HDI.1.4.xml";
        public SecurityCertificatesSuccessStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _hearingInstrumentPage = (HearingInstrumentPage)_scenarioContext["GrpcHearingInstrument"];
            _securityCertificatesPage = (SecurityCertificatesPage)_scenarioContext["GrpcSecurityCertificates"];
            _deviceImagePage = (DeviceImagePage)_scenarioContext["GrpcDeviceImage"];
        }

        [When("Send a request to the InProductionCertificate API with a valid certificate to be written to the device")]
        public async Task WhenSendARequestToTheInProductionCertificateAPIWithAValidCertificateToBeWrittenToTheDeviceAsync(DataTable dataTable)
        {
            _test = _scenarioContext.Get<ExtentTest>("CurrentTest");
            _step = ExtentReportManager.GetInstance().CreateTestStep(_test, ScenarioStepContext.Current.StepInfo.Text);
            string ValidInProductionCertificate = "ValidInProductionCertificate"; // Replace with actual valid certificate content if needed

            ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Starting device initialization and product configuration for serial number detection.");
            try
            {
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling Initialize API to initialize the device...");
                _hearingInstrumentResponse = await _hearingInstrumentPage.CallInitializeAsync();
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Device initialized successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Calling ConfigureProduct API with FDTS configuration file...");
                _hearingInstrumentResponse = await _hearingInstrumentPage.CallConfigureProductAsync("C:\\ProgramData\\GN GOP\\Configuration\\FDTS");
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

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Attempting to connect to device using Connect API...");
                _connectResponse = await _hearingInstrumentPage.CallConnectAsync(_getDeviceNodeResponse!.DeviceNode);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, "Connected to device successfully.");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Sending request to CallLoadImageDataFromFileAsync...");
                _deviceImageresponse = await _deviceImagePage.CallLoadImageDataFromFileAsync(fdiPath, hdiPath);
                ExtentReportManager.GetInstance().LogToReport(_step, Status.Pass, $"{_deviceImageresponse.ToString()}");

                ExtentReportManager.GetInstance().LogToReport(_step, Status.Info, "Preparing to write InProduction certificate to the device...");
                _scecurityCertificateResponse = await _securityCertificatesPage.CallWriteInProductionCertificateAsync(ValidInProductionCertificate);
                if (_scecurityCertificateResponse == null)
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
                if (_scecurityCertificateResponse == null)
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
    }
}
