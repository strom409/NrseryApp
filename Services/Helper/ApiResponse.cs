namespace MVC_Project.Services.Variety
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("data")]
        public T? ResponseData { get; set; }
        public object? Error { get; set; }
    }
}
