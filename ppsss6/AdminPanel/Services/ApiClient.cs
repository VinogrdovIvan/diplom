using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _baseUrl = baseUrl.TrimEnd('/'); // Удаляем слэш в конце, если есть
        }

        // Установка JWT-токена
        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // GET-запрос
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                if (string.IsNullOrEmpty(App.Token))
                {
                    throw new Exception("Требуется авторизация. Пожалуйста, войдите снова.");
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", App.Token); 

                var url = $"{_baseUrl}{endpoint}";
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Сессия истекла. Пожалуйста, войдите снова.");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Error: {ex.Message}");
                throw;
            }
        }

        // POST-запрос
        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                Console.WriteLine($"POST: {url}");

                var response = await _httpClient.PostAsJsonAsync(url, data);
                LogResponse(response);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex.Message}");
                throw;
            }
        }

        // PUT-запрос
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                Console.WriteLine($"PUT: {url}");

                var response = await _httpClient.PutAsJsonAsync(url, data);
                LogResponse(response);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Error: {ex.Message}");
                throw;
            }
        }

        // DELETE-запрос
        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                Console.WriteLine($"DELETE: {url}");

                var response = await _httpClient.DeleteAsync(url);
                LogResponse(response);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE Error: {ex.Message}");
                throw;
            }
        }

        // Логирование ответа
        private void LogResponse(HttpResponseMessage response)
        {
            Console.WriteLine($"Response: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
    }
}