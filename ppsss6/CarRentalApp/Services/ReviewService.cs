using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarRentalApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReviewService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ReviewResponse>> GetReviewsByOrderAsync(int orderId)
        {
            try
            {
                var client = await GetAuthenticatedClient();
                var response = await client.GetAsync($"api/Reviews/by-order/{orderId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new List<ReviewResponse>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reviews: {ex}");
                throw;
            }
        }

        public async Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request)
        {
            try
            {
                var client = await GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("api/Reviews", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка сервера: {response.StatusCode}. {errorContent}");
                }

                return await response.Content.ReadFromJsonAsync<ReviewResponse>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Ошибка сети: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при создании отзыва: " + ex.Message);
            }
        }

        private async Task<HttpClient> GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient("Backend");
            var token = await SecureStorage.GetAsync("access_token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }


        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var client = await GetAuthenticatedClient();
            var response = await client.DeleteAsync($"api/Reviews/{reviewId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка {response.StatusCode}: {error}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}