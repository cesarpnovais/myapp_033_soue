using PayFlow.Api.Models;
using System.Net.Http.Json;

namespace PayFlow.Api.Services
{
    public class FastPayService : IPaymentProvider
    {
        public string Name => "FastPay";
        private readonly HttpClient _http;

        public FastPayService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaymentResponse?> ProcessAsync(PaymentRequest request)
        {
            // Build FastPay payload
            var payload = new
            {
                transaction_amount = request.Amount,
                currency = request.Currency,
                payer = new { email = "cliente@teste.com" },
                installments = 1,
                description = "Compra via FastPay"
            };

            // NOTE: for the test, external endpoints are not real.
            // You can replace the URI with a mock server if needed.
            try
            {
                var response = await _http.PostAsJsonAsync("https://api.fastpay.fake/payments", payload);
                if (!response.IsSuccessStatusCode) return null;

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                var id = data != null && data.ContainsKey("id") ? data["id"]?.ToString() : null;
                var status = data != null && data.ContainsKey("status") ? data["status"]?.ToString() : null;

                var fee = Math.Round(request.Amount * 0.0349m, 2);
                return new PaymentResponse
                {
                    ExternalId = id ?? string.Empty,
                    Status = status ?? "unknown",
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
