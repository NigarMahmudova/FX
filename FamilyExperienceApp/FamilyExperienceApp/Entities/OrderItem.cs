using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyExperienceApp.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Count { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitSalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountedPrice { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
        public Size Size { get; set; }
    }
}
