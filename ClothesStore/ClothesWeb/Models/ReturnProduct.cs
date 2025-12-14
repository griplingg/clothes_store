namespace ClothesWeb.Models
{
    public class ReturnProduct
    {
        public int Id { get; set; }
        public int SellId { get; set; }
        public DateTime Date { get; set; }
        public string EmployeeId {  get; set; }
        public int SellItemId { get; set; }

        public int StatusId { get; set; }
        public string Reason { get; set; }

        public ReturnStatus Status { get; set; }
        public Sell Sell { get; set; }
        public SellItem SellItem { get; set; }
    }
}
