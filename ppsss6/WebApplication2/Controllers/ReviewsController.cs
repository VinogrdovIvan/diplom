using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IRepository<Review> _reviewRepository;

    public ReviewsController(IRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository; 
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
                return NotFound(new { Message = "Отзывы не найдены." });
            }

            return Ok(reviews); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении списка отзывов.", Error = ex.Message });
        }
    }


    // Получить отзыв по его идентификатору.

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound(new { Message = "Отзыв не найден." }); 
            }

            return Ok(review);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении отзыва.", Error = ex.Message });
        }
    }

    // Создать новый отзыв.

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Review review)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные отзыва.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            await _reviewRepository.AddAsync(review);

            return CreatedAtAction(nameof(GetById), new { id = review.ReviewId }, review); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при создании отзыва.", Error = ex.Message });
        }
    }


    // Обновить существующий отзыв.

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Review review)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные отзыва.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            if (id != review.ReviewId)
            {
                return BadRequest(new { Message = "Идентификатор отзыва в URL не совпадает с идентификатором в теле запроса." });
            }

            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound(new { Message = "Отзыв не найден." }); 
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.ReviewDate = review.ReviewDate;

            await _reviewRepository.UpdateAsync(existingReview); 

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при обновлении отзыва.", Error = ex.Message });
        }
    }

    // Удалить отзыв по его идентификатору.

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound(new { Message = "Отзыв не найден." });
            }

            await _reviewRepository.DeleteAsync(id);

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при удалении отзыва.", Error = ex.Message });
        }
    }
}