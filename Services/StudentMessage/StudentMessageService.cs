using Microsoft.Extensions.Options;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.StudentMessage;
using MVC_Project.Options;
using MVC_Project.Services.Helper;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MVC_Project.Services.StudentMessage
{
    public class StudentMessageService : IStudentMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentMessageService(HttpClient httpClient, IOptions<ApiOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<object>> AddStudentMessageAsync(StudentMessageAddRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
            var endpoint = (_options.Endpoints.StudentMessage ?? string.Empty).Trim('/');
            var url = $"{baseUrl}/{endpoint}/Add";

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(request);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }

        public async Task<ApiResponse<List<StudentDetailDto>>> GetStudentsBySectionAsync(string sectionId)
        {
            var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
            var endpoint = (_options.Endpoints.StudentDetail ?? string.Empty).Trim('/');
            var url = $"{baseUrl}/{endpoint}/by-section/{sectionId}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<StudentDetailDto>> { IsSuccess = false, Message = "Error fetching students." };

            var rawJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<StudentDetailDto>>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<List<StudentDetailDto>> { IsSuccess = false };
        }

        public async Task<ApiResponse<List<StudentMessageResponseDto>>> GetStudentMessagesAsync(string id)
        {
            var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
            var endpoint = (_options.Endpoints.StudentMessage ?? string.Empty).Trim('/');
            var url = $"{baseUrl}/{endpoint}/get?id={id}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<StudentMessageResponseDto>> { IsSuccess = false, Message = "Error fetching messages." };

            var rawJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<StudentMessageResponseDto>>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<List<StudentMessageResponseDto>> { IsSuccess = false };
        }
    }
}
