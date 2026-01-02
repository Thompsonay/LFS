using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LFSApp.Pages
{
    public class AdminDashModel : PageModel
    {
        private readonly LFSApp.Dbcontext.ApplicationDbContext _context;

        public AdminDashModel(LFSApp.Dbcontext.ApplicationDbContext context)
        {
            _context = context;
        }

        // Last 5 orders to show in the revenue overview
        public List<LFSApp.Model.Order> RecentOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load last 5 orders from the database (newest first)
            RecentOrders = await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
