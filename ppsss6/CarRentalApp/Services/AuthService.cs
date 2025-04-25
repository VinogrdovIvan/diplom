using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Логин пользователя
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Auth/login", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка при входе: {response.StatusCode}, {errorContent}");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                await SecureStorage.SetAsync("access_token", tokenResponse.AccessToken);
                await SecureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);

                return tokenResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка входа: " + ex.Message);
            }
        }



        // Регистрация пользователя
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Auth/register", request);
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

        // Получение профиля пользователя
        public async Task<UserResponse> GetUserProfileAsync()
        {
            var client = _httpClientFactory.CreateClient("Backend");
            var response = await client.GetAsync("api/users/me");
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }

        // Обновление профиля пользователя
        public async Task<bool> UpdateUserProfileAsync(UpdateCurrentUserRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PutAsJsonAsync($"api/Users/me/update", request);
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

        // Обновление токенов
        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var response = await client.PostAsJsonAsync("api/Auth/refresh-token", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TokenResponse>();
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

        // Проверка текущего пароля
        public async Task<bool> VerifyPasswordAsync(string currentPassword)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Backend");
                var request = new VerifyPasswordRequest { CurrentPassword = currentPassword };
                var response = await client.PostAsJsonAsync("api/auth/verify-password", request);
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

     
    }
}