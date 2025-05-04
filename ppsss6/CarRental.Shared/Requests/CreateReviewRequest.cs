using System.ComponentModel.DataAnnotations;

namespace CarRental.Shared.Requests
{
    public class CreateReviewRequest
    {
        [Required(ErrorMessage = "Идентификатор заказа обязателен")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Оценка обязательна")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Комментарий не должен превышать 500 символов")]
        public string? Comment { get; set; }
    }
}