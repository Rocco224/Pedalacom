using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Data;
using Pedalacom.Models.OrderModel;

namespace Pedalacom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;
        private readonly ILogger<SalesOrderHeadersController> _logger;

        public SalesOrderHeadersController(AdventureWorks2019Context context, ILogger<SalesOrderHeadersController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug("NLog in SalesOrderHeadersController");
        }

        // GET: api/SalesOrderHeaders
        [Authorize(Roles = "Admin, Guest")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
            try
            {
                _logger.LogInformation("Richiesta ordini");

                if (_context.SalesOrderHeaders == null)
                    return NotFound();

                // Admin
                if (User.IsInRole("Admin"))
                    return await _context.SalesOrderHeaders.ToListAsync();

                // Guest
                var customerID = int.Parse(User.FindFirst("CustomerID").Value);

                return await _context.SalesOrderHeaders.Where(c => c.CustomerId == customerID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // GET: api/SalesOrderHeaders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            try
            {
                _logger.LogInformation("Richiesta ordine singolo");

                if (_context.SalesOrderHeaders == null)
                {
                    return NotFound();
                }
                var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);

                if (salesOrderHeader == null)
                {
                    return NotFound();
                }

                return salesOrderHeader;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // PUT: api/SalesOrderHeaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            try
            {
                _logger.LogInformation("Modifica ordine");

                if (id != salesOrderHeader.SalesOrderId)
                {
                    return BadRequest();
                }

                _context.Entry(salesOrderHeader).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderHeaderExists(id))
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

        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            try
            {
                _logger.LogInformation("Aggiunta ordine");

                if (_context.SalesOrderHeaders == null)
                {
                    return Problem("Entity set 'AdventureWorks2019Context.SalesOrderHeaders'  is null.");
                }
                _context.SalesOrderHeaders.Add(salesOrderHeader);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        // DELETE: api/SalesOrderHeaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
            try
            {
                if (_context.SalesOrderHeaders == null)
                {
                    return NotFound();
                }
                var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
                if (salesOrderHeader == null)
                {
                    return NotFound();
                }

                _context.SalesOrderHeaders.Remove(salesOrderHeader);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return (_context.SalesOrderHeaders?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
