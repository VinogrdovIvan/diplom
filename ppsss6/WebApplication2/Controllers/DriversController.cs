using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using System.ComponentModel.DataAnnotations;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var drivers = await _driverRepository.GetAllAsync();
            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { Message = "Водители не найдены." });
            }

            return Ok(drivers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении списка водителей.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID водителя должен быть положительным числом." });
            }

            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                return NotFound(new { Message = "Водитель не найден." });
            }

            return Ok(driver);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении водителя.",
                Error = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Driver driver)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные водителя.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (string.IsNullOrWhiteSpace(driver.FirstName))
                return BadRequest(new { Message = "Имя водителя обязательно." });

            if (string.IsNullOrWhiteSpace(driver.LastName))
                return BadRequest(new { Message = "Фамилия водителя обязательна." });

            if (string.IsNullOrWhiteSpace(driver.LicenseNumber))
                return BadRequest(new { Message = "Номер водительского удостоверения обязателен." });

            if (driver.HireDate == default)
                return BadRequest(new { Message = "Дата приема на работу обязательна." });

            await _driverRepository.AddAsync(driver);

            return CreatedAtAction(nameof(GetById), new { id = driver.DriverId }, driver);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при создании водителя.",
                Error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Driver driver)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID водителя должен быть положительным числом." });

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные водителя.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (id != driver.DriverId)
            {
                return BadRequest(new
                {
                    Message = "Идентификатор водителя в URL не совпадает с идентификатором в теле запроса."
                });
            }

            if (string.IsNullOrWhiteSpace(driver.FirstName))
                return BadRequest(new { Message = "Имя водителя обязательно." });

            if (string.IsNullOrWhiteSpace(driver.LastName))
                return BadRequest(new { Message = "Фамилия водителя обязательна." });

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
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при обновлении водителя.",
                Error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID водителя должен быть положительным числом." });

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
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при удалении водителя.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableDrivers([FromServices] IRepository<Order> orderRepository)
    {
        try
        {
            var drivers = await _driverRepository.GetAllAsync();
            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { Message = "Водители не найдены." });
            }

            var activeOrders = (await orderRepository.GetAllAsync())
                .Where(o => o.Status != "Отменен" &&
                            o.EndDate > DateTime.Now)
                .ToList();

            var busyDriverIds = activeOrders
                .Select(o => o.DriverId)
                .Distinct()
                .ToList();

            var availableDrivers = drivers
                .Where(d => d.IsAvailable == true &&
                           !busyDriverIds.Contains(d.DriverId))
                .ToList();

            if (!availableDrivers.Any())
            {
                return NotFound(new { Message = "Нет доступных водителей." });
            }

            return Ok(availableDrivers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Ошибка при получении списка водителей.",
                Error = ex.Message
            });
        }
    }
}