using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFSApp.Pages
{
    public class AdminProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminProductModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Category Category { get; set; }

        public List<Category> Categories { get; set; } = new();

        //public Category Category { get; set; };
        public async Task<ActionResult> OnGet()
        {
            Categories = await _context.Categories.ToListAsync();

            return Page();

        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Page == null)
            {
                return Page();
            }

            _context.Add(Category);
            int result = await _context.SaveChangesAsync();



            if (result > 0)
            {
                ModelState.AddModelError("Category.CategoryName", "Saved in database successfully.");
                return Page();
            }

            return RedirectToPage("./AdminProduct");

        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var Category = await _context.Categories.FindAsync(id);

            if (Category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // refresh page
        }
    }
}
