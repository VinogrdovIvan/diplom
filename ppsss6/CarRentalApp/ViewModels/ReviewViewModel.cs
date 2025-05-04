using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using CarRentalApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarRentalApp.ViewModels
{
    public class ReviewViewModel : INotifyPropertyChanged
    {
        private readonly IReviewService _reviewService;
        private int _orderId;

        public ObservableCollection<ReviewResponse> Reviews { get; } = new();
        public ObservableCollection<int> RatingOptions { get; } = new() { 1, 2, 3, 4, 5 };

        private int _rating;
        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        public ICommand LoadReviewsCommand { get; }
        public ICommand SubmitReviewCommand { get; }
        public ICommand DeleteReviewCommand { get; }

        public ReviewViewModel(IReviewService reviewService)
        {
            _reviewService = reviewService;

            LoadReviewsCommand = new Command(async () => await LoadReviewsAsync());
            SubmitReviewCommand = new Command(async () => await SubmitReviewAsync());
            DeleteReviewCommand = new Command<int>(async (id) => await DeleteReviewAsync(id));
        }

        public async Task Initialize(int orderId)
        {
            if (orderId <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Неверный идентификатор заказа", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            _orderId = orderId;
            await LoadReviewsAsync();
        }

        private async Task LoadReviewsAsync()
        {
            try
            {
                IsBusy = true;
                Reviews.Clear();

                var reviews = await _reviewService.GetReviewsByOrderAsync(_orderId);
                foreach (var review in reviews)
                {
                    Reviews.Add(review);
                }
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                await Shell.Current.DisplayAlert("Информация", "Отзывов пока нет", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitReviewAsync()
        {
            try
            {
                if (Rating < 1 || Rating > 5)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Оценка должна быть от 1 до 5", "OK");
                    return;
                }

                if (_orderId <= 0)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Неверный идентификатор заказа", "OK");
                    return;
                }

                IsBusy = true;

                var request = new CreateReviewRequest
                {
                    OrderId = _orderId,
                    Rating = Rating,
                    Comment = Comment
                };

                var response = await _reviewService.CreateReviewAsync(request);

                if (response == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось создать отзыв", "OK");
                    return;
                }

                Reviews.Insert(0, response);
                Rating = 0;
                Comment = string.Empty;

                await Shell.Current.DisplayAlert("Успех", "Отзыв сохранен", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось сохранить отзыв: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteReviewAsync(int reviewId)
        {
            try
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Подтверждение",
                    "Удалить этот отзыв?",
                    "Да", "Нет");

                if (!confirm) return;

                IsBusy = true;
                await _reviewService.DeleteReviewAsync(reviewId);

                var review = Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
                if (review != null)
                    Reviews.Remove(review);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}