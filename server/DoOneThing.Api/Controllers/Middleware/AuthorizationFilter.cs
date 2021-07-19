using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoOneThing.Api.Controllers.Middleware
{
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

            // TODO: replace with [AllowAnonymous]
            if (context.HttpContext.Request.Path != "/api/auth/refresh")
            {
                // even if the request will not be calling a Google API, use Google OAuth to authorize the entire request,
                // thus preventing anonymous endpoints

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
    }

    public class RequestHeaders
    {
        public string AccessToken { get; set; }
    }
}
