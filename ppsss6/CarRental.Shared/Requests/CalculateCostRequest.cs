using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class CalculateCostRequest
    {
        [Required(ErrorMessage = "Идентификатор автомобиля обязателен.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Дата и время начала аренды обязательны.")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Дата и время окончания аренды обязательны.")]
        public DateTime EndDateTime { get; set; }
    }
}