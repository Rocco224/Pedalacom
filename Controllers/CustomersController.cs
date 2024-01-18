using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Authentication;
using Pedalacom.Data;
using Pedalacom.Models.CustomerModel;

namespace Pedalacom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(AdventureWorks2019Context context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug("NLog in CustomerController");
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                _logger.LogInformation("Richiesta clienti");

                if (_context.Customers == null)
                {
                    return NotFound();
                }

                return await _context.Customers.Include(emp => emp.CustomerAddresses)
                                               .Include(emp => emp.SalesOrderHeaders)
                                               .Where(emp => emp.CustomerAddresses != null && emp.SalesOrderHeaders != null)
                                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return Problem(ex.Message);
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Richiesta cliente singolo");

                if (_context.Customers == null)
                {
                    return NotFound();
                }
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            try
            {
                _logger.LogInformation("Modifica cliente");

                if (id != customer.CustomerId)
                {
                    return BadRequest();
                }

                _context.Entry(customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Elimina cliente");

                if (_context.Customers == null)
                {
                    return NotFound();
                }
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
