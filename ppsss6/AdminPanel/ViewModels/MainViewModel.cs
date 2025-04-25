    using AdminPanel.Models;
    using AdminPanel.Services;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using AdminPanel.Views;

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
                var cars = await _apiClient.GetAsync<List<Car>>("/api/Cars");
                CurrentItems = new ObservableCollection<object>(cars.Cast<object>());
                CurrentView = "Cars";
            }

            [RelayCommand]
            private async Task LoadDrivers()
            {
                var drivers = await _apiClient.GetAsync<List<Driver>>("/api/Drivers");
                CurrentItems = new ObservableCollection<object>(drivers.Cast<object>());
                CurrentView = "Drivers";
            }

            [RelayCommand]
            private async Task LoadUsers()
            {
                var users = await _apiClient.GetAsync<List<User>>("/api/Users");
                CurrentItems = new ObservableCollection<object>(users.Cast<object>());
                CurrentView = "Users";
            }

            [RelayCommand]
            private void AddItem()
            {
                switch (CurrentView)
                {
                    case "Cars":
                        var addCarWindow = new AddCarWindow();
                        addCarWindow.ShowDialog();
                        LoadCars();
                        break;
                    case "Drivers":
                        var addDriverWindow = new AddDriverWindow();
                        addDriverWindow.ShowDialog();
                        LoadDrivers();
                        break;
                    case "Users":
                        var addUserWindow = new AddUserWindow();
                        addUserWindow.ShowDialog();
                        LoadUsers();
                        break;
                }
            }

            [RelayCommand]
            private void EditItem(object item)
            {
                switch (CurrentView)
                {
                    case "Cars":
                        var editCarWindow = new EditCarWindow(item as AdminPanel.Models.Car);
                        editCarWindow.ShowDialog();
                        LoadCars();
                        break;
                    case "Drivers":
                        var editDriverWindow = new EditDriverWindow(item as AdminPanel.Models.Driver);
                        editDriverWindow.ShowDialog();
                        LoadDrivers();
                        break;
                    case "Users":
                        var editUserWindow = new EditUserWindow(item as AdminPanel.Models.User);
                        editUserWindow.ShowDialog();
                        LoadUsers();
                        break;
                }
            }

            [RelayCommand]
            private async Task DeleteItem(object item)
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
    }