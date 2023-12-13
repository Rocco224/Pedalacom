using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public RegisterController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // POST: RegisterController/Create
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'AdventureWorks2019Context.Customers'  is null.");
            }

            KeyValuePair<string,string> encryptedSaltPassword = Password.EncryptSaltPassword(Password.EncryptPassword(customer.PasswordHash));
            customer.PasswordHash = encryptedSaltPassword.Value;
            customer.PasswordSalt = encryptedSaltPassword.Key;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }
    }
}
