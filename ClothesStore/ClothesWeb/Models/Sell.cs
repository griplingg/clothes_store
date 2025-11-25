using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesWeb.Models
{
    public class Sell
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod {  get; set; }
        public string EmployeeId { get; set; }

        public ICollection<SellComposition> SaleComposition { get; set; } = new List<SellComposition>();

        
    }
    
}
