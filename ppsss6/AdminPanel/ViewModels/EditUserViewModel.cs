using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class EditUserViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private User _user;

        public EditUserViewModel(User user)
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
            User = user;
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            try
            {

                var userDto = new
                {
                    UserId = User.UserId,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Email = User.Email,
                    Phone = User.Phone,
                    RoleId = User.RoleId,
                    Password = User.Password
                };

                var response = await _apiClient.PutAsync($"users/{User.UserId}", userDto);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные пользователя успешно обновлены", "Успех");
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
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
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