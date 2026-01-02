using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFSApp.Model
{
    public class ProductVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int ImageId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
