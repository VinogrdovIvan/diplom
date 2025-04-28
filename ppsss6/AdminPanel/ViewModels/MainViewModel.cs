using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private ObservableCollection<object> _currentItems;

        [ObservableProperty]
        private string _currentView;

        public MainViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
            CurrentItems = new ObservableCollection<object>();
        }

        [RelayCommand]
        private async Task LoadCars()
        {
            try
            {
                var cars = await _apiClient.GetAsync<List<Car>>("cars");
                CurrentItems = new ObservableCollection<object>(cars.Cast<object>());
                CurrentView = "Cars";
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
                CurrentView = "Drivers";
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
                CurrentView = "Users";
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
        private void AddItem()
        {
            try
            {
                switch (CurrentView)
                {
                    case "Cars":
                        new AddCarWindow().ShowDialog();
                        LoadCars();
                        break;
                    case "Drivers":
                        new AddDriverWindow().ShowDialog();
                        LoadDrivers();
                        break;
                    case "Users":
                        new AddUserWindow().ShowDialog();
                        LoadUsers();
                        break;
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
                switch (CurrentView)
                {
                    case "Cars":
                        new EditCarWindow(item as Car).ShowDialog();
                        LoadCars();
                        break;
                    case "Drivers":
                        new EditDriverWindow(item as Driver).ShowDialog();
                        LoadDrivers();
                        break;
                    case "Users":
                        new EditUserWindow(item as User).ShowDialog();
                        LoadUsers();
                        break;
                }
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
                var message = $"Вы уверены, что хотите удалить этот элемент?";
                var result = MessageBox.Show(message, "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    switch (CurrentView)
                    {
                        case "Cars":
                            await _apiClient.DeleteAsync($"cars/{(item as Car).CarId}");
                            await LoadCars();
                            break;
                        case "Drivers":
                            await _apiClient.DeleteAsync($"drivers/{(item as Driver).DriverId}");
                            await LoadDrivers();
                            break;
                        case "Users":
                            await _apiClient.DeleteAsync($"users/{(item as User).UserId}");
                            await LoadUsers();
                            break;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowLoginWindow();
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show("Элемент не найден. Возможно, он был уже удален.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                await RefreshCurrentView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshCurrentView()
        {
            switch (CurrentView)
            {
                case "Cars": await LoadCars(); break;
                case "Drivers": await LoadDrivers(); break;
                case "Users": await LoadUsers(); break;
            }
        }

        private void ShowLoginWindow()
        {
            App.Token = null;
            new LoginWindow().Show();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }
    }
}