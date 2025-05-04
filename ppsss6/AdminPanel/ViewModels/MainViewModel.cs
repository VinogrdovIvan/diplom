using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<object> _currentItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _filteredItems = new();

        [ObservableProperty]
        private string _currentView = "Добро пожаловать";

        [ObservableProperty]
        private int _itemsCount;

        [ObservableProperty]
        private bool _isCarsView;

        [ObservableProperty]
        private bool _isDriversView;

        [ObservableProperty]
        private bool _isUsersView;

        [ObservableProperty]
        private bool _isOrdersView;

        [ObservableProperty]
        private bool _isReviewsView;

        [ObservableProperty]
        private bool _isFilterVisible;

        [ObservableProperty]
        private string _filterText;

        [ObservableProperty]
        private string _selectedFilterProperty;

        [ObservableProperty]
        private List<FilterOption> _filterOptions;

        public MainViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);

            FilterOptions = new List<FilterOption>
            {
                new("Все поля", ""),
                new("ID", "Id"),
                new("Название", "Name"),
                new("Марка (авто)", "Brand"),
                new("Модель (авто)", "Model"),
                new("Телефон", "Phone"),
                new("Email", "Email"),
                new("Оценка", "Rating"),
                new("Комментарий", "Comment"),
                new("ID заказа", "OrderId")
            };
            SelectedFilterProperty = "";

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentItems) ||
                e.PropertyName == nameof(FilterText) ||
                e.PropertyName == nameof(SelectedFilterProperty))
            {
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (CurrentItems == null || !CurrentItems.Any())
            {
                FilteredItems.Clear();
                return;
            }

            var filtered = string.IsNullOrWhiteSpace(FilterText)
                ? CurrentItems
                : CurrentItems.Where(item =>
                    string.IsNullOrEmpty(SelectedFilterProperty)
                        ? item.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)
                        : item.GetType().GetProperty(SelectedFilterProperty)?.GetValue(item)?.ToString()
                            ?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true);

            FilteredItems.Clear();
            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }

            ItemsCount = FilteredItems.Count;
        }

        [RelayCommand]
        private void ToggleFilter()
        {
            IsFilterVisible = !IsFilterVisible;
        }

        [RelayCommand]
        private void ResetFilter()
        {
            FilterText = string.Empty;
            SelectedFilterProperty = "";
        }

        [RelayCommand]
        private async Task LoadCars()
        {
            try
            {
                var cars = await _apiClient.GetAsync<List<Car>>("cars");
                CurrentItems = new ObservableCollection<object>(cars.Cast<object>());
                CurrentView = "Автомобили";
                ItemsCount = cars.Count;
                IsCarsView = true;
                IsDriversView = false;
                IsUsersView = false;
                IsOrdersView = false;
                IsReviewsView = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке автомобилей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadDrivers()
        {
            try
            {
                var drivers = await _apiClient.GetAsync<List<Driver>>("drivers");
                CurrentItems = new ObservableCollection<object>(drivers.Cast<object>());
                CurrentView = "Водители";
                ItemsCount = drivers.Count;
                IsCarsView = false;
                IsDriversView = true;
                IsUsersView = false;
                IsOrdersView = false;
                IsReviewsView = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке водителей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadUsers()
        {
            try
            {
                var users = await _apiClient.GetAsync<List<User>>("users");
                CurrentItems = new ObservableCollection<object>(users.Cast<object>());
                CurrentView = "Пользователи";
                ItemsCount = users.Count;
                IsCarsView = false;
                IsDriversView = false;
                IsUsersView = true;
                IsOrdersView = false;
                IsReviewsView = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadOrders()
        {
            try
            {
                var orders = await _apiClient.GetAsync<List<Order>>("orders");
                var cars = await _apiClient.GetAsync<List<Car>>("cars");
                var drivers = await _apiClient.GetAsync<List<Driver>>("drivers");
                var users = await _apiClient.GetAsync<List<User>>("users");

                foreach (var order in orders)
                {
                    order.CarInfo = cars.FirstOrDefault(c => c.CarId == order.CarId)?.Name ?? "Неизвестно";
                    order.DriverInfo = drivers.FirstOrDefault(d => d.DriverId == order.DriverId)?.Name ?? "Неизвестно";
                    order.UserInfo = users.FirstOrDefault(u => u.UserId == order.UserId)?.Name ?? "Неизвестно";
                }

                CurrentItems = new ObservableCollection<object>(orders);
                CurrentView = "Заказы";
                ItemsCount = orders.Count;
                IsCarsView = false;
                IsDriversView = false;
                IsUsersView = false;
                IsOrdersView = true;
                IsReviewsView = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadReviews()
        {
            try
            {
                var reviews = await _apiClient.GetAsync<List<Review>>("reviews");
                var orders = await _apiClient.GetAsync<List<Order>>("orders");

                foreach (var review in reviews)
                {
                    review.OrderInfo = orders.FirstOrDefault(o => o.OrderId == review.OrderId)?.Name ?? "Неизвестный заказ";
                }

                CurrentItems = new ObservableCollection<object>(reviews);
                CurrentView = "Отзывы";
                ItemsCount = reviews.Count;
                IsCarsView = false;
                IsDriversView = false;
                IsUsersView = false;
                IsOrdersView = false;
                IsReviewsView = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddItem()
        {
            try
            {
                if (IsCarsView)
                {
                    new AddCarWindow().ShowDialog();
                    LoadCars();
                }
                else if (IsDriversView)
                {
                    new AddDriverWindow().ShowDialog();
                    LoadDrivers();
                }
                else if (IsUsersView)
                {
                    new AddUserWindow().ShowDialog();
                    LoadUsers();
                }
                else if (IsOrdersView)
                {
                    new AddOrderWindow().ShowDialog();
                    LoadOrders();
                }
                else if (IsReviewsView)
                {
                    new AddReviewWindow().ShowDialog();
                    LoadReviews();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void EditItem(object item)
        {
            try
            {
                if (IsCarsView && item is Car car)
                {
                    new EditCarWindow(car).ShowDialog();
                    LoadCars();
                }
                else if (IsDriversView && item is Driver driver)
                {
                    new EditDriverWindow(driver).ShowDialog();
                    LoadDrivers();
                }
                else if (IsUsersView && item is User user)
                {
                    new EditUserWindow(user).ShowDialog();
                    LoadUsers();
                }
                else if (IsOrdersView && item is Order order)
                {
                    new EditOrderWindow(order).ShowDialog();
                    LoadOrders();
                }
                // Для отзывов редактирование не предусмотрено
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteItem(object item)
        {
            try
            {
                if (IsReviewsView && item is Review review)
                {
                    var result = MessageBox.Show("Удалить этот отзыв?", "Подтверждение удаления",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        await _apiClient.DeleteAsync($"reviews/{review.ReviewId}");
                        await LoadReviews();
                    }
                    return;
                }

                if (IsOrdersView)
                {
                    MessageBox.Show("Удаление заказов запрещено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var message = $"Вы уверены, что хотите удалить этот элемент?";
                var deleteResult = MessageBox.Show(message, "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (deleteResult != MessageBoxResult.Yes) return;

                if (IsCarsView && item is Car car)
                {
                    await _apiClient.DeleteAsync($"cars/{car.CarId}");
                    await LoadCars();
                }
                else if (IsDriversView && item is Driver driver)
                {
                    await _apiClient.DeleteAsync($"drivers/{driver.DriverId}");
                    await LoadDrivers();
                }
                else if (IsUsersView && item is User user)
                {
                    await _apiClient.DeleteAsync($"users/{user.UserId}");
                    await LoadUsers();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowLoginWindow()
        {
            App.Token = null;
            new LoginWindow().Show();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }
    }

    public class FilterOption
    {
        public string DisplayName { get; }
        public string PropertyName { get; }

        public FilterOption(string displayName, string propertyName)
        {
            DisplayName = displayName;
            PropertyName = propertyName;
        }
    }
}