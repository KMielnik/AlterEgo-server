using AlterEgo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlterEgo.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(
            RequestDelegate next, 
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception thrown while processing request");
                var response = context.Response;
                response.ContentType = "text";

                var responseCode = ex switch
                {
                    AuthenticationFailedException => HttpStatusCode.BadRequest,
                    UserAlreadyExistsException => HttpStatusCode.Conflict,
                    UnauthorizedAccessException => HttpStatusCode.Forbidden,
                    UnsupportedMediaTypeException => HttpStatusCode.UnsupportedMediaType,
                    FileNotFoundException => HttpStatusCode.NotFound,

                    _ => HttpStatusCode.InternalServerError,
                };

                response.StatusCode = (int)responseCode;

                _logger.LogError("Response code - {ResponseCode}, Message - {Message}", response.StatusCode, ex.Message);

                await response.WriteAsync(ex.Message);
            }
        }
    }
}
