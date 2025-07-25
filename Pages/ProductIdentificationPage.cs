using Avalon.Dooku3.gRPCService.Protos.ProductIdentification;
using Grpc.Net.Client;

namespace QuantumServicesAPI.Pages
{
    public class ProductIdentificationPage
    {
        private readonly ProductIdentification.ProductIdentificationClient _productIdentificationClient;
        public ProductIdentificationPage(GrpcChannel grpcChannel)
        {
            _productIdentificationClient = new ProductIdentification.ProductIdentificationClient(grpcChannel);
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
        public async Task<GetSideResponse> CallGetSideAsync()
        {
            return await _productIdentificationClient.GetSideAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetSideAsync(string side)
        {
            var request = new SetSideRequest { Side = side };
            return await _productIdentificationClient.SetSideAsync(request);
        }
        public async Task<GetNetworkAddressResponse> CallGetNetworkAddressAsync()
        {
            return await _productIdentificationClient.GetNetworkAddressAsync(new EmptyRequest());
        }
        public async Task<VerifyProductResponse> CallVerifyProductAsync(int bleId, string brand, int privateLabel)
        {
            var request = new VerifyProductRequest
            {
                BleId = bleId,
                Brand = brand,
                PrivateLabel = privateLabel
            };
            return await _productIdentificationClient.VerifyProductAsync(request);
        }
        public async Task<VoidResponse> CallUpdateGattDatabaseAsync(string mfiBrand, string mfiModel, string mfiFamily, string gapDeviceName)
        {
            var request = new UpdateGattDatabaseRequest();
            request.GattKeyValueDictionary.Add("MFIBrand", mfiBrand);
            request.GattKeyValueDictionary.Add("MFIModel", mfiModel);
            request.GattKeyValueDictionary.Add("MFIFamily", mfiFamily);
            request.GattKeyValueDictionary.Add("GapDeviceName", gapDeviceName);

            return await _productIdentificationClient.UpdateGattDatabaseAsync(request);
        }
        public async Task<ReadCloudRegistrationInputResponse> CallReadCloudRegistrationInputAsync()
        {
            var request = new ReadCloudRegistrationInputRequest();
            request.RfuEnabled = true; // Assuming you want to set RfuEnabled to true
            request.RftEnabled = true; // Assuming you want to set RftEnabled to true
            return await _productIdentificationClient.ReadCloudRegistrationInputAsync(request);
        }
        public async Task<GetDateModifiedResponse> CallGetDateModifiedAsync()
        {
            return await _productIdentificationClient.GetDateModifiedAsync(new EmptyRequest());
        }
        public async Task<GetOptionsForDeviceResponse> CallGetOptionsForDeviceAsync()
        {
            return await _productIdentificationClient.GetOptionsForDeviceAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetOptionsForDeviceAsync(int options)
        {
            var request = new SetOptionsForDeviceRequest { OptionsForDevice = options };
            return await _productIdentificationClient.SetOptionsForDeviceAsync(request);
        }
        public async Task<GetPrivateLabelCodeResponse> CallGetPrivateLabelCodeAsync()
        {
            return await _productIdentificationClient.GetPrivateLabelCodeAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetPrivateLabelCodeAsync(int privateLabelCode)
        {
            var request = new SetPrivateLabelCodeRequest { PrivateLabelCode = privateLabelCode };
            return await _productIdentificationClient.SetPrivateLabelCodeAsync(request);
        }
    }
}
