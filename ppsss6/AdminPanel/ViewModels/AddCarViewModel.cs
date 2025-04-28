using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class AddCarViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _brand;

        [ObservableProperty]
        private string _model;

        [ObservableProperty]
        private int _year;

        [ObservableProperty]
        private string _color;

        [ObservableProperty]
        private string _licensePlate;

        [ObservableProperty]
        private decimal _hourlyRate;

        [ObservableProperty]
        private bool _isAvailable;

        public AddCarViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
        }

        [RelayCommand]
        private async Task AddCar()
        {
            try
            {
                var car = new Car
                {
                    Brand = Brand,
                    Model = Model,
                    Year = Year,
                    Color = Color,
                    LicensePlate = LicensePlate,
                    HourlyRate = HourlyRate,
                    IsAvailable = IsAvailable
                };

                var response = await _apiClient.PostAsync("cars", car);

                if (response.IsSuccessStatusCode)
                {
                    CloseWindow();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowLoginWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении автомобиля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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