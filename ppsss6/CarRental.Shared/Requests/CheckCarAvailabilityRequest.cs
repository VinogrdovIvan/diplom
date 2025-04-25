using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class CheckCarAvailabilityRequest
    {
        [Required(ErrorMessage = "Идентификатор автомобиля обязателен.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Дата начала аренды обязательна.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата окончания аренды обязательна.")]
        public DateTime EndDate { get; set; }
    }
}