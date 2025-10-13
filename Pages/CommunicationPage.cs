using Avalon.Dooku3.gRPCService.Protos.Communication;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class CommunicationPage
    {
        private readonly Communication.CommunicationClient _communicationClient;
        public CommunicationPage(GrpcChannel grpcChannel)
        {
            _communicationClient = new Communication.CommunicationClient(grpcChannel);
        }
        public async Task<DeviceIdList> CallGetAvailableCommunicationDevicesAsync()
        {
            return await _communicationClient.GetAvailableCommunicationDevicesAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetCommunicationDeviceIdAsync(string deviceId)
        {
            var request = new SetCommunicationDeviceIdRequest { DeviceId = deviceId };
            return await _communicationClient.SetCommunicationDeviceIdAsync(request);
        }
        public async Task<GetCommunicationDeviceIdResponse> CallGetCommunicationDeviceIdAsync()
        {
            return await _communicationClient.GetCommunicationDeviceIdAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallRefreshCommunicationDevicesAsync()
        {
            return await _communicationClient.RefreshCommunicationDevicesAsync(new EmptyRequest());
        }
    }
}
