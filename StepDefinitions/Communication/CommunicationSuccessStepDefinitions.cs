using System;
using Reqnroll;

namespace QuantumServicesAPI.StepDefinitions.Communication
{
    [Binding]
    public class CommunicationSuccessStepDefinitions
    {
        [When("Connect HIPRO{int} with available\\/test firmware and send a request to the InitializeCommunication API")]
        public void WhenConnectHIPROWithAvailableTestFirmwareAndSendARequestToTheInitializeCommunicationAPI(int p0)
        {
            throw new PendingStepException();
        }

        [Then("API initializes the device successfully with supported firmware")]
        public void ThenAPIInitializesTheDeviceSuccessfullyWithSupportedFirmware()
        {
            throw new PendingStepException();
        }

        [When("Connect NoahLink Wireless \\(firmware {float}) and send a request to the InitializeCommunicationDevice API")]
        public void WhenConnectNoahLinkWirelessFirmwareAndSendARequestToTheInitializeCommunicationDeviceAPI(decimal p0)
        {
            throw new PendingStepException();
        }

        [Then("API initializes the device successfully")]
        public void ThenAPIInitializesTheDeviceSuccessfully()
        {
            throw new PendingStepException();
        }

        [When("Connect SpeedLink \\(firmware {float}.{int}) and send a request to the InitializeCommunicationDevice API")]
        public void WhenConnectSpeedLinkFirmware_AndSendARequestToTheInitializeCommunicationDeviceAPI(decimal p0, int p1)
        {
            throw new PendingStepException();
        }
    }
}
