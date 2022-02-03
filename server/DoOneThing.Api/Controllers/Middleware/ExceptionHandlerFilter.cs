using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace DoOneThing.Api.Controllers.Middleware
{
    public class ExceptionHandlerFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                Content = JsonConvert.SerializeObject(new { title = context.Exception.MessageStack() }),
                ContentType = "application/json; charset=UTF-8",
                StatusCode = context.Exception is ApiException e
                    ? (int)e.StatusCode
                    : (int)HttpStatusCode.InternalServerError
            };
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }

        public HttpStatusCode StatusCode { get; set; }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.Unauthorized;
        }
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.NotFound;
        }
    }

    public class ReferentialIntegrityException : ApiException
    {
        public ReferentialIntegrityException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
        }
    }

    public class InternalException : ApiException
    {
        public InternalException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }

    public static class ExceptionExtensions
    {
        public static string MessageStack(this Exception e)
        {
            var message = "";

            var ex = e;
            while (ex != null)
            {
                message += ex.Message + " ";
                ex = ex.InnerException;
            }

            return message;
        }
    }
}

