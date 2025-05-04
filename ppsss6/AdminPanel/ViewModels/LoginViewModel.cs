using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Windows;
using AdminPanel.Services;
using AdminPanel.Views;
using CarRental.Shared.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminPanel.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty] private string _email;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _errorMessage;

        public LoginViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Email и пароль обязательны для заполнения";
                    return;
                }

                var response = await _apiClient.PostAsync("auth/login", new
                {
                    Email,
                    Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Неверный email или пароль";
                    return;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonContent);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(tokenResponse.AccessToken);

                var roleIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;

                if (roleIdClaim != "3") 
                {
                    ErrorMessage = "Доступ разрешен только администраторам";
                    return;
                }

                App.Token = tokenResponse.AccessToken;
                _apiClient.SetToken(App.Token);

                new MainWindow().Show();
                Application.Current.Windows.OfType<LoginWindow>().First().Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка входа: {ex.Message}";
                Console.WriteLine($"Ошибка: {ex}");
            }
        }
    }
}