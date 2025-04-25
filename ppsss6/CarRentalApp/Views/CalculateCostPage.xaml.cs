using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class CalculateCostPage : ContentPage
    {
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;

        public CalculateCostPage(ICarService carService, IOrderService orderService)
        {
            InitializeComponent();
            _carService = carService;
            _orderService = orderService;
            BindingContext = new CalculateCostViewModel(carService, orderService);
        }

        private async void OnCalculateCostClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as CalculateCostViewModel;
            if (viewModel != null)
            {
                await viewModel.CalculateCostAsync();
            }
        }

        private async void OnBookClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as CalculateCostViewModel;
            if (viewModel != null)
            {
                await viewModel.CreateOrderAsync();
            }
        }
    }
}