using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Имя обязательно.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Электронная почта обязательна.")]
        [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон обязателен.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
        public string Password { get; set; }
    }
}