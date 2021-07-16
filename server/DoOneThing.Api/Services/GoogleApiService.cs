using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DoOneThing.Api.Controllers.Middleware;
using DoOneThing.Api.Models;
using Microsoft.Extensions.Options;

namespace DoOneThing.Api.Services
{
    public class GoogleApiService
    {
        private readonly RequestHeaders _requestHeaders;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;

        public GoogleApiService(RequestHeaders requestHeaders, IOptions<AppSettings> settings, IHttpClientFactory clientFactory)
        {
            _requestHeaders = requestHeaders;
            _clientFactory = clientFactory;
            _appSettings = settings.Value;
        }

        public async Task<T> MakeRequest<T>(string url, HttpMethod method, object data = null)
        {
            var reqData = data != null
                ? new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json")
                : null;

            var res = await MakeHttpRequest<T>(url, method, reqData);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Invalid or expired access token");
            }

            res.EnsureSuccessStatusCode();

            await using var responseStream = await res.Content.ReadAsStreamAsync();
            var resData = await JsonSerializer.DeserializeAsync<T>(responseStream);

            return resData;
        }

        private async Task<HttpResponseMessage> MakeHttpRequest<T>(string url, HttpMethod method, HttpContent reqData)
        {
            using var client = _clientFactory.CreateClient();
            return await client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = method,
                Content = reqData,
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _requestHeaders.AccessToken) }
            });
        }

        public async Task<GoogleAuthResponseModel> GetAccessToken(string authorizationCode)
        {
            var reqData = new
            {
                _appSettings.GoogleCredentials.client_id,
                _appSettings.GoogleCredentials.client_secret,
                _appSettings.GoogleCredentials.redirect_uri,
                code = authorizationCode,
                grant_type = "authorization_code"
            };

            return await MakeRequest<GoogleAuthResponseModel>("https://oauth2.googleapis.com/token", HttpMethod.Post, reqData);
        }

        public async Task<GoogleAuthResponseModel> RefreshAccessToken(string refreshToken)
        {
            var reqData = new
            {
                _appSettings.GoogleCredentials.client_id,
                _appSettings.GoogleCredentials.client_secret,
                refresh_token = refreshToken,
                grant_type = "refresh_token"
            };

            return await MakeRequest<GoogleAuthResponseModel>("https://oauth2.googleapis.com/token", HttpMethod.Post, reqData);
        }
    }
}