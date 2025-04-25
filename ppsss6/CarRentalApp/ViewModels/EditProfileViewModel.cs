using CarRentalApp.Services;
using CarRental.Shared.Responses;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CarRentalApp.ViewModels
{
    public class EditProfileViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private UpdateCurrentUserRequest _request;
        public UpdateCurrentUserRequest Request 
        {
            get => _request;
            set
            {
                _request = value;
                OnPropertyChanged(nameof(Request));
            }
        }

        public EditProfileViewModel(IAuthService authService)
        {
            _authService = authService;
            LoadUserProfile();
        }

        public async Task LoadUserProfile()
        {
            try
            {
                var userResponse = await _authService.GetUserProfileAsync();
                Request = new UpdateCurrentUserRequest
                {
                    FirstName = userResponse.FirstName,
                    LastName = userResponse.LastName,
                    Phone = userResponse.Phone
                };
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        public async Task SaveProfileAsync()
        {
            try
            {

                if (!string.IsNullOrEmpty(Request.NewPassword))
                {
                    if (string.IsNullOrEmpty(Request.CurrentPassword))
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Текущий пароль обязателен для изменения пароля.", "OK");
                        return;
                    }

                    var response = await _authService.VerifyPasswordAsync(Request.CurrentPassword);
                    if (!response)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Неверный текущий пароль.", "OK");
                        return;
                    }
                }

                var isUpdated = await _authService.UpdateUserProfileAsync(Request);
                if (isUpdated)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех", "Профиль успешно обновлен.", "OK");
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