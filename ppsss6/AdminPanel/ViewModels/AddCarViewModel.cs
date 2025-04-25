using AdminPanel.Models;
using AdminPanel.Services;
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
            _apiClient = new ApiClient("http://localhost:5299/api");
        }

        [RelayCommand]
        private async Task AddCar()
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

            await _apiClient.PostAsync("cars", car);

            // Закрываем окно после успешного добавления
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // Закрываем окно
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}