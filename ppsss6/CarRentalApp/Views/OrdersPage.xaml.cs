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
            if (BindingContext is OrdersViewModel vm)
            {
                await vm.LoadOrdersAsync();
            }
        }
    }
}