using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class CalculateCostPage : ContentPage
    {
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;
        private readonly IHttpClientFactory _httpClientFactory;

        public CalculateCostPage(ICarService carService, IOrderService orderService, IHttpClientFactory httpClientFactory)
        {
            InitializeComponent();
            _carService = carService;
            _orderService = orderService;
            _httpClientFactory = httpClientFactory;
            BindingContext = new CalculateCostViewModel(carService, orderService, httpClientFactory);
        }

        private async void OnCalculateCostClicked(object sender, EventArgs e)
        {
            try
            {
                await (BindingContext as CalculateCostViewModel)?.CalculateCostAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void OnBookClicked(object sender, EventArgs e)
        {
            try
            {
                await (BindingContext as CalculateCostViewModel)?.CreateOrderAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }
    }
}