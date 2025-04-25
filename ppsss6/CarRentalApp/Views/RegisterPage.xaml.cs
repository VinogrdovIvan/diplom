using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly IAuthService _authService;

        public RegisterPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            BindingContext = new RegisterViewModel(authService);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as RegisterViewModel;
            if (viewModel != null)
            {
                await viewModel.RegisterAsync();
            }
        }

        private async void OnNavigateToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}