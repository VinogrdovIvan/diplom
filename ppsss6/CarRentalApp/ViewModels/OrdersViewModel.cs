using CarRental.Shared.Responses;
using CarRentalApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarRentalApp.ViewModels
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;

        private ObservableCollection<OrderResponse> _orders = new ObservableCollection<OrderResponse>();
        public ObservableCollection<OrderResponse> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
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

        public ICommand LoadOrdersCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CancelOrderCommand { get; }

        public OrdersViewModel(IOrderService orderService)
        {
            _orderService = orderService;

            LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
            RefreshCommand = new Command(async () => await RefreshOrdersAsync());
            CancelOrderCommand = new Command<int>(async (orderId) => await CancelOrderAsync(orderId));
        }

        public async Task LoadOrdersAsync()
        {
            try
            {
                IsBusy = true;
                var orders = await _orderService.GetUserOrdersAsync();
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
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

        private async Task RefreshOrdersAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500); // Для визуального эффекта обновления
                await LoadOrdersAsync();
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

        public async Task CancelOrderAsync(int orderId)
        {
            try
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Подтверждение",
                    "Вы уверены, что хотите отменить этот заказ?",
                    "Да", "Нет");

                if (!confirm) return;

                IsBusy = true;
                bool success = await _orderService.CancelOrderAsync(orderId);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Успех", "Заказ отменен", "OK");
                    await LoadOrdersAsync();
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}