using CarRentalApp.Services;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRentalApp.Views;

namespace CarRentalApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        public LoginRequest LoginRequest { get; set; } = new LoginRequest();

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
            NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage), true));
        }

        public async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginRequest.Email) || string.IsNullOrWhiteSpace(LoginRequest.Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Пожалуйста, заполните все поля.", "OK");
                    return;
                }

                var tokenResponse = await _authService.LoginAsync(LoginRequest);
                if (tokenResponse != null)
                {
                    await SecureStorage.SetAsync("access_token", tokenResponse.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}