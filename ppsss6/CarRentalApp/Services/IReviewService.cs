using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetReviewsByOrderAsync(int orderId);
        Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}