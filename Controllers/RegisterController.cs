using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using Pedalacom.Data;
using Pedalacom.Models.CustomerModel;
using PedalacomLibrary;

namespace Pedalacom.Controllers
{        
    [Route("[controller]")]    
    [ApiController]
    public class RegisterController : Controller
    {
        private readonly AdventureWorks2019Context _context;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(AdventureWorks2019Context context, ILogger<RegisterController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug("NLog in RegisterController");
        }

        // POST: RegisterController/Create
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                _logger.LogInformation("Richiesta registrazione utente");

                if (_context.Customers == null)
                {
                    return Problem("Entity set 'AdventureWorks2019Context.Customers' is null.");
                }

                //verifica email univoca
                var emailParameter = new SqlParameter("email", customer.EmailAddress);

                var user = _context.Customers
                    .FromSql($"EXEC [dbo].[sp_CheckEmail] @email={emailParameter}")
                    .AsEnumerable()
                    .SingleOrDefault();

                if (user != null)
                    return Problem("Email esistente");
                
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Utente registrato");

                return Ok("Utente registrato");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }
    }
}
