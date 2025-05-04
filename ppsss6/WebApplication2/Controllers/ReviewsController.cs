using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApplication2.Entities;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            IRepository<Review> reviewRepository,
            IRepository<Order> orderRepository,
            ILogger<ReviewsController> logger)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        // Получить список всех отзывов.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllAsync();
                if (reviews == null || !reviews.Any())
                {
                    return NotFound(new
                    {
                        Message = "Отзывы не найдены.",
                        Details = "В системе пока нет ни одного отзыва."
                    });
                }

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка отзывов");
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при получении списка отзывов.",
                    Error = ex.Message,
                    Details = "Попробуйте повторить запрос позже."
                });
            }
        }

        // Получить отзыв по его идентификатору.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректный идентификатор отзыва.",
                        Details = "Идентификатор должен быть положительным числом."
                    });
                }

                var review = await _reviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    return NotFound(new
                    {
                        Message = "Отзыв не найден.",
                        Details = $"Отзыв с ID {id} не существует в системе."
                    });
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении отзыва с ID {ReviewId}", id);
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при получении отзыва.",
                    Error = ex.Message,
                    Details = "Попробуйте повторить запрос позже."
                });
            }
        }

        // Получить отзывы по ID заказа
        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректный идентификатор заказа.",
                        Details = "Идентификатор должен быть положительным числом."
                    });
                }

                var reviews = (await _reviewRepository.GetAllAsync())
                    .Where(r => r.OrderId == orderId)
                    .ToList();

                if (!reviews.Any())
                {
                    return NotFound(new
                    {
                        Message = "Отзывы не найдены для данного заказа",
                        Details = $"Для заказа с ID {orderId} нет отзывов."
                    });
                }

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении отзывов для заказа {OrderId}", orderId);
                return StatusCode(500, new
                {
                    Message = "Ошибка сервера",
                    Error = ex.Message,
                    Details = "Попробуйте повторить запрос позже."
                });
            }
        }

        // Создать новый отзыв.
        [HttpPost]
        public async Task<ActionResult<ReviewResponse>> Create([FromBody] CreateReviewRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректные данные отзыва.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors),
                        Details = "Проверьте правильность заполнения всех полей."
                    });
                }

                if (request.OrderId <= 0)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректный идентификатор заказа.",
                        Details = "Идентификатор заказа должен быть положительным числом."
                    });
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректная оценка.",
                        Details = "Оценка должна быть в диапазоне от 1 до 5."
                    });
                }

                if (!string.IsNullOrEmpty(request.Comment) && request.Comment.Length > 500)
                {
                    return BadRequest(new
                    {
                        Message = "Слишком длинный комментарий.",
                        Details = "Комментарий не должен превышать 500 символов."
                    });
                }

                var orderExists = await _orderRepository.ExistsAsync(o => o.OrderId == request.OrderId);
                if (!orderExists)
                {
                    return BadRequest(new
                    {
                        Message = "Указанный заказ не существует",
                        Details = $"Заказ с ID {request.OrderId} не найден в системе."
                    });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userMadeOrder = await _orderRepository.ExistsAsync(o =>
                    o.OrderId == request.OrderId && o.UserId == userId);

                if (!userMadeOrder)
                {
                    return BadRequest(new
                    {
                        Message = "Недостаточно прав для создания отзыва",
                        Details = "Вы можете оставлять отзывы только на свои заказы."
                    });
                }

                var review = new Review
                {
                    OrderId = request.OrderId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    ReviewDate = DateTime.UtcNow
                };

                await _reviewRepository.AddAsync(review);

                return Ok(new ReviewResponse
                {
                    ReviewId = review.ReviewId,
                    OrderId = review.OrderId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewDate = review.ReviewDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании отзыва");
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при создании отзыва",
                    Error = ex.Message,
                    Details = "Попробуйте повторить операцию позже."
                });
            }
        }

        // Удалить отзыв по его идентификатору.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректный идентификатор отзыва.",
                        Details = "Идентификатор должен быть положительным числом."
                    });
                }

                var review = await _reviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    return NotFound(new
                    {
                        Message = "Отзыв не найден.",
                        Details = $"Отзыв с ID {id} не существует в системе."
                    });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = await _orderRepository.GetByIdAsync(review.OrderId);

                if (order == null || order.UserId != userId)
                {
                    return Forbid();
                }

                await _reviewRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении отзыва с ID {ReviewId}", id);
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при удалении отзыва.",
                    Error = ex.Message,
                    Details = "Попробуйте повторить операцию позже."
                });
            }
        }
    }

    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Идентификатор заказа обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "Идентификатор заказа должен быть положительным числом.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Оценка обязательна.")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Комментарий не должен превышать 500 символов.")]
        public string Comment { get; set; }
    }
}