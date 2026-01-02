using LFSApp.Dbcontext;
using LFSApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LFSApp.Pages
{
    public class ProductItemModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProductItemModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public List<IFormFile> ProductImages { get; set; }
        public ProductImage ProductImage { get; set; }

        //[BindProperty]
        public List<LFSApp.Model.Product> GetProduct { get; set; } = new();

        [BindProperty]
        public ProductVm ProductVm { get; set; }

        /*
         * ProductVm vm = new();
           Product p = new();
           p.name = vm.name;


            _context.Products.Add(p)
         * 
         * 
         * */
        public async Task<IActionResult> OnGet()
        {
            Categories = await _context.Categories.ToListAsync();

            GetProduct = await _context.Products.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Create the product
            Product p = new();
            p.Name = ProductVm.Name;
            p.Price = ProductVm.Price;
            p.CategoryId = ProductVm.CategoryId;
            p.Description = ProductVm.Description;

            _context.Products.Add(p);
            await _context.SaveChangesAsync(); // Save first to get p.Id

            int? firstImageId = null;
            List<string> savedFilePaths = new();

            // 2. Handle uploaded images
            if (ProductImages != null && ProductImages.Count > 0)
            {
                foreach (var image in ProductImages)
                {
                    if (image.Length > 0)
                    {
                        var fileExt = Path.GetExtension(image.FileName);
                        var fileName = Guid.NewGuid() + fileExt;
                        var uploadPath = Path.Combine("wwwroot", "Uploads");

                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var imagePath = "/uploads/" + fileName;
                        savedFilePaths.Add(imagePath);

                        // Save in ProductImages table
                        var productImage = new ProductImage
                        {
                            ProductId = p.Id,
                            ImageUrl = imagePath
                        };

                        _context.ProductImages.Add(productImage);
                        await _context.SaveChangesAsync();

                        if (firstImageId == null)
                            firstImageId = productImage.Id;
                    }
                }
            }

            // 3. Save all images as comma-separated string in Product.Images
            //if (savedFilePaths.Any())
            //{
            //    p.Images = string.Join(",", savedFilePaths);
            //}

            // 4. Save the first image as the main ImageId
            if (firstImageId != null)
            {
                p.ImageId = firstImageId.Value;
            }

            _context.Products.Update(p);
            await _context.SaveChangesAsync();

            return RedirectToPage("ProductItem");
        }




        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // refresh page
        }

    }
}
