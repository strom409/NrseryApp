using Microsoft.Extensions.Options;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.Notification;
using MVC_Project.Options;
using MVC_Project.Services.Helper;
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

            string base64File = string.Empty;
            string fileExtension = model.FileExtension ?? string.Empty;

            if (!string.IsNullOrEmpty(model.Filepath))
            {
                if (model.Filepath.Contains(","))
                {
                    // Case 1: New file uploaded (Base64)
                    var parts = model.Filepath.Split(',');
                    base64File = parts[1];
                    
                    // Extract extension from MIME type if possible
                    if (parts[0].Contains(";base64"))
                    {
                        var mime = parts[0].Split(':')[1].Split(';')[0];
                        if (mime.Contains("/"))
                            fileExtension = "." + mime.Split('/')[1];
                    }
                }
                else if (model.Filepath.StartsWith("http") || model.Filepath.Contains("Assignments"))
                {
                    // Case 2: Existing file (URL or relative path) - Fetch and convert to Base64
                    try
                    {
                        var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
                        var fullUrl = model.Filepath.StartsWith("http") 
                            ? model.Filepath 
                            : $"{baseUrl}/{model.Filepath.TrimStart('/')}";

                        var fileBytes = await _httpClient.GetByteArrayAsync(fullUrl);
                        base64File = Convert.ToBase64String(fileBytes);
                        
                        if (string.IsNullOrEmpty(fileExtension))
                        {
                            fileExtension = Path.GetExtension(model.Filepath) ?? ".pdf";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching existing notification file: {ex.Message}");
                        // If we can't fetch it, we might have to send empty or fail, 
                        // but usually it's because it's already a local reference or broken.
                    }
                }
            }

            var requestBody = new
            {
                notificationID = model.NotificationID,
                title = model.Title,
                description = model.Description,
                filepath = base64File,
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

        public async Task<ApiResponse<object>> AddOrUpdateDownloadsAsync(DownloadUploadRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Notification/AddOrupdateDownloads";

            string base64File = "";
            string fileExtension = "";

            if (model.File != null && model.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await model.File.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    base64File = Convert.ToBase64String(fileBytes);
                }
                fileExtension = Path.GetExtension(model.File.FileName);
            }
            else if (!string.IsNullOrEmpty(model.FilePath))
            {
                // No new file: fetch existing file from API server and send again as Base64
                var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
                var relativePath = model.FilePath.TrimStart('/');
                var remoteUrl = $"{baseUrl}/{relativePath}";

                try
                {
                    var fileBytes = await _httpClient.GetByteArrayAsync(remoteUrl);
                    base64File = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(relativePath) ?? string.Empty;
                }
                catch (Exception ex)
                {
                    // Fallback or log error
                    Console.WriteLine($"Error fetching existing file: {ex.Message}");
                }
            }

            var requestBody = new
            {
                assignmentID = int.TryParse(model.NotificationID, out var id) ? id : 0,
                title = model.Title,
                filePath = base64File,
                fileExtension = fileExtension,
                date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                uploadedBy = session.UserName ?? string.Empty,
                classIdFk = model.ClassIdFk,
                subjectId = model.SubjectId,
                uploadType = model.UploadType,
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
            var base64Photo = model.Base64Photo ?? string.Empty;

            // Robust Base64 Resolution:
            // 1. If it's a DataURL (contains ','), strip the prefix.
            // 2. If it's a path (contains '/' or starts with http), fetch and convert to Base64.
            if (!string.IsNullOrEmpty(base64Photo))
            {
                if (base64Photo.Contains(","))
                {
                    base64Photo = base64Photo.Split(',')[1];
                }
                else if (base64Photo.StartsWith("http") || base64Photo.Contains("/"))
                {
                    try
                    {
                        var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
                        var fullUrl = base64Photo.StartsWith("http") 
                            ? base64Photo 
                            : $"{baseUrl}/{base64Photo.TrimStart('/')}";

                        var fileBytes = await _httpClient.GetByteArrayAsync(fullUrl);
                        base64Photo = Convert.ToBase64String(fileBytes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error resolving notification image path {base64Photo} to Base64: {ex.Message}");
                    }
                }
            }

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

        public async Task<ApiResponse<object>> AddOrUpdateCircularAsync(CircularAddRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Notification/AddOrupdateCircular";

            // If no new file is uploaded (FilePath is empty/null) but it's an update (ActionType 2),
            // we should fetch the existing file and send it back as Base64 to satisfy the API.
            if (model.ActionType == 2 && string.IsNullOrEmpty(model.FilePath) && model.NotificationID > 0)
            {
                try
                {
                    // Fetch existing data to get the current file path
                    var existingData = await GetNotificationDataAsync($"3_{model.ClassidID},{model.SectionID}");
                    if (existingData.IsSuccess && existingData.ResponseData != null)
                    {
                        var circular = existingData.ResponseData.FirstOrDefault(x => x.NotificationId == model.NotificationID);
                        if (circular != null && !string.IsNullOrEmpty(circular.FilePath))
                        {
                            var baseUrl = (_options.BaseUrl ?? string.Empty).TrimEnd('/');
                            var remoteUrl = circular.FilePath.StartsWith("http") 
                                ? circular.FilePath 
                                : $"{baseUrl}/{circular.FilePath.TrimStart('/')}";

                            var fileBytes = await _httpClient.GetByteArrayAsync(remoteUrl);
                            model.FilePath = Convert.ToBase64String(fileBytes);
                            model.FileExtension = Path.GetExtension(circular.FilePath) ?? ".pdf";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error preserving circular file: {ex.Message}");
                }
            }

            var json = JsonSerializer.Serialize(model);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(requestMessage);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }

        public async Task<ApiResponse<List<EventResponse>>> GetEventDataAsync(string date)
        {
            // Format: 9_yyyy-MM-dd
            var id = $"9_{date}";
            var url = $"{_options.BaseUrl}{_options.Endpoints.Notification}/{id}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<EventResponse>> { IsSuccess = false, Message = "Error fetching event data." };

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ApiResponse<List<EventResponse>>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new ApiResponse<List<EventResponse>> { IsSuccess = false };
        }

        public async Task<ApiResponse<object>> AddOrUpdateEventAsync(EventAddRequest model)
        {
            var session = _httpContextAccessor.HttpContext?
                .Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null || string.IsNullOrEmpty(session.Token))
                return new ApiResponse<object> { IsSuccess = false, Message = "Session expired. Please login again." };

            var url = $"{_options.BaseUrl}api/Notification/AddOrupdateEvent";
            
            // Set createdBy from session
            model.CreatedBy = session.UserName ?? "Admin";

            var json = JsonSerializer.Serialize(model);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);

            var response = await _httpClient.SendAsync(requestMessage);
            var rawJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<object> { IsSuccess = false, Message = "No response from server." };
        }
    }
}