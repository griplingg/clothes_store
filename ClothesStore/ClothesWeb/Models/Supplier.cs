
using System.ComponentModel.DataAnnotations;

namespace ClothesWeb.Models
{
    public class Supplier
    {
        public int Id { get; set; }
       [Required(ErrorMessage = "Введите название организации!")]
        public string OrganizationName { get; set; }
        [Required(ErrorMessage = "Введите контактное лицо!!")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Введите контактный адрес электронной почты!")]
        [EmailAddress]
        public string ContactMail { get; set; }

        [Required(ErrorMessage = "Введите контактный номер телефона!")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        public string ContactPhone { get; set; }

        public bool? IsDeleted { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
