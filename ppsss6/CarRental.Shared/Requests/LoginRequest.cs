using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Электронная почта обязательна.")]
        [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        public string Password { get; set; }
    }
}