using DatingApp.Application.Errors;
using System.Net;
using System.Text.Json;

namespace DatingApp.Api.Middlewares
{
    public class ExceptionsMiddleware (RequestDelegate next,ILogger<ExceptionsMiddleware> logger,
        IHostEnvironment environment)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"{message}",ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = environment.IsDevelopment() ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiExceptions(context.Response.StatusCode, ex.Message, "internal Server error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }

        }
    }
}
