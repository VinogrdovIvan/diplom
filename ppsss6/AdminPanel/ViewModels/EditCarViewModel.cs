using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class EditCarViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private Car _car;

        public EditCarViewModel(Car car)
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
            Car = car;
        }

        [RelayCommand]
        private async Task SaveCar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Car.LicensePlate))
                {
                    MessageBox.Show("Введите номерной знак", "Ошибка");
                    return;
                }

                if (Car.HourlyRate <= 0)
                {
                    MessageBox.Show("Стоимость аренды должна быть больше 0", "Ошибка");
                    return;
                }

                var response = await _apiClient.PutAsync($"cars/{Car.CarId}", Car);

                if (response.IsSuccessStatusCode)
                {
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