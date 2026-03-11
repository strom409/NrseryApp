using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.Slider;
using MVC_Project.Options;
using Microsoft.Extensions.Options;
using MVC_Project.Services.Variety;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MVC_Project.Services.Slider
{
    public class SliderService : ISliderService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SliderService(HttpClient httpClient, IOptions<ApiOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SliderResponse?> GetAllSlidersAsync(CancellationToken ct = default)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/1_0";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (session != null && !string.IsNullOrEmpty(session.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
            }

            var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode) return new SliderResponse { IsSuccess = false, Data = new List<SliderData>() };

            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<SliderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<object>> AddSliderAsync(SliderRequest requestData, CancellationToken ct = default)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/AddOrupdateSlider";
            
            var json = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(request, ct);
            var rawJson = await response.Content.ReadAsStringAsync(ct);

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse != null)
                {
                    apiResponse.Status = (int)response.StatusCode;
                    // If IsSuccess isn't set by backend, infer from HTTP status
                    if (!apiResponse.IsSuccess)
                    {
                        apiResponse.IsSuccess = response.IsSuccessStatusCode;
                    }
                    return apiResponse;
                }
            }
            catch
            {
                // Fall through to generic error
            }

            return new ApiResponse<object>
            {
                IsSuccess = response.IsSuccessStatusCode,
                Status = (int)response.StatusCode,
                Message = response.IsSuccessStatusCode
                    ? "Operation completed successfully."
                    : $"Server error: {(int)response.StatusCode} {response.ReasonPhrase}"
            };
        }
    }
}
