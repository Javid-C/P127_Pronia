using P127_Pronia.Models.Base;

namespace P127_Pronia.Models
{
    public class BasketItem:BaseEntity
    {
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
