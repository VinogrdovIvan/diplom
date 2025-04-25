using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class EditDriverViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private Driver _driver;

        public EditDriverViewModel(Driver driver)
        {
            _apiClient = new ApiClient("http://localhost:5299/api");
            Driver = driver;
        }

        [RelayCommand]
        private async Task SaveDriver()
        {
            await _apiClient.PutAsync<Driver>($"drivers/{Driver.DriverId}", Driver);

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