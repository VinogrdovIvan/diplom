using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<UserResponse> GetUserProfileAsync();
        Task<bool> UpdateUserProfileAsync(UpdateCurrentUserRequest request);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> VerifyPasswordAsync(string currentPassword);
    }
}