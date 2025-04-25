using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService _authService;

        public LoginPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            BindingContext = new LoginViewModel(authService);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as LoginViewModel;
            if (viewModel != null)
            {
                await viewModel.LoginAsync();
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage), true);
        }
    }
}