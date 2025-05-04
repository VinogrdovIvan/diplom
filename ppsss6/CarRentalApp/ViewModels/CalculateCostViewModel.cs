using CarRentalApp.Services;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace CarRentalApp.ViewModels
{
    public class CalculateCostViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;
        private readonly IHttpClientFactory _httpClientFactory;

        private List<DriverResponse> _availableDrivers;
        private DriverResponse _selectedDriver;
        private decimal _totalCost;
        private bool _showBookButton;
        private bool _isBusy;
        private int _carId;
        private CarResponse _car;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; } = DateTime.Now.TimeOfDay;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public TimeSpan EndTime { get; set; } = DateTime.Now.TimeOfDay;

        public List<DriverResponse> AvailableDrivers
        {
            get => _availableDrivers;
            set
            {
                _availableDrivers = value;
                OnPropertyChanged();
            }
        }

        public DriverResponse SelectedDriver
        {
            get => _selectedDriver;
            set
            {
                _selectedDriver = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalCost
        {
            get => _totalCost;
            set
            {
                _totalCost = value;
                OnPropertyChanged();
            }
        }

        public bool ShowBookButton
        {
            get => _showBookButton;
            set
            {
                _showBookButton = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public int CarId
        {
            get => _carId;
            set
            {
                if (_carId == value) return;
                _carId = value;
                OnPropertyChanged();
            }
        }

        public CarResponse Car
        {
            get => _car;
            set
            {
                _car = value;
                OnPropertyChanged();
            }
        }

        public CalculateCostViewModel(ICarService carService, IOrderService orderService, IHttpClientFactory httpClientFactory)
        {
            _carService = carService;
            _orderService = orderService;
            _httpClientFactory = httpClientFactory;
            ShowBookButton = false;
            LoadAvailableDrivers();
            LoadCarDetails();
        }

        private async Task LoadAvailableDrivers()
        {
            try
            {
                IsBusy = true;
                var client = _httpClientFactory.CreateClient("Backend");

                // Добавляем токен авторизации
                var token = await SecureStorage.GetAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.GetAsync("api/Drivers/available");
                response.EnsureSuccessStatusCode();

                AvailableDrivers = await response.Content.ReadFromJsonAsync<List<DriverResponse>>();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить список водителей: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadCarDetails()
        {
            try
            {
                if (CarId <= 0) return;

                IsBusy = true;
                Car = await _carService.GetCarByIdAsync(CarId);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить данные автомобиля: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task CalculateCostAsync()
        {
            try
            {
                IsBusy = true;
                ShowBookButton = false;

                if (SelectedDriver == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, выберите водителя", "OK");
                    return;
                }

                var startDateTime = StartDate.Add(StartTime);
                var endDateTime = EndDate.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Дата окончания должна быть позже даты начала", "OK");
                    return;
                }

                var request = new CalculateCostRequest
                {
                    CarId = CarId,
                    DriverId = SelectedDriver.DriverId,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                var response = await _carService.CalculateCostAsync(request);
                TotalCost = response.TotalCost;
                ShowBookButton = true;

                // Обновляем список водителей после расчета
                await LoadAvailableDrivers();

                // Проверяем, что выбранный водитель все еще доступен
                if (AvailableDrivers.All(d => d.DriverId != SelectedDriver.DriverId))
                {
                    await Shell.Current.DisplayAlert("Внимание", "Выбранный водитель больше не доступен", "OK");
                    SelectedDriver = null;
                    ShowBookButton = false;
                }

                await Shell.Current.DisplayAlert(
                    "Расчет стоимости",
                    $"Стоимость аренды: {TotalCost} ₽\n" +
                    $"Водитель: {SelectedDriver?.FullName ?? "не выбран"}",
                    "OK");
            }
            catch (Exception ex)
            {
                ShowBookButton = false;
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task CreateOrderAsync()
        {
            try
            {
                IsBusy = true;

                var userId = await SecureStorage.GetAsync("user_id");
                var accessToken = await SecureStorage.GetAsync("access_token");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Требуется авторизация", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                if (SelectedDriver == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, выберите водителя", "OK");
                    return;
                }

                var startDateTime = StartDate.Add(StartTime);
                var endDateTime = EndDate.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Дата окончания должна быть позже даты начала", "OK");
                    return;
                }

                var orderRequest = new CreateOrderRequest
                {
                    UserId = int.Parse(userId),
                    CarId = CarId,
                    DriverId = SelectedDriver.DriverId,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    TotalCost = TotalCost
                };

                var response = await _orderService.CreateOrderAsync(orderRequest);

                await Shell.Current.DisplayAlert(
                    "Успех",
                    $"Заказ успешно создан!\n" +
                    $"Автомобиль: {Car.Brand} {Car.Model}\n" +
                    $"Водитель: {SelectedDriver.FullName}\n" +
                    $"Стоимость: {TotalCost} ₽",
                    "OK");

                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("carId", out var carIdObj) && carIdObj is int carId)
            {
                CarId = carId;
                LoadCarDetails();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}