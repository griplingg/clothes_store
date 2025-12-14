namespace ClothesWeb.Models
{
    public class ReturnSelectViewModel
    {
        public int SellItemId { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public int AvailableCount { get; set; }
    }

    public class SellWithItemsForReturnViewModel
    {
        public int SellId { get; set; }
        public DateTime SellDate { get; set; }
        public List<SellItemForReturnViewModel> Items { get; set; } = new();
    }

    public class SellItemForReturnViewModel
    {
        public int SellItemId { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public int AvailableForReturn { get; set; }
    }

}
