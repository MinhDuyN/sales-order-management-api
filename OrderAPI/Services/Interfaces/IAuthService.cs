using OrderAPI.DTOs.Auth.Login;
using OrderAPI.DTOs.Auth.Logout;
using OrderAPI.DTOs.Auth.RefreshToken;
using OrderAPI.DTOs.Auth.Register;

namespace OrderAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<RefreshTokenReponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    }
}
