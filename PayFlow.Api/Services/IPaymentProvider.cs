using PayFlow.Api.Models;

namespace PayFlow.Api.Services
{
    public interface IPaymentProvider
    {
        string Name { get; }
        Task<PaymentResponse?> ProcessAsync(PaymentRequest request);
    }
}
