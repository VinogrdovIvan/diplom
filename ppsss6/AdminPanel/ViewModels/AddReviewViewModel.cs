using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace AdminPanel.ViewModels
{
    public partial class AddReviewViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private int _orderId;

        [ObservableProperty]
        private int _rating = 5;

        [ObservableProperty]
        private string _comment;

        public AddReviewViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5299");
            _apiClient.SetToken(App.Token);
        }

        [RelayCommand]
        private async Task AddReview()
        {
            try
            {
                if (OrderId <= 0)
                {
                    MessageBox.Show("Введите номер заказа");
                    return;
                }

                if (Rating < 1 || Rating > 5)
                {
                    MessageBox.Show("Оценка должна быть от 1 до 5");
                    return;
                }

                var review = new Review
                {
                    OrderId = OrderId,
                    Rating = Rating,
                    Comment = Comment,
                    ReviewDate = DateTime.Now
                };

                var response = await _apiClient.PostAsync("reviews", review);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Отзыв успешно добавлен");
                    CloseWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CloseWindow()
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
    }
}