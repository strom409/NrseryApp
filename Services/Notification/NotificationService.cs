using Microsoft.Extensions.Options;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.Notification;
using MVC_Project.Options;
using MVC_Project.Services.Variety;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MVC_Project.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(HttpClient httpClient, IOptions<ApiOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<ContactPerson>>> GetContactDataAsync(string id)
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/{id}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<ContactPerson>> { IsSuccess = false, Message = "Error fetching contact data." };

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ApiResponse<List<ContactPerson>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new ApiResponse<List<ContactPerson>> { IsSuccess = false };
        }
        public async Task<ApiResponse<List<NotificationResponse>>> GetAllNotificationsAsync()
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/0_0";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<NotificationResponse>> { IsSuccess = false, Message = "Error fetching notifications." };

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ApiResponse<List<NotificationResponse>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new ApiResponse<List<NotificationResponse>> { IsSuccess = false };
        }

        public async Task<ApiResponse<object>> AddOrUpdateNotificationAsync(NotificationAddUpdateRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/AddOrupdatenotification";

            // Base64 prefix hata do agar hai
            var filepath = model.Filepath ?? string.Empty;
            string fileExtension = string.Empty;
            if (filepath.Contains(","))
            {
                // data:image/jpeg;base64,xxx — extension nikalo
                var mime = filepath.Split(';')[0].Split('/')[1];
                fileExtension = "." + mime;
                filepath = filepath.Split(',')[1];
            }
            else if (filepath.StartsWith("http"))
            {
                filepath = string.Empty;
            }

            var requestBody = new
            {
                notificationID = model.NotificationID,
                title = model.Title,
                description = model.Description,
                filepath,
                notificationDate = model.NotificationDate,
                actionType = model.ActionType,
                username = session.UserName ?? string.Empty,
                fileExtension
            };

            var json = JsonSerializer.Serialize(requestBody);
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
        public async Task<ApiResponse<object>> AddOrUpdateContactAsync(ContactUpdateRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/AddOrUpdateContact";

            //var requestBody = new
            //{
            //    cid = model.Cid,
            //    personalName = model.PersonalName,
            //    designation = model.Designation,
            //    phoneNo = model.PhoneNo,
            //    email = model.Email,
            //    filepath = model.Filepath,
            //    actionType = model.ActionType
            //};
            // Base64 prefix hata do agar hai
            //var filepath = model.Filepath ?? string.Empty;
            //if (filepath.Contains(","))
            //    filepath = filepath.Split(',')[1];
            var filepath = model.Filepath ?? string.Empty;

            // Agar Base64 hai toh prefix hata do
            if (filepath.Contains(","))
                filepath = filepath.Split(',')[1];
            // Agar URL hai (http/https) toh empty bhejo — backend apni photo rakhega
            else if (filepath.StartsWith("http"))
                filepath = string.Empty;

            var requestBody = new
            {
                cid = model.Cid,
                personalName = model.PersonalName,
                designation = model.Designation,
                phoneNo = model.PhoneNo,
                email = model.Email,
                filepath,
                actionType = model.ActionType
            };

            var json = JsonSerializer.Serialize(requestBody);
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

        public async Task<ApiResponse<List<NotificationResponse>>> GetNotificationDataAsync(string id)
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/{id}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<NotificationResponse>> { IsSuccess = false, Message = "Error fetching data." };

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ApiResponse<List<NotificationResponse>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new ApiResponse<List<NotificationResponse>> { IsSuccess = false };
        }

        public async Task<ApiResponse<object>> UpdateNotificationAsync(NotificationUpdateRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/UpdateSchoolData";
            // data:image/jpeg;base64, prefix hata do
            var base64Photo = model.Base64Photo ?? string.Empty;
            if (base64Photo.Contains(","))
                base64Photo = base64Photo.Split(',')[1];
            var requestBody = new
            {
                model.Description,
                Base64Photo = base64Photo,
                model.Type
            };

            var json = JsonSerializer.Serialize(requestBody);
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
    }
}