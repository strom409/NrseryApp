using MVC_Project.Models.Topper;
using MVC_Project.Options;
using MVC_Project.Services.Helper;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MVC_Project.Services.Topper
{
    public class TopperService : ITopperService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;

        public TopperService(HttpClient httpClient, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<TopperResponse?> GetToppersAsync(CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}gettoppers";
            try
            {
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return new TopperResponse { IsSuccess = false, Message = "Failed to fetch toppers." };
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<TopperResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                return new TopperResponse { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<object>> SaveTopperAsync(TopperAddRequest request, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}savetopper";
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

            return new ApiResponse<object> { IsSuccess = false, Message = "Failed to save topper." };
        }

        public async Task<ApiResponse<object>> DeleteTopperAsync(long tid, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}deletetopper/{tid}";
            try
            {
                var response = await _httpClient.DeleteAsync(url, ct);
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

            return new ApiResponse<object> { IsSuccess = false, Message = "Failed to delete topper." };
        }
    }
}
