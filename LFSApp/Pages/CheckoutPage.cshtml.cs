using System.Net.Http.Headers;
using LFSApp.Dbcontext;
using LFSApp.Helper;
using LFSApp.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;

namespace LFSApp.Pages
{
    public class CheckoutPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly LFSApp.Services.IPaystackService _paystackService;
        private readonly Microsoft.Extensions.Logging.ILogger<CheckoutPageModel> _logger;

        public CheckoutPageModel(ApplicationDbContext context, LFSApp.Services.IPaystackService paystackService, Microsoft.Extensions.Logging.ILogger<CheckoutPageModel> logger)
        {
            _context = context;
            _paystackService = paystackService;
            _logger = logger;
        }
        [BindProperty]
        public Order Order { get; set; } = new();
        public List<Cart> CartItems { get; set; } = new();

        public decimal Total => CartItems.Sum(c => c.SubTotal);

        public string? RedirectUrl { get; private set; }

        public async Task<ActionResult> OnGet()
        {
            // Enforce server-side auth check: redirect guests to AuthPage with ReturnUrl
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/AuthPage", new { returnUrl = "/CheckoutPage" });
            }
                        // Load cart items from session to compute totals and pre-fill amount
                        var cart = HttpContext.Session.GetObject<List<Cart>>("CartItems");
                        CartItems = cart ?? new List<Cart>();
                        // Prepopulate the order amount so the form binds correctly
                        Order.Amount = Total;
                        return Page();
        }
        // public async Task<IActionResult> OnPostAsync()
        // {
        //     // ✅ Retrieve cart from session
        //     var cart = HttpContext.Session.GetObject<List<Cart>>("CartItems");
        //     if (cart == null || !cart.Any())
        //     {
        //         ModelState.AddModelError(string.Empty, "Your cart is empty.");
              
        //     }

        //     // ✅ Create Order
        //     var order = new Order
        //     {
        //         FullName = Order.FullName,
        //         Email = Order.Email,
        //         Phone = Order.Phone,
        //         Address = Order.Address,
        //         Size = Order.Size,
        //         Delivery = Order.Delivery,
        //         PaymentMethod = Order.PaymentMethod,
        //         //CartId = Order.CartId,
        //     };
        //     _context.Orders.Add(order);
        //     _context.SaveChanges();

        //     // ✅ Save Order Items
        //     foreach (var item in cart)
        //     {
        //         _context.CartItem.Add(new Cart
        //         {
        //              // ✅ Link to order
        //             ProductId = item.ProductId,
        //             Name = item.Name,
        //             Price = item.Price,
        //             Quantity = item.Quantity,
        //             ImageUrl = item.ImageUrl,
             
        //         });
        //     }

        //     _context.SaveChanges();

        //     // ✅ Clear cart
        //     HttpContext.Session.Remove("Cart");

        //     return RedirectToPage("/Index");
        // }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                // If a guest somehow posts the checkout form, send them to the auth page first.
                return RedirectToPage("/AuthPage", new { returnUrl = "/CheckoutPage" });
            }
            if (!ModelState.IsValid) return Page();

            // Step 1: Save to DB with a unique reference
            Order.Reference = "PS_" + new Random().Next(1000000, int.MaxValue);
            Order.IsPaid = false;
            Order.CreatedAt = DateTime.Now;
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            // Step 2: Initialize Paystack transaction via service
            var callbackUrl = $"{Request.Scheme}://{Request.Host}{Url.Page("/CheckoutPage", "VerifyPayment")}";
            try
            {
                RedirectUrl = await _paystackService.InitializeTransactionAsync(Order.Email, Order.Amount, Order.Reference, callbackUrl);
            }
            catch (HttpRequestException ex)
            {
                // Surface a friendly message in the UI while logging the detailed error.
                ModelState.AddModelError(string.Empty, "Unable to initialize Paystack transaction. Check your Paystack secret key and try again.");
                // Log full exception to the configured logger (do not log secrets)
                _logger?.LogError(ex, "Paystack initialization failed: {Message}", ex.Message);
                return Page();
            }

            return Page(); // Will trigger the redirect via JS
        }

        // Step 4: Verify payment after user returns from Paystack
        public async Task<IActionResult> OnGetVerifyPayment(string reference)
        {
            var verified = await _paystackService.VerifyTransactionAsync(reference);

            var payment = await _context.Orders.FirstOrDefaultAsync(p => p.Reference == reference);

            if (verified)
            {
                if (payment != null)
                {
                    payment.IsPaid = true;
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage("/PaymentResult", new { reference, success = true });
            }
            else
            {
               return RedirectToPage("/PaymentResult", new { reference, success = false });
            }
            
        }
    }
}
