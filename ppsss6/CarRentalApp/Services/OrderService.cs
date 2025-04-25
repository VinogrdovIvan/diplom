using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Создание нового заказа
        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Orders", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка сервера: {response.StatusCode}, {errorContent}");
                }

                return await response.Content.ReadFromJsonAsync<OrderResponse>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Неизвестная ошибка: {ex.Message}");
            }
        }

        // Получение списка заказов пользователя
        public async Task<List<OrderResponse>> GetOrdersAsync(int userId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.GetAsync($"api/Orders/user/{userId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Ошибка сети: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Неизвестная ошибка: " + ex.Message);
            }
        }

        // Отмена заказа
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.DeleteAsync($"api/Orders/{orderId}");
                return response.IsSuccessStatusCode;
            }

            catch (HttpRequestException ex)
            {
                throw new Exception("Ошибка сети: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при отмене заказа: {ex.Message}");
            }
        }




        public async Task<List<OrderResponse>> GetUserOrdersAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.GetAsync("api/Orders/user");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Ошибка сети: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Неизвестная ошибка: " + ex.Message);
            }
        }
    }
}