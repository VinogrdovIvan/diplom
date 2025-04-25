using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Идентификатор пользователя обязателен.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Идентификатор автомобиля обязателен.")]
        public int CarId { get; set; }

        public int? DriverId { get; set; } 

        [Required(ErrorMessage = "Дата начала аренды обязательна.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата окончания аренды обязательна.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Общая стоимость обязательна.")]
        [Range(0, double.MaxValue, ErrorMessage = "Стоимость должна быть положительной.")]
        public decimal TotalCost { get; set; }
    }
}