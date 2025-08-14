using Avalon.Dooku3.gRPCService.Protos.SecurityCertificates;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumServicesAPI.Pages
{
    public class SecurityCertificatesPage
    {
        private readonly SecurityCertificates.SecurityCertificatesClient _securityCertificatesClient;
        public SecurityCertificatesPage(GrpcChannel grpcChannel)
        {
            _securityCertificatesClient = new SecurityCertificates.SecurityCertificatesClient(grpcChannel);
        }
        public async Task<VoidResponse> CallWriteInProductionCertificateAsync(string certificate)
        {
            var request = new WriteInProductionCertificateRequest
            {
                Certificate = certificate
            };
            return await _securityCertificatesClient.WriteInProductionCertificateAsync(request);
        }
        public async Task<VerifyModelInPricePointCertificateResponse> CallVerifyModelInPricePointCertificateAsync()
        {
            return await _securityCertificatesClient.VerifyModelInPricePointCertificateAsync(new EmptyRequest());
        }
        public async Task<IsFamilyCertificateValidResponse> CallIsFamilyCertificateValidAsync()
        {
            return await _securityCertificatesClient.IsFamilyCertificateValidAsync(new EmptyRequest());
        }
        public async Task<ReadPricePointCertInputResponse> CallReadPricePointCertInputAsync()
        {
            return await _securityCertificatesClient.ReadPricePointCertInputAsync(new EmptyRequest());
        }
        public async Task<VoidResponse> CallWritePricePointCertificateAsync(string certificate)
        {
            var request = new WritePricePointCertificateRequest
            {
                Certificate = certificate
            };
            return await _securityCertificatesClient.WritePricePointCertificateAsync(request);
        }

    }
}