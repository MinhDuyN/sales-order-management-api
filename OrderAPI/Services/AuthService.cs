using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderAPI.Data;
using OrderAPI.DTOs.Auth.Login;
using OrderAPI.DTOs.Auth.Logout;
using OrderAPI.DTOs.Auth.RefreshToken;
using OrderAPI.DTOs.Auth.Register;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _configurationSection;
        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _configurationSection = _configuration.GetSection("JwtSettings");
        }

        private string GenerateAccessToken(User user)
        {
            if (user == null)
            {
                throw new BadRequestException("User is not valid");
            }
            var secretToken = _configurationSection["SecretKey"];
            if (secretToken == null)
            {
                throw new BadRequestException("Secret token is not valid");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role!.RoleName),
            };
            var token = new JwtSecurityToken(
                issuer: _configurationSection["Issuer"],
                audience: _configurationSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configurationSection["AccessTokenExpiryMinutes"]!)),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var checkEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (checkEmail != null)
            {
                throw new ConflictDataException("This email is exists");
            }
            var checkRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Customer");
            if (checkRole == null)
            {
                throw new BadRequestException("Role is not valid");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            var register = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = checkRole.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
            };
            await _context.Users.AddAsync(register);
            await _context.SaveChangesAsync();
            return new RegisterResponse
            {
                Messenger = "Register success!",
                Username = register.Name,
                Email = register.Email,
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var checkLogin = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            if (checkLogin == null)
            {
                throw new NotFoundException("Email is not exists");
            }
            else
            {
                bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, checkLogin.PasswordHash);
                if (!validPassword)
                {
                    throw new BadRequestException("Password is not valid");
                }
                var accessToken = GenerateAccessToken(checkLogin);
                var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var expiresDay = DateTime.UtcNow.AddDays(double.Parse(_configurationSection["RefreshTokenExpiryDays"]!));
                var oldToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == checkLogin.Id);
                if (oldToken != null)
                {
                    _context.RefreshTokens.Remove(oldToken);
                }
                var login = new RefreshToken
                {
                    UserId = checkLogin.Id,
                    Token = refreshToken,
                    ExpiresAt = expiresDay,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.AddAsync(login);
                await _context.SaveChangesAsync();
                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
        }

        public async Task<RefreshTokenReponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var checkToken = await _context.RefreshTokens
                .Include(x => x.User)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);
            if (checkToken == null)
            {
                throw new NotFoundException("Token is not found");
            }
            else
            {
                if (DateTime.UtcNow > checkToken.ExpiresAt)
                {
                    throw new BadRequestException("This token is expired");
                }
                else
                {
                    var accessToken = GenerateAccessToken(checkToken.User);
                    var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                    var expiresDay = DateTime.UtcNow.AddDays(double.Parse(_configurationSection["RefreshTokenExpiryDays"]!));
                    var oldToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == checkToken.UserId);
                    if (oldToken != null)
                    {
                        _context.Remove(oldToken);
                    }
                    var newToken = new RefreshToken
                    {
                        UserId = checkToken.UserId,
                        Token = refreshToken,
                        ExpiresAt = expiresDay,
                        CreatedDate = DateTime.UtcNow
                    };
                    await _context.AddAsync(newToken);
                    await _context.SaveChangesAsync();
                    return new RefreshTokenReponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                    };
                }
            }
        }

        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var logout = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken);
            if (logout != null)
            {
                _context.Remove(logout);
                await _context.SaveChangesAsync();
                return new LogoutResponse
                {
                    Messenger = "Logout success"
                };
            }
            else
            {
                throw new NotFoundException("This token is not found");
            }
        }
    }
}
