using CarRentalApp.Services;
using CarRental.Shared.Requests;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRentalApp.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        public RegisterRequest RegisterRequest { get; set; } = new RegisterRequest();

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await RegisterAsync());
            NavigateToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        public async Task RegisterAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RegisterRequest.FirstName) ||
                    string.IsNullOrWhiteSpace(RegisterRequest.LastName) ||
                    string.IsNullOrWhiteSpace(RegisterRequest.Email) ||
                    string.IsNullOrWhiteSpace(RegisterRequest.Phone) ||
                    string.IsNullOrWhiteSpace(RegisterRequest.Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Пожалуйста, заполните все поля.", "OK");
                    return;
                }

                var isRegistered = await _authService.RegisterAsync(RegisterRequest);
                if (isRegistered)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех", "Регистрация прошла успешно.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
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