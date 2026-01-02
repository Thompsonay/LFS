using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LFSApp.Pages
{
    public class PaymentResultModel : PageModel
    {
        public string Reference { get; set; } = string.Empty;
        public bool Success { get; set; }

        public void OnGet(string reference, bool success)
        {
            Reference = reference ?? string.Empty;
            Success = success;
        }
    }
}
