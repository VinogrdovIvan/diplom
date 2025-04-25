using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class UpdateCarRequest
    {
        [Required(ErrorMessage = "Марка автомобиля обязательна.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Модель автомобиля обязательна.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Год выпуска обязателен.")]
        [Range(1900, 2100, ErrorMessage = "Некорректный год выпуска.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Цвет автомобиля обязателен.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Номерной знак обязателен.")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Стоимость аренды за час обязательна.")]
        [Range(0, double.MaxValue, ErrorMessage = "Стоимость аренды должна быть положительной.")]
        public decimal HourlyRate { get; set; } 

        public bool IsAvailable { get; set; }
    }
}