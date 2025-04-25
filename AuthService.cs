using System.Net.Http;
using System.Net.Http.Json;
using CarRental.Shared.Requests;

namespace AdminPanel.Services;

public interface IAuthService
{
    Task<HttpResponseMessage> LoginAsync(LoginRequest request);
    Task<HttpResponseMessage> RefreshTokenAsync(RefreshTokenRequest request);
}

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> LoginAsync(LoginRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("Backend");
        var response = await httpClient.PostAsJsonAsync("api/Auth/login", request);
        return response;
    }

    public Task<HttpResponseMessage> RefreshTokenAsync(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }
}
