using System.Threading.Tasks;
using DoOneThing.Api.Models;
using DoOneThing.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoOneThing.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GoogleApiService _service;

        public AuthController(GoogleApiService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<GoogleAuthResponseModel> GetToken([FromForm] string authorizationCode, [FromForm] string redirectUri)

        {
            return await _service.GetAccessToken(authorizationCode, redirectUri);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<GoogleAuthResponseModel> RefreshToken([FromForm] string refreshToken)

        {
            return await _service.RefreshAccessToken(refreshToken);
        }
    }
}