using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CarRentalApp.Views
{
    public partial class EditProfilePage : ContentPage
    {
        private readonly IAuthService _authService;

        public EditProfilePage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            BindingContext = new EditProfileViewModel(authService);
        }

        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as EditProfileViewModel;
            if (viewModel != null)
            {
                await viewModel.SaveProfileAsync();
            }
        }
    }
}