using Microsoft.AspNetCore.Authorization;

namespace Pedalacom.Authentication
{
    public class BasicAutorizationAttributes:AuthorizeAttribute
    {
        public BasicAutorizationAttributes() {
            Policy = "BasicAuthentication";
        }
    }
}
