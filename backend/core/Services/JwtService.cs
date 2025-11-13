using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Services.RedisService;

namespace GymManagement.Services.JwtService
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly RedisSessionService _sessionService;

        public JwtService(IConfiguration config, RedisSessionService sessionService)
        {
            _config = config;
            _sessionService = sessionService;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key missing"))
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = TimeSpan.FromHours(6);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: creds
            );

            var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            // 💾 Save session in Redis
            await _sessionService.SetSessionAsync(jwtString, new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role
            }, expiry);

            return jwtString;
        }
    }
}
