namespace OrderAPI.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse(bool success, string message, T? data, List<string>? errors)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
        }
        public static ApiResponse<T> Ok(string message, T? data)
        {
            return new ApiResponse<T>(true, message, data, null);
        }
        public static ApiResponse<T> Fail(string message, List<string>? errors)
        {
            return new ApiResponse<T>(false, message, default,errors);
        }

    }
}
