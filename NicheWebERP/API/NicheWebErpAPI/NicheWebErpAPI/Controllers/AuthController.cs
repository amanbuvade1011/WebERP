using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        // POST api/Auth/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result is null)
            {
                // Deliberately generic - don't reveal whether the username existed.
                return Unauthorized(new { message = "Invalid username or password." });
            }
            return Ok(result);
        }

        // POST api/Auth/Logout
        // No-op for a pure JWT scheme - the client discards the token. Kept as a real endpoint
        // so the frontend has a consistent call to make, and so a server-side blacklist can be
        // added later without an API shape change.
        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout() => Ok();

        // GET api/Auth/Me
        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<CurrentUserDto>> Me()
        {
            var user = await _authService.GetCurrentUserAsync(_currentUserService.UserId);
            if (user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
    }
}
