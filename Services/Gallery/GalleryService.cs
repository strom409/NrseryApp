using Microsoft.Extensions.Options;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.Gallery;
using MVC_Project.Options;
using MVC_Project.Services.Helper;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MVC_Project.Services.Gallery
{
    public class GalleryService : IGalleryService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GalleryService(HttpClient httpClient, IOptions<ApiOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<GalleryHeading>>> GetGalleryHeadingsAsync()
        {
            var url = $"{_options.BaseUrl}api/Gallery/get?id=0_1";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<GalleryHeading>> { IsSuccess = false, Message = "Error fetching gallery headings." };

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<GalleryHeading>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<List<GalleryHeading>> { IsSuccess = false };
        }

        public async Task<ApiResponse<List<GalleryItem>>> GetGalleryItemsAsync(int gid)
        {
            var url = $"{_options.BaseUrl}api/Gallery/get?id=1_{gid}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<GalleryItem>> { IsSuccess = false, Message = "Error fetching gallery items." };

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<GalleryItem>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<List<GalleryItem>> { IsSuccess = false };
        }

        public async Task<ApiResponse<object>> AddGalleryAsync(GalleryAddRequest request)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Gallery/AddGallery";
            
            var json = JsonSerializer.Serialize(request);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(httpRequest);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }

        public async Task<ApiResponse<object>> DeleteGalleryItemAsync(int gdid)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Gallery/get?id=3_{gdid}";
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(httpRequest);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }

        public async Task<ApiResponse<object>> AddGalleryHeadingAsync(GalleryHeadingRequest request)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Gallery/AddHeading";

            var json = JsonSerializer.Serialize(request);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(httpRequest);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }
    }
}
