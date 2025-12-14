namespace ClothesWeb.Models
{
    public class ReturnItem
    {
        public int Id { get; set; }
        public int ReturnId { get; set; }
        public int Quantity { get; set; }
        public int SellItemId { get; set; }
        

        public SellItem SellItem { get; set; }
        public ReturnProduct Return { get; set; }

    }
}
