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

        // verify access token: GET https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=accessToken

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
                _requestHeaders.AccessToken = await RefreshAccessToken();
                res = await MakeHttpRequest<T>(url, method, reqData);
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

        private async Task<string> RefreshAccessToken()
        {
            var reqData = new
            {
                _appSettings.GoogleCredentials.client_id,
                _appSettings.GoogleCredentials.client_secret,
                refresh_token = _requestHeaders.RefreshToken,
                grant_type = "refresh_token"
            };

            var resData = await MakeRequest<GoogleAuthResponseModel>("https://oauth2.googleapis.com/token", HttpMethod.Post, reqData);
            return resData.access_token;
        }
    }
}