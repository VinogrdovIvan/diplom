using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class OrdersPage : ContentPage
    {
        private readonly IOrderService _orderService;

        public OrdersPage(IOrderService orderService)
        {
            InitializeComponent();
            _orderService = orderService;
            BindingContext = new OrdersViewModel(orderService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (BindingContext is OrdersViewModel vm)
                {
                    await vm.LoadOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить заказы: {ex.Message}", "OK");
            }
        }

        private async void OnReviewButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int orderId)
            {
                await Shell.Current.GoToAsync($"ReviewPage?orderId={orderId}");
            }
        }

        private async Task NavigateToReviewPage(int orderId)
        {
            try
            {
                if (orderId <= 0) return;
                await Shell.Current.GoToAsync($"//ReviewPage?orderId={orderId}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Navigation error: {ex.Message}", "OK");
            }
        }
    }
}