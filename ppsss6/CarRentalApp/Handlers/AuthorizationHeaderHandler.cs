using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.SE.Omapi;
using CarRental.Shared.Requests;
using CarRentalApp.Services;

namespace CarRentalApp.Handlers
{
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private bool isRefreshing;

        public AuthorizationHeaderHandler(
            IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await SecureStorage.GetAsync("access_token");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (isRefreshing || string.IsNullOrWhiteSpace(accessToken) ||
                response.StatusCode != HttpStatusCode.Unauthorized)
                return response;
            try
            {
                isRefreshing = true;

                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                var refreshTokenRequest = new RefreshTokenRequest() { AccessToken = accessToken, RefreshToken = refreshToken};

                var tokenResponse = await _authService.RefreshTokenAsync(refreshTokenRequest);
                if (tokenResponse != null) 
                {
                    await SecureStorage.SetAsync("access_token", tokenResponse.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
                    if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            finally
            {
                isRefreshing = false;
            }
            return response;
        }

    }
}
