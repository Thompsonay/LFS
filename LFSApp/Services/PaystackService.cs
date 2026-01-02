using LFSApp.Model;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace LFSApp.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly IHttpClientFactory _factory;
        private readonly PaystackSettings _settings;

        public PaystackService(IHttpClientFactory factory, IOptions<PaystackSettings> options)
        {
            _factory = factory;
            _settings = options.Value;
        }

        public async Task<string> InitializeTransactionAsync(string email, decimal amount, string reference, string callbackUrl)
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_settings.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

            var payload = new
            {
                email,
                amount = (int)(amount * 100), // kobo
                reference,
                callback_url = callbackUrl
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await client.PostAsync("/transaction/initialize", content);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                // Surface the response body so it's easier to debug when Paystack returns 4xx/5xx
                throw new HttpRequestException($"Paystack initialize failed (Status={resp.StatusCode}): {body}");
            }
            var doc = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);

            if (doc.TryGetProperty("data", out var data) && data.TryGetProperty("authorization_url", out var authUrl))
            {
                return authUrl.GetString() ?? string.Empty;
            }

            throw new Exception("Unable to initialize paystack transaction.");
        }

        public async Task<bool> VerifyTransactionAsync(string reference)
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_settings.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

            var resp = await client.GetAsync($"/transaction/verify/{reference}");
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                // Make it easy to debug verification errors
                throw new HttpRequestException($"Paystack verify failed (Status={resp.StatusCode}): {body}");
            }
            var doc = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);

            var status = doc.GetProperty("data").GetProperty("status").GetString();
            return string.Equals(status, "success", StringComparison.OrdinalIgnoreCase);
        }
    }
}
