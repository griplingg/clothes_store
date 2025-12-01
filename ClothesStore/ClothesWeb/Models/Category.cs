using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
namespace ClothesWeb.Models

{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }

        [Required]
      
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}