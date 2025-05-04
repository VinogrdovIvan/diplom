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
    public partial class AddOrderViewModel : OrderViewModel
    {
        [ObservableProperty]
        private List<User> _users;

        [ObservableProperty]
        private List<Car> _cars;

        [ObservableProperty]
        private List<Driver> _drivers;

        [ObservableProperty]
        private List<string> _statuses = new() { "Подтвержден", "Активен", "Отменен" };

        public AddOrderViewModel() : base()
        {
            Order = new Order
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Status = "Подтвержден"
            };

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                Users = await _apiClient.GetAsync<List<User>>("users");
                Cars = await _apiClient.GetAsync<List<Car>>("cars");
                Drivers = await _apiClient.GetAsync<List<Driver>>("drivers");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddOrder()
        {
            try
            {
                if (Order.UserId == 0 || Order.CarId == 0 || Order.DriverId == 0)
                {
                    MessageBox.Show("Выберите пользователя, автомобиль и водителя");
                    return;
                }

                var response = await _apiClient.PostAsync("orders", Order);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Заказ успешно создан");
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
    }
}