using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoOneThing.Api.Controllers.Middleware
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestContext = (RequestHeaders)context.HttpContext.RequestServices.GetService(typeof(RequestHeaders));
            requestContext.AccessToken = GetAccessToken(context);
        }

        private static string GetAccessToken(ActionContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            return authHeader.StartsWith("Bearer") ? authHeader[7..] : null;
        }
    }

    public class RequestHeaders
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
