using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using OrderAPI.DTOs.Auth.Login;
using OrderAPI.DTOs.Auth.Logout;
using OrderAPI.DTOs.Auth.RefreshToken;
using OrderAPI.DTOs.Auth.Register;
using OrderAPI.DTOs.Common;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) 
            {
                var errors = ModelState.Values.SelectMany(x=>x.Errors).Select(x=>x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
                var register = await _authService.RegisterAsync(request);
                return StatusCode(201, ApiResponse<RegisterResponse>.Ok("Register successfully", register));
            
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var login = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponse>.Ok("Login succesfully", login));
            
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var refreshToken = await _authService.RefreshTokenAsync(request);
            return Ok(ApiResponse<RefreshTokenReponse>.Ok("RefreshToken succesfully", refreshToken));
            
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            await _authService.LogoutAsync(request);
            return NoContent();
           
        }

    }
}
