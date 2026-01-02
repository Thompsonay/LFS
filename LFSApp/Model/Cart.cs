using System.ComponentModel.DataAnnotations.Schema;

namespace LFSApp.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        // Option A: computed but with explicit backing field (rarely needed)
        [Column(TypeName = "decimal(18,2)")]
        private decimal? _subTotalOverride;
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal
        {
            get => _subTotalOverride ?? Price * Quantity;
            set => _subTotalOverride = value; // now assignable
        }
    }
}
