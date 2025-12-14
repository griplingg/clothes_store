using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCLothesWeb
{
    // Простая замена для отсутствующих ViewModel


    using System.ComponentModel.DataAnnotations;
    using ClothesWeb.Models;
    using global::ClothesWeb.Models;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    namespace ClothesWeb.Models
    {

        public class AddPurchaseItemViewModel
        {

            [Required(ErrorMessage = "Выберите товар")]
            public int ProductId { get; set; }

            [Required(ErrorMessage = "Выберите размер")]
            public int SizeId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
            public int Quantity { get; set; }

            [BindNever]
            public string? ProductName { get; set; }
            [BindNever]
            public string? SizeName { get; set; }
        }


        public class AddPurchaseViewModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Укажите метод оплаты")]
            [Display(Name = "Метод оплаты")]
            public string PaymentMethod { get; set; }

            [Required(ErrorMessage = "Добавьте хотя бы один товар")]
            public List<AddPurchaseItemViewModel> Items { get; set; } = new List<AddPurchaseItemViewModel>();


            public IEnumerable<Product>? AllProducts { get; set; }
        }
    }
}
