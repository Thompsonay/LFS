using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFSApp.Pages
{
    public class ShopModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ShopModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<LFSApp.Model.Product> Products { get; set; } = new();
        public Product pro1 { get; set; } = new();

        [BindProperty]
        public ProductImage ProductImage { get; set; }
        [BindProperty]
        public List<LFSApp.Model.Category> Categories { get; set; }
     


        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }


        public async Task OnGetAsync()
        {

            Products = await _context.Products.Include(p => p.Images).OrderByDescending(p => p.Id).ToListAsync();


            Categories = _context.Categories.ToList();

            //Filter products based on selected category name
            Products = string.IsNullOrEmpty(SelectedCategory)
               ? _context.Products.Include(p => p.Category).ToList()
               : _context.Products
                   .Include(p => p.Category)
                   .Where(p => p.Category.CategoryName == SelectedCategory)
                   .ToList();


        }




    }
}
