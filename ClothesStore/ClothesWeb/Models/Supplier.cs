
namespace ClothesWeb.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public string ContactMail { get; set; }
        public string ContactPhone { get; set; }

        
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
