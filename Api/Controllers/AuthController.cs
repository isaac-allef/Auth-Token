using Api.Models;
using Api.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyHolidays.Controllers
{
    [Route("v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(
            [FromServices] IAuthenticateUseCase authenticateUseCase,
            [FromBody] AccessCredentials accessCredentials)
        {
            var token = await authenticateUseCase.GetTokenAsync(accessCredentials);
            if (token is null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpGet]
        [Route("check-access")]
        [Authorize]
        public string? CheckAccess()
            => $"Welcome {User.Identity?.Name}";
    }
}