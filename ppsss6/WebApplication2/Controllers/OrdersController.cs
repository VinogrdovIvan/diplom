using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.DbContexts;

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
                if (request.UserId <= 0)
                    return BadRequest(new { Message = "Неверный идентификатор пользователя" });

                _logger.LogInformation("Создание заказа для пользователя {UserId}, автомобиль {CarId}",
                    request.UserId, request.CarId);

                var isCarAvailable = await IsCarAvailableAsync(request.CarId, request.StartDate, request.EndDate);
                if (!isCarAvailable)
                {
                    return BadRequest(new { Message = "Автомобиль недоступен на указанные даты" });
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    CarId = request.CarId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalCost = request.TotalCost,
                    Status = "Подтвержден",
                    DriverId = request.DriverId
                };

                await _orderRepository.AddAsync(order);
                _logger.LogInformation("Заказ успешно создан с ID {OrderId}", order.OrderId);

                var car = await _context.Cars.FindAsync(order.CarId);

                return Ok(new OrderResponse
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    CarId = order.CarId,
                    DriverId = order.DriverId ?? 0,
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    TotalCost = order.TotalCost,
                    Status = order.Status,
                    Car = car != null ? new CarResponse(
                        car.CarId,
                        car.Brand,
                        car.Model,
                        car.Year,
                        car.Color,
                        car.LicensePlate,
                        car.HourlyRate,
                        car.IsAvailable) : null
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при создании заказа");
                return StatusCode(500, new { Message = "Ошибка при сохранении заказа в базу данных" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при создании заказа");
                return StatusCode(500, new { Message = "Произошла непредвиденная ошибка при создании заказа" });
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<OrderResponse>>> GetUserOrders()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized(new { Message = "Пользователь не авторизован" });

                var orders = await _context.Orders
                    .Include(o => o.Car) 
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.StartDate)
                    .ToListAsync();

                var response = orders.Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    CarId = o.CarId,
                    DriverId = o.DriverId ?? 0,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    TotalCost = o.TotalCost,
                    Status = o.Status,
                    Car = o.Car != null ? new CarResponse(
                        o.Car.CarId,
                        o.Car.Brand,
                        o.Car.Model,
                        o.Car.Year,
                        o.Car.Color,
                        o.Car.LicensePlate,
                        o.Car.HourlyRate,
                        o.Car.IsAvailable) : null
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
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { Message = "Заказ не найден" });
                }

                if (order.StartDate <= DateTime.Now.AddHours(24))
                {
                    return BadRequest(new
                    {
                        Message = "Заказ можно отменить не позднее чем за 24 часа до начала"
                    });
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

        private async Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.CarId == carId && o.Status != "Отменен")
                .ToListAsync();

            return !orders.Any(o =>
                (startDate >= o.StartDate && startDate <= o.EndDate) ||
                (endDate >= o.StartDate && endDate <= o.EndDate) ||
                (startDate <= o.StartDate && endDate >= o.EndDate));
        }
    }
}