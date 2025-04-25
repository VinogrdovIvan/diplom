using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using System.Linq;
using System.Threading.Tasks;

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


        // Получить список всех автомобилей.

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
                return StatusCode(500, new { Message = "Произошла ошибка при получении списка автомобилей.", Error = ex.Message });
            }
        }


        // Получить список доступных автомобилей.

        [HttpGet("available")]
        [Authorize]
        public async Task<IActionResult> GetAvailableCars()
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();
                var availableCars = cars.Where(c => c.IsAvailable == true).ToList();

              
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
                return StatusCode(500, new { Message = "Произошла ошибка при получении списка доступных автомобилей.", Error = ex.Message });
            }
        }

        // Добавить новый автомобиль.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] AddCarRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Некорректные данные автомобиля.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
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
                return Ok(new CarAddedResponse { CarId = car.CarId, Message = "Автомобиль успешно добавлен." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при добавлении автомобиля.", Error = ex.Message });
            }
        }


        // Обновить информацию об автомобиле .

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Некорректные данные автомобиля.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
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
                return Ok(new CarUpdatedResponse { CarId = car.CarId, Message = "Информация об автомобиле успешно обновлена." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при обновлении автомобиля.", Error = ex.Message });
            }
        }


        // Удалить автомобиль.

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
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
                return StatusCode(500, new { Message = "Произошла ошибка при удалении автомобиля.", Error = ex.Message });
            }
        }



        [HttpPost("calculate-cost")]
        [Authorize]
        public async Task<ActionResult<CalculateCostResponse>> CalculateCost([FromBody] CalculateCostRequest request)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(request.CarId);
                if (car == null)
                    return NotFound(new { Message = "Автомобиль не найден." });

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
                return StatusCode(500, new { Message = "Ошибка при расчете стоимости", Error = ex.Message });
            }
        }
    }
}