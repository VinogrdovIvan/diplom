using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetUserOrdersAsync();
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<List<OrderResponse>> GetOrdersAsync(int userId);
        Task<bool> CancelOrderAsync(int orderId);
    }


}