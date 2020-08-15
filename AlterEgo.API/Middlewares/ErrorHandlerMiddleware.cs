using AlterEgo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlterEgo.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                var response = context.Response;
                response.ContentType = "text";

                var responseCode = ex switch
                {
                    AuthenticationFailedException => HttpStatusCode.BadRequest,
                    UserAlreadyExistsException => HttpStatusCode.Conflict,

                    _ => HttpStatusCode.InternalServerError,
                };

                response.StatusCode = (int)responseCode;

                await response.WriteAsync(ex.Message);
            }
        }
    }
}
