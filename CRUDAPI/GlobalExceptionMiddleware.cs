using System.Net;
using System.Text.Json;

namespace CRUDAPI
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // 執行下一個中介軟體
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}", ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            // 根據錯誤類型設定不同的 HTTP 狀態碼
            response.StatusCode = exception switch
            {
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // 回傳錯誤訊息
            var errorResponse = new
            {
                StatusCode = response.StatusCode,
                Message = exception.Message,
                Detail = exception.InnerException?.Message
            };

            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
