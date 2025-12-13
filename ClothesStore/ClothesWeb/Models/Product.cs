

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ClothesWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
       
        public string? Sizes { get; set; }

        public string ArticleNumber { get; set; }

        public int SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public bool IsDeleted { get; set; }

        public string? ImagePath { get; set; }


        public ICollection<ProductSizes> ProductSizes { get; set; } = new List<ProductSizes>();

        [BindNever]
        [NotMapped]
        public Supplier? Supplier { get; set; }
        public Category? Category { get; set; }
    }
}
