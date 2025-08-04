using Avalon.Dooku3.gRPCService.Protos.DeviceImage;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class DeviceImagePage
    {
        private readonly DeviceImage.DeviceImageClient _deviceImageClient;
        public DeviceImagePage(GrpcChannel grpcChannel)
        {
            _deviceImageClient = new DeviceImage.DeviceImageClient(grpcChannel);
        }

        public async Task<VoidResponse> CallLoadImageDataFromFileAsync(string fdiPath, string hdiPath)
        {
            var request = new LoadImageDataFromFileRequest
            {
                FdiPath = fdiPath,
                HdiPath = hdiPath
            };
            return await _deviceImageClient.LoadImageDataFromFileAsync(request);
        }
        public async Task<IsCustomProductResponse> CallIsCustomProductAsync()
        {
            return await _deviceImageClient.IsCustomProductAsync(new EmptyRequest());
        }
        public async Task<IsOptimizedProgrammingResponse> CallIsOptimizedProgrammingAsync(int imageHashCookie, string deviceName)
        {
            var request = new IsOptimizedProgrammingRequest
            {
                ImageHashCookie = imageHashCookie,
                DeviceName = deviceName
            };
            return await _deviceImageClient.IsOptimizedProgrammingAsync(request);
        }
        public async Task<IsDfuCompatibleResponse> CallIsDfuCompatibleAsync()
        {
            return await _deviceImageClient.IsDfuCompatibleAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallWriteAsync(bool isOptimizedProgramming)
        {
            var request = new WriteRequest { IsOptimizedProgramming = isOptimizedProgramming };
            return await _deviceImageClient.WriteAsync(request);
        }
        public async Task<VoidResponse> CallWriteImageHashAsync()
        {
            return await _deviceImageClient.WriteImageHashAsync(new EmptyRequest());
        }
    }
}
