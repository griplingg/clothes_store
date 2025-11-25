namespace ClothesWeb.Models
{
    public class SellComposition
    {
        public int SellItemId { get; set; }
        public int SellId { get; set; }

        public SellItem SellItem { get; set; }
        public Sell Sell { get; set; }
    }
}
