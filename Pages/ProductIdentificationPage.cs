using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class ProductIdentificationPage
    {
        private readonly ProductIdentification.ProductIdentificationClient _productIdentificationClient;
        public ProductIdentificationPage(GrpcChannel grpcChannel)
        {
            _productIdentificationClient= new ProductIdentification.ProductIdentificationClient(grpcChannel);

        }
        public async Task<ReadPcbaPartNumberResponse> CallReadPcbaPartNumberAsync()
        {
            return await _productIdentificationClient.ReadPcbaPartNumberAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallReadAsync()
        {
            return await _productIdentificationClient.ReadAsync(new EmptyRequest());
        }
        public async Task<GetPlatformNameResponse> CallGetPlatformNameAsync()
        {
            return await _productIdentificationClient.GetPlatformNameAsync(new EmptyRequest());
        }
        public async Task<GetSerialNumberResponse> CallGetSerialNumberAsync()
        {
            return await _productIdentificationClient.GetSerialNumberAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetSerialNumberAsync(string serialNumber)
        {
            var request = new SetSerialNumberRequest { SerialNumber = serialNumber };
            return await _productIdentificationClient.SetSerialNumberAsync(request);
        }

        public async Task<GetSideResponse> GetSideResponseAsync()
        {
            return await _productIdentificationClient.GetSideAsync(new EmptyRequest());
        }

        public async Task<VoidResponse> CallSetSideAsync(string side)
        {
            var request = new SetSideRequest { Side = side };
            return await _productIdentificationClient.SetSideAsync(request);
        }

        public async Task<GetNetworkAddressResponse> GetNetworkAddressResponseAsync()
        {
            return await _productIdentificationClient.GetNetworkAddressAsync(new EmptyRequest());
        }

        public async Task<VerifyProductResponse> VerifyProductAsync(VerifyProductRequest request)
        {
            return await _productIdentificationClient.VerifyProductAsync(request);
        }


        public async Task<VoidResponse> UpdateGattDatabaseAsync(UpdateGattDatabaseRequest request)
        {
            return await _productIdentificationClient.UpdateGattDatabaseAsync(request);
        }

        public async Task<ReadCloudRegistrationInputResponse> ReadCloudRegistrationInputAsync(ReadCloudRegistrationInputRequest inputRequest)
        {
            return await _productIdentificationClient.ReadCloudRegistrationInputAsync(inputRequest);
        }

        public async Task<GetDateModifiedResponse> GetDateModifiedResponseAsync()
        {
            return await _productIdentificationClient.GetDateModifiedAsync(new EmptyRequest());
        }

        public async Task<VoidResponse> ResetDateModifiedAsync()
        {
            return await _productIdentificationClient.ResetDateModifiedAsync(new EmptyRequest());
        }

        public async Task<GetOptionsForDeviceResponse> GetOptionsForDeviceAsync()
        {
            return await _productIdentificationClient.GetOptionsForDeviceAsync(new EmptyRequest());
        }

        public async Task<VoidResponse> SetOptionsForDeviceAsync(SetOptionsForDeviceRequest request)
        {
            return await _productIdentificationClient.SetOptionsForDeviceAsync(request);
        }

        public async Task<GetPrivateLabelCodeResponse> GetPrivateLabelCodeAsync()
        {
            return await _productIdentificationClient.GetPrivateLabelCodeAsync(new EmptyRequest());
        }

        public async Task<VoidResponse> SetPrivateLabelCodeAsync(SetPrivateLabelCodeRequest codeRequest)
        {
            return await _productIdentificationClient.SetPrivateLabelCodeAsync(codeRequest);
        }



    }
}
