using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class EditOrderViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private Order _order;

        [ObservableProperty]
        private List<Car> _availableCars;

        [ObservableProperty]
        private List<Driver> _availableDrivers;

        [ObservableProperty]
        private List<string> _statuses = new() { "Подтвержден", "Активен", "Завершен", "Отменен" };

        public EditOrderViewModel(Order order)
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
            Order = order;
            LoadAdditionalData();
        }

        private async void LoadAdditionalData()
        {
            try
            {
                AvailableCars = await _apiClient.GetAsync<List<Car>>("cars");
                AvailableDrivers = await _apiClient.GetAsync<List<Driver>>("drivers");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SaveOrder()
        {
            try
            {
                var response = await _apiClient.PutAsync($"orders/{Order.OrderId}", new
                {
                    Order.OrderId,
                    Order.UserId,
                    Order.CarId,
                    Order.DriverId,
                    Order.StartDate,
                    Order.EndDate,
                    Order.TotalCost,
                    Order.Status
                });

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Заказ успешно обновлен");
                    CloseWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow();
        }

        [RelayCommand]
        private async Task CompleteOrder()
        {
            Order.Status = "Завершен";
            await SaveOrder();
        }

        [RelayCommand]
        private async Task CancelOrder()
        {
            Order.Status = "Отменен";
            await SaveOrder();
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
    }
}