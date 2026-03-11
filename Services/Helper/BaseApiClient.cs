using Microsoft.Extensions.Options;
using MVC_Project.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVC_Project.Services.Variety
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient HttpClient;
        protected readonly ApiOptions Options;

        protected BaseApiClient(HttpClient httpClient, IOptions<ApiOptions> options)
        {
            HttpClient = httpClient;
            Options = options.Value;
            ConfigureBaseAddress();
        }

        private void ConfigureBaseAddress()
        {
            if (string.IsNullOrWhiteSpace(Options.BaseUrl)) return;
            var baseUrl = Options.BaseUrl.EndsWith('/') ? Options.BaseUrl : $"{Options.BaseUrl}/";
            HttpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        }

        protected string NormalizeEndpoint(string endpoint)
            => string.IsNullOrWhiteSpace(endpoint) ? string.Empty : endpoint.TrimStart('/');

        protected void AddAuthHeaders(HttpRequestMessage request, string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
                }
                catch
                {
                    request.Headers.TryAddWithoutValidation("token", token);
                }
            }
        }

        protected static async Task<ApiResponse<T>> HandleResponseAsync<T>(
            HttpResponseMessage response,
            CancellationToken ct = default)
        {
            var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    options.Converters.Add(new CustomDateTimeConverter());

                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, options);

                    if (apiResponse != null)
                    {
                        apiResponse.Status = (int)response.StatusCode;
                        return apiResponse;
                    }
                }
                catch
                {
                    // If JSON deserialization fails (e.g. plain text response), use the raw content as the message
                    return new ApiResponse<T>
                    {
                        IsSuccess = false,
                        Message = json,
                        Status = (int)response.StatusCode,
                        ResponseData = default
                    };
                }
            }

            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = response.IsSuccessStatusCode
                    ? "Invalid or empty response from server"
                    : $"Server error: {(int)response.StatusCode} {response.ReasonPhrase}",
                Status = (int)response.StatusCode,
                ResponseData = default
            };
        }
    }
}
