using AdminPanel.Models;
using AdminPanel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace AdminPanel.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        protected readonly ApiClient _apiClient;

        [ObservableProperty]
        protected Order _order;

        public OrderViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
        }

        protected void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        protected void ShowLoginWindow()
        {
            App.Token = null;
            new Views.LoginWindow().Show();
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }
    }
}