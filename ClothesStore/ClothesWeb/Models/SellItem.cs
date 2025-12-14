namespace ClothesWeb.Models
{
    public class SellItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }

        public int SellId { get; set; }


        public Sell Sell { get; set; }
        public Product Product { get; set; }
        public Size Size { get; set; }

        public ICollection<ReturnProduct> Returns { get; set; }
    }
}
