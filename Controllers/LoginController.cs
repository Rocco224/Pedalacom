using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pedalacom.Authentication;
using Pedalacom.Models.UserModel;

namespace Pedalacom.Controllers
{
    [BasicAutorizationAttributes]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost]
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
    }
}
