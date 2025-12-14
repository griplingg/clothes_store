namespace ClothesWeb.Models
{
    public class Size
    {
        public int Id   { get; set; }
        public string Name { get; set; }

        public ICollection<ProductSizes> ProductSizes { get; set; }
    }
}
