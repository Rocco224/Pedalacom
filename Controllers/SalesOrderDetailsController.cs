using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Data;
using Pedalacom.Models.OrderModel;

namespace Pedalacom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderDetailsController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;

        public SalesOrderDetailsController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // GET: api/SalesOrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderDetail>>> GetSalesOrderDetails()
        {
          if (_context.SalesOrderDetails == null)
          {
              return NotFound();
          }
            return await _context.SalesOrderDetails.ToListAsync();
        }

        // GET: api/SalesOrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderDetail>> GetSalesOrderDetail(int id)
        {
          if (_context.SalesOrderDetails == null)
          {
              return NotFound();
          }
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);

            if (salesOrderDetail == null)
            {
                return NotFound();
            }

            return salesOrderDetail;
        }

        // PUT: api/SalesOrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderDetail(int id, SalesOrderDetail salesOrderDetail)
        {
            if (id != salesOrderDetail.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderDetailExists(id))
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

        // POST: api/SalesOrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesOrderDetail>> PostSalesOrderDetail(SalesOrderDetail salesOrderDetail)
        {
          if (_context.SalesOrderDetails == null)
          {
              return Problem("Entity set 'AdventureWorks2019Context.SalesOrderDetails'  is null.");
          }
            _context.SalesOrderDetails.Add(salesOrderDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SalesOrderDetailExists(salesOrderDetail.SalesOrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSalesOrderDetail", new { id = salesOrderDetail.SalesOrderId }, salesOrderDetail);
        }

        // DELETE: api/SalesOrderDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderDetail(int id)
        {
            if (_context.SalesOrderDetails == null)
            {
                return NotFound();
            }
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return NotFound();
            }

            _context.SalesOrderDetails.Remove(salesOrderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalesOrderDetailExists(int id)
        {
            return (_context.SalesOrderDetails?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
