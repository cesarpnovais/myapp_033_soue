using PayFlow.Api.Models;
using System.Net.Http.Json;

namespace PayFlow.Api.Services
{
    public class SecurePayService : IPaymentProvider
    {
        public string Name => "SecurePay";
        private readonly HttpClient _http;

        public SecurePayService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaymentResponse?> ProcessAsync(PaymentRequest request)
        {
            var payload = new
            {
                amount_cents = (int)(request.Amount * 100),
                currency_code = request.Currency,
                client_reference = $"ORD-{DateTime.Now:yyyyMMddHHmmss}"
            };

            try
            {
                var response = await _http.PostAsJsonAsync("https://api.securepay.fake/charge", payload);
                if (!response.IsSuccessStatusCode) return null;

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                var txId = data != null && data.ContainsKey("transaction_id") ? data["transaction_id"]?.ToString() : null;
                var result = data != null && data.ContainsKey("result") ? data["result"]?.ToString() : null;

                var fee = Math.Round((request.Amount * 0.0299m) + 0.40m, 2);
                return new PaymentResponse
                {
                    ExternalId = txId ?? string.Empty,
                    Status = (result == "success") ? "approved" : "failed",
                    Provider = Name,
                    GrossAmount = request.Amount,
                    Fee = fee,
                    NetAmount = Math.Round(request.Amount - fee, 2)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
