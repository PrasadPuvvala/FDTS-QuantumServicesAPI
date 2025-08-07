using Avalon.Dooku3.gRPCService.Protos.ProductionTestData;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class ProductionTestDataPage
    {
        private readonly ProductionTestData.ProductionTestDataClient _productionTestDataClient;
        public ProductionTestDataPage(GrpcChannel grpcChannel)
        {
            _productionTestDataClient = new ProductionTestData.ProductionTestDataClient(grpcChannel);
        }
        public async Task<VoidResponse> CallReadAsync()
        {
            return await _productionTestDataClient.ReadAsync(new EmptyRequest());
        }
        public async Task<GetTestSiteResponse> CallGetTestSiteAsync()
        {
            return await _productionTestDataClient.GetTestSiteAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetTestSiteAsync(string testSite)
        {
            var request = new SetTestSiteRequest { TestSite = testSite };
            return await _productionTestDataClient.SetTestSiteAsync(request);
        }
        public async Task<GetTestStationResponse> CallGetTestStationAsync()
        {
            return await _productionTestDataClient.GetTestStationAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetTestStationAsync(string testStation)
        {
            var request = new SetTestStationRequest { TestStation = testStation };
            return await _productionTestDataClient.SetTestStationAsync(request);
        }
        public async Task<GetTpiReleaseCodeResponse> CallGetTpiReleaseCodeAsync()
        {
            return await _productionTestDataClient.GetTpiReleaseCodeAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetTpiReleaseCodeAsync(string tpiReleaseCode)
        {
            var request = new SetTpiReleaseCodeRequest { TpiReleaseCode = tpiReleaseCode };
            return await _productionTestDataClient.SetTpiReleaseCodeAsync(request);
        }
        public async Task<GetTestDateResponse> CallGetTestDateAsync()
        {
            return await _productionTestDataClient.GetTestDateAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetTestDataAsync(string year, string month, string day, string hour, string minute, string second)
        {
            var request = new SetTestDateRequest
            {
                Year = int.Parse(year),
                Month = int.Parse(month),
                Day = int.Parse(day),
                Hour = int.Parse(hour),
                Minute = int.Parse(minute),
                Second = int.Parse(second)
            };
            return await _productionTestDataClient.SetTestDataAsync(request);
        }
        public async Task<GetModelVerificationIdResponse> CallGetModelVerificationIdAsync()
        {
            return await _productionTestDataClient.GetModelVerificationIdAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallSetModelVerificationIdAsync(string modelVerificationId)
        {
            var request = new SetModelVerificationIdRequest { ModelVerificationId = int.Parse(modelVerificationId) };
            return await _productionTestDataClient.SetModelVerificationIdAsync(request);
        }
        public async Task<VoidResponse> CallWriteAsync()
        {
            return await _productionTestDataClient.WriteAsync(new EmptyRequest());
        }
    }
}