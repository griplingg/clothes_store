namespace ClothesWeb.Models
{
    public class ProductSizes
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }

        // навигационные свойства
        public Product Product { get; set; }
        public Size Size { get; set; }
    }
}
