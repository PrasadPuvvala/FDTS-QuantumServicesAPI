using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalon.Dooku3.gRPCService.Protos.HearingInstrument;
using Grpc.Net.Client;

namespace QuantumServicesAPI.Pages
{
    /// <summary>
    /// Provides methods to interact with the Hearing Instrument gRPC service.
    /// </summary>
    public class HearingInstrumentPage
    {
        private readonly HearingInstrument.HearingInstrumentClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HearingInstrumentPage"/> class.
        /// </summary>
        /// <param name="grpcChannel">The gRPC channel to use for communication.</param>
        public HearingInstrumentPage(GrpcChannel grpcChannel)
        {
            _client = new HearingInstrument.HearingInstrumentClient(grpcChannel);
        }

        /// <summary>
        /// Calls the Initialize method on the gRPC service.
        /// </summary>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallInitializeAsync()
        {
            var request = new EmptyRequest();
            return await _client.InitializeAsync(request);
        }

        /// <summary>
        /// Configures the product using the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path to configure the product with.</param>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallConfigureProductAsync(string folderPath)
        {
            var request = new ConfigureProductRequest { FolderPath = folderPath };
            return await _client.ConfigureProductAsync(request);
        }

        /// <summary>
        /// Detects a device by its serial number.
        /// </summary>
        /// <param name="serialnumber">The serial number of the device.</param>
        /// <returns>A <see cref="DetectBySerialNumberResponse"/> from the service.</returns>
        public async Task<DetectBySerialNumberResponse> CallDetectBySerialNumberAsync(string serialnumber)
        {
            var request = new DetectBySerialNumberRequest { SerialNumber = serialnumber };
            return await _client.DetectBySerialNumberAsync(request);
        }

        /// <summary>
        /// Detects the closest device.
        /// </summary>
        /// <returns>A <see cref="DetectClosestResponse"/> from the service.</returns>
        public async Task<DetectClosestResponse> CallDetectClosestAsync()
        {
            var request = new EmptyRequest();
            return await _client.DetectClosestAsync(request);
        }

        /// <summary>
        /// Detects a device on the specified side.
        /// </summary>
        /// <param name="side">The side to detect the device on.</param>
        /// <returns>A <see cref="DetectOnSideResponse"/> from the service.</returns>
        public async Task<DetectOnSideResponse> CallDetectOnSideAsync(ChannelSide side)
        {
            var request = new DetectOnSideRequest { Side = side };
            return await _client.DetectOnSideAsync(request);
        }


        /// <summary>
        /// Enables or disables master connect mode.
        /// </summary>
        /// <param name="isEnabled">True to enable, false to disable.</param>
        /// <returns>A <see cref="EnableMasterConnectResponse"/> from the service.</returns>
        public async Task<EnableMasterConnectResponse> CallEnableMasterConnectAsync(bool isEnabled)
            => await _client.EnableMasterConnectAsync(new EnableMasterConnectRequest { IsEnabled = isEnabled });

        /// <summary>
        /// Enables or disables fitting mode.
        /// </summary>
        /// <param name="isEnabled">True to enable, false to disable.</param>
        /// <returns>A <see cref="EnableFittingModeResponse"/> from the service.</returns>
        public async Task<EnableFittingModeResponse> CallEnableFittingModeAsync(bool isEnabled)
            => await _client.EnableFittingModeAsync(new EnableFittingModeRequest { IsEnabled = isEnabled });

        /// <summary>
        /// Gets the device node information.
        /// </summary>
        /// <returns>A <see cref="GetDeviceNodeResponse"/> from the service.</returns>
        public async Task<GetDeviceNodeResponse> CallGetDeviceNodeAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetDeviceNodeAsync(request);
        }

        /// <summary>
        /// Connects to a device node.
        /// </summary>
        /// <param name="deviceNode">The device node to connect to.</param>
        /// <returns>A <see cref="ConnectResponse"/> from the service.</returns>
        public async Task<ConnectResponse> CallConnectAsync(DeviceNode deviceNode)
        {
            var request = new ConnectRequest { DeviceNode = deviceNode };
            return await _client.ConnectAsync(request);
        }

        /// <summary>
        /// Gets the current boot mode.
        /// </summary>
        /// <returns>A <see cref="GetBootModeResponse"/> from the service.</returns>
        public async Task<GetBootModeResponse> CallGetBootModeAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetBootModeAsync(request);
        }

        /// <summary>
        /// Boots the device with the specified boot type and reconnect option.
        /// </summary>
        /// <param name="bootType">The type of boot to perform.</param>
        /// <param name="reconnect">Whether to reconnect after booting.</param>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallBootAsync(BootType bootType, bool reconnect)
        {
            var request = new BootRequest
            {
                BootType = bootType,
                Reconnect = reconnect
            };
            return await _client.BootAsync(request);
        }

        /// <summary>
        /// Gets the flash write protect status.
        /// </summary>
        /// <returns>A <see cref="GetFlashWriteProtectStatusResponse"/> from the service.</returns>
        public async Task<GetFlashWriteProtectStatusResponse> CallGetFlashWriteProtectStatusAsync()
        {
            var request = new EmptyRequest();
            return await _client.GetFlashWriteProtectStatusAsync(request);
        }

        /// <summary>
        /// Sets the flash write protect state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        /// <returns>A <see cref="SetFlashWriteProtectStateResponse"/> from the service.</returns>
        public async Task<SetFlashWriteProtectStateResponse> CallSetFlashWriteProtectStateAsync(FlashWriteProtectState state)
        {
            var request = new SetFlashWriteProtectStateRequest
            {
                FlashWriteProtectState = state
            };

            return await _client.SetFlashWriteProtectStateAsync(request);
        }

        /// <summary>
        /// Checks if the device is rechargeable.
        /// </summary>
        /// <returns>An <see cref="IsRechargeableResponse"/> from the service.</returns>
        public async Task<IsRechargeableResponse> CallIsRechargeableAsync()
        {
            return await _client.IsRechargeableAsync(new EmptyRequest());
        }

        /// <summary>
        /// Gets the battery level.
        /// </summary>
        /// <returns>A <see cref="GetBatteryLevelResponse"/> from the service.</returns>
        public async Task<GetBatteryLevelResponse> CallGetBatteryLevelAsync()
        {
            return await _client.GetBatteryLevelAsync(new EmptyRequest());
        }

        /// <summary>
        /// Checks if the MFi chip should be verified.
        /// </summary>
        /// <returns>A <see cref="ShouldVerifyMfiChipResponse"/> from the service.</returns>
        public async Task<ShouldVerifyMfiChipResponse> CallShouldVerifyMfiChipAsync()
        {
            return await _client.ShouldVerifyMfiChipAsync(new EmptyRequest());
        }

        /// <summary>
        /// Verifies if the MFi chip is healthy.
        /// </summary>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallVerifyMfiChipIsHealthyAsync()
        {
            return await _client.VerifyMfiChipIsHealthyAsync(new EmptyRequest());
        }

        /// <summary>
        /// Gets the battery type.
        /// </summary>
        /// <returns>A <see cref="GetBatteryTypeResponse"/> from the service.</returns>
        public async Task<GetBatteryTypeResponse> CallGetBatteryTypeAsync()
        {
            return await _client.GetBatteryTypeAsync(new EmptyRequest());
        }

        /// <summary>
        /// Gets the battery voltage.
        /// </summary>
        /// <returns>A <see cref="GetBatteryVoltageResponse"/> from the service.</returns>
        public async Task<GetBatteryVoltageResponse> CallGetBatteryVoltageAsync()
        {
            return await _client.GetBatteryVoltageAsync(new EmptyRequest());
        }

        /// <summary>
        /// Sets the battery type.
        /// </summary>
        /// <param name="batteryType">The battery type to set.</param>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallSetBatteryTypeAsync(string batteryType)
        {
            var request = new SetBatteryTypeRequest { BatteryType = batteryType };
            return await _client.SetBatteryTypeAsync(request);
        }

        /// <summary>
        /// Sets the device functional state.
        /// </summary>
        /// <param name="deviceIsFunctional">True if the device is functional, otherwise false.</param>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallMakeDeviceFunctionalAsync(bool deviceIsFunctional)
        {
            var request = new MakeDeviceFunctionalRequest
            {
                DeviceIsFunctional = deviceIsFunctional
            };
            return await _client.MakeDeviceFunctionalAsync(request);
        }

        /// <summary>
        /// Powers off the device.
        /// </summary>
        /// <returns>A <see cref="VoidResponse"/> from the service.</returns>
        public async Task<VoidResponse> CallSetPowerOffAsync()
        {
            var request = new EmptyRequest();
            return await _client.SetPowerOffAsync(request);
        }
    }
}
