namespace MessengerClone.API.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }
    }
}
