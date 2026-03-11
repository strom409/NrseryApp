using MVC_Project.Models.Auth;
using MVC_Project.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MVC_Project.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;

        public AuthService(HttpClient httpClient, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.Login}";
            var response = await _httpClient.PostAsJsonAsync(url, request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Invalid login"
                };
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>(ct);
        }
    }
}