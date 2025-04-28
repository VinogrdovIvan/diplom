using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

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
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
        }

        [RelayCommand]
        private async Task AddDriver()
        {
            try
            {
                var driver = new Driver
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    LicenseNumber = LicenseNumber,
                    IsAvailable = IsAvailable
                };

                var response = await _apiClient.PostAsync("drivers", driver);

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
                MessageBox.Show($"Ошибка при добавлении водителя: {ex.Message}",
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