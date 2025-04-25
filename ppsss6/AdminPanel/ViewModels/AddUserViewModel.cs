using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using static Xamarin.Essentials.Permissions;
using Xamarin.Essentials;

namespace AdminPanel.ViewModels
{
    public partial class AddUserViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _phone;

        [ObservableProperty]
        private string _password;

        public AddUserViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299/api");
        }

        [RelayCommand]
        private async Task AddUser()
        {
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Phone = Phone,
                Password = Password
            };

            await _apiClient.PostAsync("users", user);

            // Закрываем окно после успешного добавления
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // Закрываем окно
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}