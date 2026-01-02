using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFSApp.Pages
{
  
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<LFSApp.Model.Product> Products { get; set; } = new();


        public async Task OnGet()
        {
            Products = await _context.Products.Include(p => p.Images).OrderByDescending(p => p.Id).Take(4).ToListAsync();

        }


    }

   
   

}


