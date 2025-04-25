using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class EditUserViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private User _user;

        public EditUserViewModel(User user)
        {
            _apiClient = new ApiClient("http://localhost:5299/api");
            User = user;
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            await _apiClient.PutAsync<User>($"users/{User.UserId}", User);

            // Закрываем окно после успешного сохранения
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