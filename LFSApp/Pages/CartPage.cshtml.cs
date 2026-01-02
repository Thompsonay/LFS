using LFSApp.Helper;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Newtonsoft.Json;


namespace LFSApp.Pages
{
    public class CartPageModel : PageModel
    {

        public decimal CartTotal => Cart?.Sum(i => i.SubTotal) ?? 0;

        [BindProperty]
        //public string Response { get; set; } = "";
        public List<Cart> Cart { get; set; } = new();

        public IActionResult  OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/AuthPage", new { returnUrl = "/CheckoutPage" });
            }

            LoadCartFromSession();
            return Page();
        }

        public ActionResult OnPostAddToCart(int productId, string name, decimal price, string imageUrl, int quantity, string source)
        {
            LoadCartFromSession();

            var existingItem = Cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Cart.Add(new Cart
                {
                    ProductId = productId,
                    Name = name,
                    Price = price,
                    ImageUrl = imageUrl,
                    Quantity = quantity
                });
            }

            SaveCartToSession();

            //return new JsonResult(new { success = true });
            //Response = "true";
            //Response = "false";
            //return RedirectToPage(new { added = true });
            //Response = "true";

            if (source == "shop")
            {
                // Stay on Shop
                TempData["CartMessage"] = "✅" +name+ " Item added to cart!";
                return RedirectToPage("/Shop");
            }
            else if (source == "viewproduct")
            {
                // Go to Cart
                return RedirectToPage("/CartPage");
            }

            return Page();
            
        }


        public IActionResult OnPostUpdateQuantity(int index, int quantity)
        {
            LoadCartFromSession();

            if (index >= 0 && index < Cart.Count)
            {
                if (quantity <= 0)
                    Cart.RemoveAt(index);
                else
                    Cart[index].Quantity = quantity;

                SaveCartToSession();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int index)
        {
            LoadCartFromSession();

            if (index >= 0 && index < Cart.Count)
            {
                Cart.RemoveAt(index);
                SaveCartToSession();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostClearCart()
        {
            HttpContext.Session.Remove("CartItems");
            return RedirectToPage();
        }


        private void LoadCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("CartItems");
            Cart = string.IsNullOrEmpty(cartJson)
                ? new List<Cart>()
                : JsonConvert.DeserializeObject<List<Cart>>(cartJson) ?? new List<Cart>();
        }

        private void SaveCartToSession()
        {
            var cartJson = JsonConvert.SerializeObject(Cart);
            HttpContext.Session.SetString("CartItems", cartJson);
        }




    }

}

