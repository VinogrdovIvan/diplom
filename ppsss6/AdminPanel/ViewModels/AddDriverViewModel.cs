using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using static Xamarin.Essentials.Permissions;

namespace AdminPanel.ViewModels
{
    public partial class AddDriverViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _phone;

        [ObservableProperty]
        private string _licenseNumber;

        [ObservableProperty]
        private bool _isAvailable;

        public AddDriverViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299/api");
        }

        [RelayCommand]
        private async Task AddDriver()
        {
            var driver = new Driver
            {
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                LicenseNumber = LicenseNumber,
                IsAvailable = IsAvailable
            };

            await _apiClient.PostAsync("drivers", driver);

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