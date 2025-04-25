using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly IAuthService _authService;

        public ProfilePage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            BindingContext = new ProfileViewModel(authService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as ProfileViewModel)?.LoadUserProfile();
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EditProfilePage), true);
        }
    }
}