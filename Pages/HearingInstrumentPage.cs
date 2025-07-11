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
        public async Task<ConnectResponse> CallConnectAsync(DeviceNode deviceNode)
        {
            var request = new ConnectRequest { DeviceNode = deviceNode };
            return await _client.ConnectAsync(request);
        }
        public async Task<GetBootModeResponse> CallGetBootModeAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetBootModeAsync(request);
        }
        public async Task<VoidResponse> CallBootAsync(BootType bootType, bool reconnect)
        {
            var request = new BootRequest
            {
                BootType = bootType,
                Reconnect = reconnect
            };
            return await _client.BootAsync(request);
         }
        public async Task<GetFlashWriteProtectStatusResponse> CallGetFlashWriteProtectStatusAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetFlashWriteProtectStatusAsync(request);
        }
        public async Task<SetFlashWriteProtectStateResponse> CallSetFlashWriteProtectStateAsync(FlashWriteProtectState state)
        {
            var request = new SetFlashWriteProtectStateRequest
            {
                FlashWriteProtectState = state
            };

            return await _client.SetFlashWriteProtectStateAsync(request);
        }
        public async Task<IsRechargeableResponse> CallIsRechargeableAsync()
        {
            return await _client.IsRechargeableAsync(new EmptyRequest());
        }
        public async Task<GetBatteryLevelResponse> CallGetBatteryLevelAsync()
        {
            return await _client.GetBatteryLevelAsync(new EmptyRequest());
        }
        public async Task<ShouldVerifyMfiChipResponse> CallShouldVerifyMfiChipAsync()
        {
            return await _client.ShouldVerifyMfiChipAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallVerifyMfiChipIsHealthyAsync()
        {
            return await _client.VerifyMfiChipIsHealthyAsync(new EmptyRequest());
        }
        public async Task<GetBatteryTypeResponse> CallGetBatteryTypeAsync()
        {
            return await _client.GetBatteryTypeAsync(new EmptyRequest());
        }
        public async Task<GetBatteryVoltageResponse> CallGetBatteryVoltageAsync()
        {

            return await _client.GetBatteryVoltageAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetBatteryTypeAsync(string batteryType)
        {
            var request = new SetBatteryTypeRequest { BatteryType = batteryType };
            return await _client.SetBatteryTypeAsync(request);
        }
        public async Task<VoidResponse> CallMakeDeviceFunctionalAsync(bool deviceIsFunctional)
        {
            var request = new MakeDeviceFunctionalRequest
            {
                DeviceIsFunctional = deviceIsFunctional
            };
            return await _client.MakeDeviceFunctionalAsync(request);
        }
        public async Task<VoidResponse> CallSetPowerOffAsync()
        {
            var request = new EmptyRequest();
            return await _client.SetPowerOffAsync(request);
        }
    }
}
