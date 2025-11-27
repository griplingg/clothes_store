
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClothesWeb.Models
{

    public class SellViewModel
    {
        [Required]
        public string PaymentMethod { get; set; }

        public List<SellItemViewModel> Items { get; set; } = new List<SellItemViewModel>();
    }

    public class SellItemViewModel
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }

}
