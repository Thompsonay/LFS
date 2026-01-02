using System.Security.Cryptography;
using System.Text;
using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LFSApp.Controllers
{
    [ApiController]
    [Route("api/paystack/webhook")]
    public class PaystackWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PaystackSettings _settings;

        public PaystackWebhookController(ApplicationDbContext context, IOptions<PaystackSettings> options)
        {
            _context = context;
            _settings = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Read request body
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();

            // Verify signature header
            if (!Request.Headers.TryGetValue("x-paystack-signature", out var signature))
            {
                return BadRequest();
            }

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            if (!string.Equals(computed, signature.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            // Parse payload and update order if necessary
            var doc = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(payload);
            if (doc.TryGetProperty("event", out var evt) && evt.GetString() == "charge.success")
            {
                var data = doc.GetProperty("data");
                var reference = data.GetProperty("reference").GetString();
                var status = data.GetProperty("status").GetString();

                if (string.Equals(status, "success", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(reference))
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Reference == reference);
                    if (order != null)
                    {
                        order.IsPaid = true;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // return 200 OK quickly
            return Ok();
        }
    }
}
