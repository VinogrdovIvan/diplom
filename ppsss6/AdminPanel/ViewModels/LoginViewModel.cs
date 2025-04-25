using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CarRental.Shared.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;

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
            _apiClient = new ApiClient("http://localhost:5299/");
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                var response = await _apiClient.PostAsync("api/auth/login", new
                {
                    Email = Email,
                    Password = Password
                });
                var jsonContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Ошибка авторизации: " + jsonContent;
                    return;
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonContent);
                App.Token = tokenResponse.AccessToken;

                // Устанавливаем токен для ApiClient
                _apiClient.SetToken(App.Token);

                // Открываем основное окно
                new MainWindow().Show();
                Application.Current.Windows.OfType<LoginWindow>().First().Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка: " + ex.Message;
            }
        }
    }
}