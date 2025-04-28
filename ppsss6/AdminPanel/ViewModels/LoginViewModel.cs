using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CarRental.Shared.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

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
                    Email = Email,
                    Password = Password
                });

                var jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonContent);

                if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
                {
                    ErrorMessage = "Неверный ответ сервера";
                    return;
                }

                App.Token = tokenResponse.AccessToken;
                _apiClient.SetToken(App.Token);

                new MainWindow().Show();
                Application.Current.Windows.OfType<LoginWindow>().First().Close();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Неверный email или пароль";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка входа: {ex.Message}";
            }
        }
    }
}