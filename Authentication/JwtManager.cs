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
        // Creazione del token
        public static string GenerateJwtToken(Customer customer, string secretKey, string issuer, string audience)
        {
            Console.WriteLine(customer.FirstName);
            Console.WriteLine(customer.Role);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Cosa inserire nel token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, customer.FirstName),
                new Claim(ClaimTypes.Role, customer.Role),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1), // Scadenza del token
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
