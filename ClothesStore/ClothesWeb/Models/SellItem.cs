namespace ClothesWeb.Models
{
    public class SellItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Color { get; set; }
    }
}
