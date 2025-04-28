using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
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
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
            Driver = driver;
        }

        [RelayCommand]
        private async Task SaveDriver()
        {
            try
            {
                var response = await _apiClient.PutAsync($"drivers/{Driver.DriverId}", Driver);
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
                MessageBox.Show($"Ошибка при сохранении водителя: {ex.Message}",
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