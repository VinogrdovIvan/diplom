using CarRentalApp.Services;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRentalApp.ViewModels
{
    public class CalculateCostViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; } = DateTime.Now.TimeOfDay;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public TimeSpan EndTime { get; set; } = DateTime.Now.TimeOfDay;

        private decimal _totalCost;
        public decimal TotalCost
        {
            get => _totalCost;
            set
            {
                _totalCost = value;
                OnPropertyChanged();
            }
        }

        private int _carId;
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

        private bool _showBookButton;
        public bool ShowBookButton
        {
            get => _showBookButton;
            set
            {
                _showBookButton = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public CarResponse Car { get; set; }

        public CalculateCostViewModel(ICarService carService, IOrderService orderService)
        {
            _carService = carService;
            _orderService = orderService;
            ShowBookButton = false; // Изначально кнопка скрыта
        }

        public async Task CalculateCostAsync()
        {
            try
            {
                IsBusy = true;
                ShowBookButton = false;

                var startDateTime = StartDate.Add(StartTime);
                var endDateTime = EndDate.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Дата окончания должна быть позже даты начала.", "OK");
                    return;
                }

                var request = new CalculateCostRequest
                {
                    CarId = CarId,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                var response = await _carService.CalculateCostAsync(request);
                TotalCost = response.TotalCost;
                ShowBookButton = true; // Показываем кнопку после успешного расчета

                await Application.Current.MainPage.DisplayAlert("Успех", $"Стоимость аренды: {TotalCost} ₽", "OK");
            }
            catch (Exception ex)
            {
                ShowBookButton = false;
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
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

                // Проверяем авторизацию
                var userId = await SecureStorage.GetAsync("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Требуется авторизация", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
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
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    TotalCost = TotalCost
                };

                var response = await _orderService.CreateOrderAsync(orderRequest);
                await Shell.Current.DisplayAlert("Успех", "Заказ создан", "OK");
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
            var carId = Convert.ToInt32(query["carId"]);
            CarId = carId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}