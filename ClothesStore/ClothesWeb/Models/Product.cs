

namespace ClothesWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string Sizes { get; set; }
        public int SupplierId { get; set; }
    }
}
