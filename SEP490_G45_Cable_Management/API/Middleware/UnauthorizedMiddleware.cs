using Common.Base;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Xử lý tiếp chuỗi middleware
            await _next(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";
                ResponseBase response = new ResponseBase("Unauthorized", (int)HttpStatusCode.Unauthorized);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
