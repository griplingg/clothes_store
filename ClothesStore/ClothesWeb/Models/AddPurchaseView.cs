// Models/ViewModels/SellViewModels.cs

using System.ComponentModel.DataAnnotations;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClothesWeb.Models
{
    // Модель для одной позиции (строки) в продаже
    public class AddPurchaseItemViewModel
    {
        // Эти поля будут приходить с формы (скрытые и видимые)
        [Required(ErrorMessage = "Выберите товар")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Выберите размер")]
        public int SizeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }

        // Дополнительные поля для отображения или внутренней логики
        [BindNever]
        public string? ProductName { get; set; }
        [BindNever]
        public string? SizeName { get; set; }
    }

    // Общая модель для всей страницы добавления продажи
    public class AddPurchaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите метод оплаты")]
        [Display(Name = "Метод оплаты")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Добавьте хотя бы один товар")]
        public List<AddPurchaseItemViewModel> Items { get; set; } = new List<AddPurchaseItemViewModel>();

        // Список товаров для формирования главного выпадающего списка
        public IEnumerable<Product>? AllProducts { get; set; }
    }
}