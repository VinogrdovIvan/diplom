using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class PaymentsController : ControllerBase
{
    private readonly IRepository<Payment> _paymentRepository; 

    public PaymentsController(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository; 
    }


    // Получить список всех платежей.

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var payments = await _paymentRepository.GetAllAsync();
            if (payments == null || !payments.Any())
            {
                return NotFound(new { Message = "Платежи не найдены." }); 
            }

            return Ok(payments); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении списка платежей.", Error = ex.Message });
        }
    }

    // Получить платеж по его идентификатору.

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { Message = "Платеж не найден." }); 
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении платежа.", Error = ex.Message });
        }
    }


    // Создать новый платеж.

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payment payment)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные платежа.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            await _paymentRepository.AddAsync(payment);

            return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, payment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при создании платежа.", Error = ex.Message });
        }
    }


    // Обновить существующий платеж.

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Payment payment)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные платежа.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            if (id != payment.PaymentId)
            {
                return BadRequest(new { Message = "Идентификатор платежа в URL не совпадает с идентификатором в теле запроса." });
            }

            var existingPayment = await _paymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return NotFound(new { Message = "Платеж не найден." }); 
            }

            existingPayment.Amount = payment.Amount;
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.OrderId = payment.OrderId;

            await _paymentRepository.UpdateAsync(existingPayment); 

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при обновлении платежа.", Error = ex.Message });
        }
    }


    // Удалить платеж по его идентификатору.

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { Message = "Платеж не найден." }); 
            }

            await _paymentRepository.DeleteAsync(id); 

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при удалении платежа.", Error = ex.Message });
        }
    }
}