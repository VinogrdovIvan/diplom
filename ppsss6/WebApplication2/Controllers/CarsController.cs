using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly IRepository<Car> _carRepository;

        public CarsController(IRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();
                if (cars == null || !cars.Any())
                {
                    return NotFound(new { Message = "Автомобили не найдены." });
                }

                var carsResponse = cars
                    .Select(car => new CarResponse(
                        car.CarId,
                        car.Brand,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.LicensePlate,
                        car.HourlyRate,
                        car.IsAvailable))
                    .ToList();

                return Ok(carsResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при получении списка автомобилей.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("available")]
        [Authorize]
        public async Task<IActionResult> GetAvailableCars()
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();
                var availableCars = cars.Where(c => c.IsAvailable == true).ToList();

                if (!availableCars.Any())
                {
                    return NotFound(new { Message = "Нет доступных автомобилей." });
                }

                var carsResponse = availableCars
                    .Select(car => new CarResponse(
                        car.CarId,
                        car.Brand,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.LicensePlate,
                        car.HourlyRate,
                        car.IsAvailable))
                    .ToList();

                return Ok(carsResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при получении списка доступных автомобилей.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCarById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "ID автомобиля должен быть положительным числом." });
                }

                var car = await _carRepository.GetByIdAsync(id);
                if (car == null)
                {
                    return NotFound(new { Message = "Автомобиль не найден." });
                }

                var carResponse = new CarResponse(
                    car.CarId,
                    car.Brand,
                    car.Model,
                    car.Year,
                    car.Color,
                    car.LicensePlate,
                    car.HourlyRate,
                    car.IsAvailable);

                return Ok(carResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при получении автомобиля.",
                    Error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] AddCarRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректные данные автомобиля.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Brand))
                {
                    return BadRequest(new { Message = "Марка автомобиля обязательна." });
                }

                if (string.IsNullOrWhiteSpace(request.Model))
                {
                    return BadRequest(new { Message = "Модель автомобиля обязательна." });
                }

                if (request.Year < 1900 || request.Year > DateTime.Now.Year + 1)
                {
                    return BadRequest(new
                    {
                        Message = $"Год выпуска должен быть между 1900 и {DateTime.Now.Year + 1}."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.LicensePlate))
                {
                    return BadRequest(new { Message = "Госномер обязателен." });
                }

                if (request.HourlyRate <= 0)
                {
                    return BadRequest(new { Message = "Стоимость аренды должна быть больше 0." });
                }

                var car = new Car
                {
                    Brand = request.Brand,
                    Model = request.Model,
                    Year = request.Year,
                    Color = request.Color,
                    LicensePlate = request.LicensePlate,
                    HourlyRate = request.HourlyRate,
                    IsAvailable = request.IsAvailable
                };

                await _carRepository.AddAsync(car);
                return Ok(new CarAddedResponse
                {
                    CarId = car.CarId,
                    Message = "Автомобиль успешно добавлен."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при добавлении автомобиля.",
                    Error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "ID автомобиля должен быть положительным числом." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректные данные автомобиля.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Brand))
                {
                    return BadRequest(new { Message = "Марка автомобиля обязательна." });
                }

                if (string.IsNullOrWhiteSpace(request.Model))
                {
                    return BadRequest(new { Message = "Модель автомобиля обязательна." });
                }

                var car = await _carRepository.GetByIdAsync(id);
                if (car == null)
                {
                    return NotFound(new { Message = "Автомобиль не найден." });
                }

                car.Brand = request.Brand;
                car.Model = request.Model;
                car.Year = request.Year;
                car.Color = request.Color;
                car.LicensePlate = request.LicensePlate;
                car.HourlyRate = request.HourlyRate;
                car.IsAvailable = request.IsAvailable;

                await _carRepository.UpdateAsync(car);
                return Ok(new CarUpdatedResponse
                {
                    CarId = car.CarId,
                    Message = "Информация об автомобиле успешно обновлена."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при обновлении автомобиля.",
                    Error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "ID автомобиля должен быть положительным числом." });
                }

                var car = await _carRepository.GetByIdAsync(id);
                if (car == null)
                {
                    return NotFound(new { Message = "Автомобиль не найден." });
                }

                await _carRepository.DeleteAsync(id);
                return Ok(new { Message = "Автомобиль успешно удален." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Произошла ошибка при удалении автомобиля.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("calculate-cost")]
        [Authorize]
        public async Task<ActionResult<CalculateCostResponse>> CalculateCost([FromBody] CalculateCostRequest request)
        {
            try
            {
                if (request.CarId <= 0)
                {
                    return BadRequest(new { Message = "ID автомобиля должен быть положительным числом." });
                }

                if (request.StartDateTime >= request.EndDateTime)
                {
                    return BadRequest(new { Message = "Дата окончания должна быть позже даты начала." });
                }

                var car = await _carRepository.GetByIdAsync(request.CarId);
                if (car == null)
                {
                    return NotFound(new { Message = "Автомобиль не найден." });
                }

                var duration = request.EndDateTime - request.StartDateTime;
                var hours = (decimal)Math.Ceiling(duration.TotalHours);
                var totalCost = hours * car.HourlyRate;

                return Ok(new CalculateCostResponse
                {
                    TotalCost = totalCost,
                    Message = "Стоимость рассчитана успешно"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Ошибка при расчете стоимости",
                    Error = ex.Message
                });
            }
        }
    }
}