using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class DriversController : ControllerBase
{
    private readonly IRepository<Driver> _driverRepository; 

    public DriversController(IRepository<Driver> driverRepository)
    {
        _driverRepository = driverRepository; 
    }


    // Получить список всех водителей.

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var drivers = await _driverRepository.GetAllAsync();
 

            return Ok(drivers); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении списка водителей.", Error = ex.Message });
        }
    }


    // Получить водителя по его идентификатору.

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                return NotFound(new { Message = "Водитель не найден." });
            }

            return Ok(driver); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при получении водителя.", Error = ex.Message });
        }
    }


    // Создать нового водителя.

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Driver driver)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные водителя.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            await _driverRepository.AddAsync(driver); 

            return CreatedAtAction(nameof(GetById), new { id = driver.DriverId }, driver); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при создании водителя.", Error = ex.Message });
        }
    }


    // Обновить существующего водителя.

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Driver driver)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные водителя.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            if (id != driver.DriverId)
            {
                return BadRequest(new { Message = "Идентификатор водителя в URL не совпадает с идентификатором в теле запроса." });
            }

            var existingDriver = await _driverRepository.GetByIdAsync(id);
            if (existingDriver == null)
            {
                return NotFound(new { Message = "Водитель не найден." }); 
            }

            existingDriver.FirstName = driver.FirstName;
            existingDriver.LastName = driver.LastName;
            existingDriver.Phone = driver.Phone;
            existingDriver.LicenseNumber = driver.LicenseNumber;
            existingDriver.HireDate = driver.HireDate;
            existingDriver.IsAvailable = driver.IsAvailable;

            await _driverRepository.UpdateAsync(existingDriver);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при обновлении водителя.", Error = ex.Message });
        }
    }

    // Удалить водителя по его идентификатору.

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                return NotFound(new { Message = "Водитель не найден." }); 
            }

            await _driverRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при удалении водителя.", Error = ex.Message });
        }
    }
}