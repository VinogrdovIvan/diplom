using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly ICarService _carService;

        public HomePage(ICarService carService)
        {
            InitializeComponent();
            _carService = carService;
            BindingContext = new HomeViewModel(carService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is HomeViewModel viewModel)
            {
                viewModel.LoadCarsCommand.Execute(null);
            }
        }
    }
}