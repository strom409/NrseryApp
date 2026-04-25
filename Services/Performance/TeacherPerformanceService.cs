using MVC_Project.Models.Performance;
using MVC_Project.Options;
using MVC_Project.Services.Helper;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MVC_Project.Services.Performance
{
    public class TeacherPerformanceService : ITeacherPerformanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;

        public TeacherPerformanceService(HttpClient httpClient, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<EmployeePerformanceResponse?> GetEmployeesPerformanceAsync(string type, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}api/EmployeePerformance/Getperformance?type={type}";
            try
            {
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return new EmployeePerformanceResponse { IsSuccess = false, Message = "Failed to fetch employee performance data." };
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<EmployeePerformanceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                return new EmployeePerformanceResponse { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<object>> UpdatePerformanceAsync(PerformanceUpdateRequest request, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}api/EmployeePerformance/AddOrUpdate";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request, ct);
                var rawJson = await response.Content.ReadAsStringAsync(ct);
                
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse != null)
                {
                    apiResponse.Status = (int)response.StatusCode;
                    if (!apiResponse.IsSuccess)
                    {
                        apiResponse.IsSuccess = response.IsSuccessStatusCode;
                    }
                    return apiResponse;
                }
            }
            catch (Exception ex)
            {
                 return new ApiResponse<object> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }

            return new ApiResponse<object> { IsSuccess = false, Message = "Failed to update performance." };
        }
    }
}
