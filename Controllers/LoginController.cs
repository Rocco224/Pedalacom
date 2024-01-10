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

namespace Pedalacom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly AdventureWorks2019Context _context;
        private readonly JwtManager _jwtManager;
        private CustomerHandler _customerHandler;

        public LoginController(AdventureWorks2019Context context, JwtManager jwtManager) 
        { 
            _context = context;
            _jwtManager = jwtManager;
            _customerHandler = new CustomerHandler(context);
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

        [HttpPost("GetSalt")]
        public async Task<string> GetUserSalt(User user)
        {
            try
            {
                Customer customer = _customerHandler.GetCustomer(user.Email);

                return customer.PasswordSalt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Jwt")]
        public async Task<IActionResult> GetToken(User user)
        {
            try
            {     
                Customer customer = _customerHandler.GetCustomer(user.Email);

                if (customer.PasswordHash != user.Password)
                    throw new Exception("Password errata");

                return Ok(_jwtManager.GenerateJwtToken(customer));
            }

            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Si è verificato un errore durante l'elaborazione della richiesta.",
                    errorDetails = ex.Message,
                    statusCode = StatusCodes.Status400BadRequest,
                    traceId = Guid.NewGuid()
                });
            }
        }
    }
}
