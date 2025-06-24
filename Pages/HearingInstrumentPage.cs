using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Grpc.Net.Client;

namespace QuantumServicesAPI.Pages
{
    public class HearingInstrumentPage
    {
        private readonly HearingInstrument.HearingInstrumentClient _client;
        public HearingInstrumentPage(GrpcChannel grpcChannel)
        {
            _client = new HearingInstrument.HearingInstrumentClient(grpcChannel);
        }
        public async Task<VoidResponse> CallInitializeAsync()
        {
            var request = new EmptyRequest();  
            return await _client.InitializeAsync(request);  
        }
        public async Task<VoidResponse> CallConfigureProductAsync(string folderPath)
        {
            var request = new ConfigureProductRequest { FolderPath = folderPath };
            return await _client.ConfigureProductAsync(request);
        }
        public async Task<DetectBySerialNumberResponse> CallDetectBySerialNumberAsync(string serialnumber)
        {
            var request = new DetectBySerialNumberRequest { SerialNumber = serialnumber };
            return await _client.DetectBySerialNumberAsync(request);
        }
        public async Task<DetectClosestResponse> CallDetectClosestAsync()
        {
            var request = new EmptyRequest();
            return await _client.DetectClosestAsync(request);
        }
        public async Task<DetectOnSideResponse> CallDetectOnSideAsync(ChannelSide side)
        {
            var request = new DetectOnSideRequest { Side = side };
            return await _client.DetectOnSideAsync(request);
        }
        public async Task<EnableMasterConnectResponse> CallEnableMasterConnectAsync(bool isEnabled)
      => await _client.EnableMasterConnectAsync(new EnableMasterConnectRequest { IsEnabled = isEnabled });
        public async Task<EnableFittingModeResponse> CallEnableFittingModeAsync(bool isEnabled)
          => await _client.EnableFittingModeAsync(new EnableFittingModeRequest { IsEnabled = isEnabled });
        public async Task<GetDeviceNodeResponse> CallGetDeviceNodeAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetDeviceNodeAsync(request);
        }
    }
}
