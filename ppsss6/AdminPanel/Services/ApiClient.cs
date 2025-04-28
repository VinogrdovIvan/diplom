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
            _baseUrl = baseUrl.TrimEnd('/');
        }

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

        private void EnsureAuthenticated()
        {
            if (string.IsNullOrEmpty(App.Token))
            {
                throw new UnauthorizedAccessException("Требуется авторизация. Пожалуйста, войдите снова.");
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                EnsureAuthenticated();
                var url = $"{_baseUrl}/api/{endpoint.TrimStart('/')}";
                var response = await _httpClient.GetAsync(url);

                await HandleCommonErrors(response);
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Error: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            try
            {
                var url = $"{_baseUrl}/api/{endpoint.TrimStart('/')}";
                Console.WriteLine($"POST to: {url}");

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex}");
                throw;
            }
        }



        public async Task<HttpResponseMessage> PutAsync(string endpoint, object data)
        {
            try
            {
                var url = $"{_baseUrl}/api/{endpoint.TrimStart('/')}";
                Console.WriteLine($"PUT Request to: {url}");
                Console.WriteLine($"Data: {JsonSerializer.Serialize(data)}");

                var response = await _httpClient.PutAsJsonAsync(url, data);

                Console.WriteLine($"Response Status: {response.StatusCode}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Request Error: {ex}");
                throw;
            }
        }

        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                EnsureAuthenticated();
                var url = $"{_baseUrl}/api/{endpoint.TrimStart('/')}";
                var response = await _httpClient.DeleteAsync(url);

                await HandleCommonErrors(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE Error: {ex.Message}");
                throw;
            }
        }

        private async Task HandleCommonErrors(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException(content);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new KeyNotFoundException(content);
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP Error: {response.StatusCode} - {content}");
            }
        }
    }
}