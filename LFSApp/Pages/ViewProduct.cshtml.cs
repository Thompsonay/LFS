using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFSApp.Pages
{
    public class ViewProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewProductModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = new();
        [BindProperty]
        public ProductImage ProductImage { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);



            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
