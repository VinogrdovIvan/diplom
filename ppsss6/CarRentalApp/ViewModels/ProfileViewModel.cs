using CarRentalApp.Services;
using CarRental.Shared.Responses;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CarRentalApp.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;

        private UserResponse _user;
        public UserResponse User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public ProfileViewModel(IAuthService authService)
        {
            _authService = authService;
            LoadUserProfile();
        }

        public async Task LoadUserProfile()
        {
            try
            {
                User = await _authService.GetUserProfileAsync();
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