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
    }
}
