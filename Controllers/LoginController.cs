using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Authentication;
using Pedalacom.Data;
using Pedalacom.Models.CustomerModel;
using Pedalacom.Models.UserModel;
using PedalacomLibrary;
using Pedalacom.Handler;
using NLog;

namespace Pedalacom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly AdventureWorks2019Context _context;
        private readonly Jwt _jwt;
        private CustomerHandler _customerHandler;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AdventureWorks2019Context context, Jwt jwt, ILogger<LoginController> logger) 
        { 
            _context = context;
            _jwt = jwt;
            _customerHandler = new CustomerHandler(context);
            _logger = logger;
            _logger.LogDebug("NLog in LoginController");
        }

        [BasicAutorizationAttributes]
        [HttpPost("Basic")]
        public IActionResult Auth(User user)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("JwtLogin")]
        public async Task<IActionResult> JwtLogin(User user)
        {
            try
            {
                Customer customer = _customerHandler.GetCustomer(user.Email);

                string passwordSha256 = Password.EncryptPassword(user.Password);
                KeyValuePair<string, string> encryptedSaltPassword = Password.DecryptSaltPassword(customer.PasswordSalt, passwordSha256);
                string userPasswordSaltHash = encryptedSaltPassword.Value;

                if (customer.PasswordHash != userPasswordSaltHash)
                    throw new Exception("Password errata");

                _logger.LogInformation("Login effettuato");

                return Ok(JwtManager.GenerateJwtToken(customer, _jwt.SecretKey, _jwt.Issuer, _jwt.Audience));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // Metodo per ricevere il sale dell'utente
        [HttpPost("GetSalt")]
        public async Task<string> GetUserSalt(User user)
        {
            try
            {
                _logger.LogInformation("Richiesta login utente");

                Customer customer = _customerHandler.GetCustomer(user.Email);

                return customer.PasswordSalt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // Verifica password Login
        [HttpPost("Jwt")]
        public async Task<IActionResult> GetToken(User user)
        {
            try
            {
                Customer customer = _customerHandler.GetCustomer(user.Email);

                if (customer.PasswordHash != user.Password)
                    throw new Exception("Password errata");

                _logger.LogInformation("Login effettuato");

                return Ok(new {Token = JwtManager.GenerateJwtToken(customer, _jwt.SecretKey, _jwt.Issuer, _jwt.Audience) });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        [HttpGet("GetSalt/{email}")]
        public async Task<ActionResult> GetCustomer(string email)
        {
            try
            {
                if (_context.Customers == null)
                {
                    return NotFound();
                }
                var customer = await _context.Customers
                    .Where(c => c.EmailAddress == email && c.CustomerId >= 29485)
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(new { Salt = customer.PasswordSalt });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }

        }
    }
}
