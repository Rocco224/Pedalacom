using Microsoft.IdentityModel.Tokens;
using Pedalacom.Models.CustomerModel;
using Pedalacom.Models.UserModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pedalacom.Authentication
{
    public class JwtManager
    {
        private string _secretKey {  get; set; }
        private string _issuer {  get; set; }
        private string _audience { get; set; }

        public JwtManager(string secretKey, string issuer, string audience) 
        { 
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateJwtToken(Customer customer)
        {
            Console.WriteLine(customer.FirstName);
            Console.WriteLine(customer.Role);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, customer.FirstName),
                new Claim(ClaimTypes.Role, customer.Role),
                // Aggiungi altri claim necessari
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1), // Scadenza del token
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateJwtToken(string token, string secretKey, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
