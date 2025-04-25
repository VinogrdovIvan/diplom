using CarRentalApp.Services;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CarRentalApp.Views;

namespace CarRentalApp.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly ICarService _carService;
        private List<CarResponse> _allCars;
        private string _searchQuery;
        private string _selectedPriceFilter;
        private bool _isBusy;

        public ICommand NavigateToOrdersCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToCalculateCostCommand { get; }
        public ICommand LoadCarsCommand { get; }

        public ObservableCollection<CarResponse> Cars { get; } = new ObservableCollection<CarResponse>();

        public List<string> PriceFilters { get; } = new List<string>
        {
            "Все",
            "До 1000 ₽/час",
            "1000-2000 ₽/час",
            "2000-3000 ₽/час",
            "Более 3000 ₽/час"
        };

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterCars(value);
            }
        }

        public string SelectedPriceFilter
        {
            get => _selectedPriceFilter;
            set
            {
                _selectedPriceFilter = value;
                OnPropertyChanged();
                FilterCars(SearchQuery);
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

        public HomeViewModel(ICarService carService)
        {
            _carService = carService;

            NavigateToOrdersCommand = new Command(async () =>
                await Shell.Current.GoToAsync(nameof(OrdersPage)));

            NavigateToProfileCommand = new Command(async () =>
                await Shell.Current.GoToAsync(nameof(ProfilePage)));

            NavigateToCalculateCostCommand = new Command<int>(async (carId) =>
            {
                var navigationParameters = new Dictionary<string, object>
                {
                    { "carId", carId },
                };
                await Shell.Current.GoToAsync(nameof(CalculateCostPage), navigationParameters);
            });

            LoadCarsCommand = new Command(async () => await LoadCarsAsync());

            // Загружаем данные при создании
            Task.Run(async () => await LoadCarsAsync());
        }

        public async Task LoadCarsAsync()
        {
            try
            {
                IsBusy = true;
                var cars = await _carService.GetAvailableCarsAsync();
                _allCars = cars.ToList();
                FilterCars(SearchQuery);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void FilterCars(string searchQuery)
        {
            var filteredCars = _allCars;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                filteredCars = filteredCars
                    .Where(c => c.Brand.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                c.Model.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(SelectedPriceFilter))
            {
                switch (SelectedPriceFilter)
                {
                    case "До 1000 ₽/час":
                        filteredCars = filteredCars.Where(c => c.HourlyRate <= 1000).ToList();
                        break;
                    case "1000-2000 ₽/час":
                        filteredCars = filteredCars.Where(c => c.HourlyRate > 1000 && c.HourlyRate <= 2000).ToList();
                        break;
                    case "2000-3000 ₽/час":
                        filteredCars = filteredCars.Where(c => c.HourlyRate > 2000 && c.HourlyRate <= 3000).ToList();
                        break;
                    case "Более 3000 ₽/час":
                        filteredCars = filteredCars.Where(c => c.HourlyRate > 3000).ToList();
                        break;
                }
            }

            Cars.Clear();
            foreach (var car in filteredCars)
            {
                Cars.Add(car);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}