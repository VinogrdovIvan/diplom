using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel.ViewModels
{
    public partial class AddUserViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _phone;

        [ObservableProperty]
        private string _password;

        public AddUserViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
        }

        [RelayCommand]
        private async Task AddUser()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FirstName) ||
                    string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Phone) ||
                    string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Все поля обязательны для заполнения", "Ошибка");
                    return;
                }

                if (Password.Length < 6)
                {
                    MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка");
                    return;
                }

                var userDto = new
                {
                    FirstName,
                    LastName,
                    Email,
                    Phone,
                    Password,
                    RoleId = 3
                };

                Console.WriteLine($"Отправка данных: {JsonSerializer.Serialize(userDto)}");

                var response = await _apiClient.PostAsync("auth/register", userDto);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Пользователь успешно создан", "Успех");
                    CloseWindow();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера: {errorContent}", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка");
                Console.WriteLine($"Полная ошибка: {ex}");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void ShowLoginWindow()
        {
            App.Token = null;
            new LoginWindow().Show();
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }
    }
}