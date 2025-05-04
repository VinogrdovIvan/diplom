using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public class CarService : ICarService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CarService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Получение списка доступных автомобилей
        public async Task<List<CarResponse>> GetAvailableCarsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.GetAsync("api/Cars/available");
                response.EnsureSuccessStatusCode();
                return  await response.Content.ReadFromJsonAsync<List<CarResponse>>();
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

        // Получение автомобиля по ID
        public async Task<CarResponse> GetCarByIdAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.GetAsync($"api/Cars/{id}"); 
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CarResponse>();
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

        // Добавление нового автомобиля
        public async Task<CarAddedResponse> AddCarAsync(AddCarRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Cars/add", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CarAddedResponse>();
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

        // Обновление информации об автомобиле
        public async Task<CarUpdatedResponse> UpdateCarAsync(UpdateCarRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PutAsJsonAsync("api/Cars/update", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CarUpdatedResponse>();
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

        // Удаление автомобиля
        public async Task<bool> DeleteCarAsync(int carId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.DeleteAsync($"api/Cars/delete/{carId}");
                return response.IsSuccessStatusCode;
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


        public async Task<CalculateCostResponse> CalculateCostAsync(CalculateCostRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Cars/calculate-cost", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CalculateCostResponse>();
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