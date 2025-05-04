using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication2.DbContexts;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<OrdersController> _logger;
        private readonly TestdbContext _context;

        public OrdersController(
            IRepository<Order> orderRepository,
            ILogger<OrdersController> logger,
            TestdbContext context)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректные данные заказа.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                if (request.UserId <= 0)
                    return BadRequest(new { Message = "ID пользователя должен быть положительным числом." });

                if (request.CarId <= 0)
                    return BadRequest(new { Message = "ID автомобиля должен быть положительным числом." });

                if (request.DriverId <= 0)
                    return BadRequest(new { Message = "ID водителя должен быть положительным числом." });

                if (request.StartDate < DateTime.Now)
                    return BadRequest(new { Message = "Дата начала не может быть в прошлом." });

                if (request.EndDate <= request.StartDate)
                    return BadRequest(new { Message = "Дата окончания должна быть позже даты начала." });

                if (request.TotalCost <= 0)
                    return BadRequest(new { Message = "Стоимость должна быть положительной." });

                var carExists = await _context.Cars.AnyAsync(c => c.CarId == request.CarId);
                var driverExists = await _context.Drivers.AnyAsync(d => d.DriverId == request.DriverId);

                if (!carExists || !driverExists)
                {
                    return BadRequest(new
                    {
                        Message = carExists ? "Указанный водитель не найден" : "Указанный автомобиль не найден"
                    });
                }

                var isCarAvailable = await IsCarAvailableAsync(request.CarId, request.StartDate, request.EndDate);
                if (!isCarAvailable)
                {
                    return BadRequest(new { Message = "Автомобиль недоступен на указанные даты" });
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    CarId = request.CarId,
                    DriverId = request.DriverId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalCost = request.TotalCost,
                    Status = "Подтвержден"
                };

                await _orderRepository.AddAsync(order);
                await _context.SaveChangesAsync();

                var car = await _context.Cars.FindAsync(order.CarId);
                var driver = await _context.Drivers.FindAsync(order.DriverId);

                return Ok(new OrderResponse
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    CarId = order.CarId,
                    DriverId = order.DriverId,
                    Driver = new DriverResponse
                    {
                        DriverId = driver.DriverId,
                        FirstName = driver.FirstName,
                        LastName = driver.LastName,
                        Phone = driver.Phone,
                        LicenseNumber = driver.LicenseNumber,
                        IsAvailable = driver.IsAvailable ?? false
                    },
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    TotalCost = order.TotalCost,
                    Status = order.Status,
                    Car = new CarResponse(
                        car.CarId,
                        car.Brand,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.LicensePlate,
                        car.HourlyRate,
                        car.IsAvailable)
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при создании заказа");
                return StatusCode(500, new { Message = "Ошибка при сохранении заказа" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при создании заказа");
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<OrderResponse>>> GetUserOrders()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized(new { Message = "Необходима авторизация" });
                }

                var orders = await _context.Orders
                    .Include(o => o.Car)
                    .Include(o => o.Driver)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.StartDate)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return NotFound(new { Message = "Заказы не найдены." });
                }

                var response = orders.Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    CarId = o.CarId,
                    DriverId = o.DriverId,
                    Driver = new DriverResponse
                    {
                        DriverId = o.Driver.DriverId,
                        FirstName = o.Driver.FirstName,
                        LastName = o.Driver.LastName,
                        Phone = o.Driver.Phone,
                        LicenseNumber = o.Driver.LicenseNumber,
                        IsAvailable = o.Driver.IsAvailable ?? false
                    },
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    TotalCost = o.TotalCost,
                    Status = o.Status,
                    Car = new CarResponse(
                        o.Car.CarId,
                        o.Car.Brand,
                        o.Car.Model,
                        o.Car.Year,
                        o.Car.Color,
                        o.Car.LicensePlate,
                        o.Car.HourlyRate,
                        o.Car.IsAvailable)
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказов пользователя");
                return StatusCode(500, new { Message = "Ошибка при получении списка заказов" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Message = "ID заказа должен быть положительным числом." });

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { Message = "Заказ не найден" });
                }

                if (order.StartDate <= DateTime.Now.AddHours(24))
                {
                    return BadRequest(new { Message = "Заказ можно отменить не позднее чем за 24 часа до начала" });
                }

                order.Status = "Отменен";
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отмене заказа");
                return StatusCode(500, new { Message = "Ошибка при отмене заказа" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderResponse>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Car)
                    .Include(o => o.Driver)
                    .OrderByDescending(o => o.StartDate)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return NotFound(new { Message = "Заказы не найдены." });
                }

                var response = orders.Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    CarId = o.CarId,
                    DriverId = o.DriverId,
                    Driver = new DriverResponse
                    {
                        DriverId = o.Driver.DriverId,
                        FirstName = o.Driver.FirstName,
                        LastName = o.Driver.LastName,
                        Phone = o.Driver.Phone,
                        LicenseNumber = o.Driver.LicenseNumber,
                        IsAvailable = o.Driver.IsAvailable ?? false
                    },
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    TotalCost = o.TotalCost,
                    Status = o.Status,
                    Car = new CarResponse(
                        o.Car.CarId,
                        o.Car.Brand,
                        o.Car.Model,
                        o.Car.Year,
                        o.Car.Color,
                        o.Car.LicensePlate,
                        o.Car.HourlyRate,
                        o.Car.IsAvailable)
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех заказов");
                return StatusCode(500, new { Message = "Ошибка при загрузке списка заказов" });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Message = "ID заказа должен быть положительным числом." });

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Некорректные данные заказа.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                if (request.StartDate >= request.EndDate)
                    return BadRequest(new { Message = "Дата окончания должна быть позже даты начала." });

                if (request.TotalCost <= 0)
                    return BadRequest(new { Message = "Стоимость должна быть положительной." });

                var order = await _context.Orders
                    .Include(o => o.Car)
                    .Include(o => o.Driver)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    return NotFound(new { Message = "Заказ не найден." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = User.IsInRole("Admin");

                if (order.UserId != currentUserId && !isAdmin)
                {
                    return Forbid();
                }

                var isCarAvailable = await IsCarAvailableAsync(order.CarId, request.StartDate, request.EndDate, id);
                if (!isCarAvailable)
                {
                    return BadRequest(new { Message = "Автомобиль недоступен на указанные даты" });
                }

                order.StartDate = request.StartDate;
                order.EndDate = request.EndDate;
                order.TotalCost = request.TotalCost;

                if (!string.IsNullOrEmpty(request.Status))
                {
                    order.Status = request.Status;
                }

                await _context.SaveChangesAsync();

                return Ok(new OrderResponse
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    CarId = order.CarId,
                    DriverId = order.DriverId,
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    TotalCost = order.TotalCost,
                    Status = order.Status,
                    Car = new CarResponse(
                        order.Car.CarId,
                        order.Car.Brand,
                        order.Car.Model,
                        order.Car.Year,
                        order.Car.Color,
                        order.Car.LicensePlate,
                        order.Car.HourlyRate,
                        order.Car.IsAvailable),
                    Driver = new DriverResponse
                    {
                        DriverId = order.Driver.DriverId,
                        FirstName = order.Driver.FirstName,
                        LastName = order.Driver.LastName,
                        Phone = order.Driver.Phone,
                        LicenseNumber = order.Driver.LicenseNumber,
                        IsAvailable = order.Driver.IsAvailable ?? false
                    }
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при обновлении заказа {OrderId}", id);
                return StatusCode(500, new { Message = "Ошибка при сохранении изменений в базе данных" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при обновлении заказа {OrderId}", id);
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
            }
        }

        private async Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate, int? excludeOrderId = null)
        {
            var query = _context.Orders
                .Where(o => o.CarId == carId &&
                           o.Status != "Отменен" &&
                           o.OrderId != excludeOrderId);

            var conflictingOrders = await query
                .Where(o => (startDate >= o.StartDate && startDate <= o.EndDate) ||
                           (endDate >= o.StartDate && endDate <= o.EndDate) ||
                           (startDate <= o.StartDate && endDate >= o.EndDate))
                .ToListAsync();

            return !conflictingOrders.Any();
        }
    }
}