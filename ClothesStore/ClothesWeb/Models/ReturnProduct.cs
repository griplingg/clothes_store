namespace ClothesWeb.Models
{
    public class ReturnProduct
    {
        public int Id { get; set; }
        public int SellId { get; set; }
        public DateTime Date { get; set; }
        public int ProductId {  get; set; }
        public decimal amount { get; set; }
        public int SellItemId { get; set; }
    }
}
