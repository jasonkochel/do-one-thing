using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoOneThing.Api.Controllers.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminOnlyAttribute : Attribute { }

    public class AuthorizationFilter : ActionFilterAttribute
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthorizationFilter(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestContext = (RequestHeaders)context.HttpContext.RequestServices.GetService(typeof(RequestHeaders));
            requestContext.AccessToken = GetAccessToken(context);

            if (GetControllerAccessLevel(context) != AccessLevel.Anonymous)
            {
                // even if the request will not be calling a Google API, use Google OAuth to ensure the caller is logged into the overall app

                var client = _clientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v1/tokeninfo");

                request.Headers.Add("Authorization", $"Bearer {requestContext.AccessToken}");

                var response = client.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new UnauthorizedException("Access token validation failed");
                }
            }
        }

        private static string GetAccessToken(ActionContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            return authHeader.StartsWith("Bearer") ? authHeader[7..] : null;
        }

        private static AccessLevel GetControllerAccessLevel(ActionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                var controllerAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true);
                var methodAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);

                if (methodAttributes.Any(a => a.GetType() == typeof(AdminOnlyAttribute)) ||
                    controllerAttributes.Any(a => a.GetType() == typeof(AdminOnlyAttribute)))
                {
                    return AccessLevel.Admin;
                }
                if (methodAttributes.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)) ||
                    controllerAttributes.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)))
                {
                    return AccessLevel.Anonymous;
                }

            }

            return AccessLevel.User;
        }

        private enum AccessLevel
        {
            Anonymous,
            User,
            Admin
        }

    }

    public class RequestHeaders
    {
        public string AccessToken { get; set; }
    }
}
