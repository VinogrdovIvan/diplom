using AdminPanel.Models;
using AdminPanel.Services;
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
            _apiClient = new ApiClient("http://localhost:5299/api");
            Car = car;
        }

        [RelayCommand]
        private async Task SaveCar()
        {
            await _apiClient.PutAsync<Car>($"cars/{Car.CarId}", Car);

            // Закрываем окно после успешного сохранения
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