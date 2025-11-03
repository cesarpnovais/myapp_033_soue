using PayFlow.Api.Models;

namespace PayFlow.Api.Services
{
    public class PaymentService
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public PaymentService(IEnumerable<IPaymentProvider> providers)
        {
            _providers = providers;
        }

        public async Task<PaymentResponse> ProcessAsync(PaymentRequest request)
        {
            // pick preferred provider based on amount
            IPaymentProvider? mainProvider = request.Amount < 100
                ? _providers.FirstOrDefault(p => p.Name == "FastPay")
                : _providers.FirstOrDefault(p => p.Name == "SecurePay");

            // fallback: the other provider
            var backupProvider = _providers.FirstOrDefault(p => p != mainProvider);

            if (mainProvider == null && backupProvider == null)
                throw new InvalidOperationException("No payment providers registered.");

            PaymentResponse? result = null;

            if (mainProvider != null)
            {
                result = await mainProvider.ProcessAsync(request);
            }

            if (result == null && backupProvider != null)
            {
                result = await backupProvider.ProcessAsync(request);
            }

            if (result == null)
            {
                // All providers failed
                return new PaymentResponse
                {
                    Id = 0,
                    ExternalId = string.Empty,
                    Status = "failed",
                    Provider = "none",
                    GrossAmount = request.Amount,
                    Fee = 0,
                    NetAmount = 0
                };
            }

            result.Id = new Random().Next(1, 999999);
            return result;
        }
    }
}
