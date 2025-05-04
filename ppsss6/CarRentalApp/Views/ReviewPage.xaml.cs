using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    [QueryProperty(nameof(OrderId), "orderId")]
    public partial class ReviewPage : ContentPage
    {
        private readonly IReviewService _reviewService;

        public int OrderId { get; set; }

        public ReviewPage(IReviewService reviewService)
        {
            InitializeComponent();
            _reviewService = reviewService;
            BindingContext = new ReviewViewModel(reviewService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (OrderId <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Неверный заказ", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            if (BindingContext is ReviewViewModel vm)
            {
                await vm.Initialize(OrderId);
            }
        }
    }
}