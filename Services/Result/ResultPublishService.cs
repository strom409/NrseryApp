using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MVC_Project.Models.Result;
using MVC_Project.Options;
using MVC_Project.Services.Helper;

namespace MVC_Project.Services.Result
{
    public class ResultPublishService : IResultPublishService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiOptions _options;

        public ResultPublishService(HttpClient httpClient, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<ResultStatusResponse?> GetResultStatusAsync(int classId, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}getresultstatus/{classId}";
            try
            {
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResultStatusResponse { IsSuccess = false, Message = "Failed to fetch result status." };
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<ResultStatusResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                return new ResultStatusResponse { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<object>> SaveResultStatusAsync(ResultStatusRequest request, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl}saveresultstatus";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request, ct);
                var rawJson = await response.Content.ReadAsStringAsync(ct);
                
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse != null)
                {
                    return apiResponse;
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }

            return new ApiResponse<object> { IsSuccess = false, Message = "Failed to save result status." };
        }
    }
}
