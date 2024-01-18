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
                new Claim("CustomerID", customer.CustomerId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
<<<<<<< HEAD
                expires: DateTime.UtcNow.AddHours(24), // Scadenza del token
=======
                expires: DateTime.UtcNow.AddHours(1), // Scadenza del token
>>>>>>> b5b486735051255deb6dfbf9187df85d065010fd
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
