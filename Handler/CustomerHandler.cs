using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Authentication;
using Pedalacom.Data;
using Pedalacom.Models.CustomerModel;

namespace Pedalacom.Handler
{
    public class CustomerHandler
    {
        private AdventureWorks2019Context _context;

        public CustomerHandler(AdventureWorks2019Context context)
        {
            _context = context;
        }

        public Customer GetCustomer(string userEmail)
        {
            try
            {
                var emailParameter = new SqlParameter("email", userEmail);

                var customer = _context.Customers
                    .FromSql($"EXEC [dbo].[sp_CheckEmail] @email={emailParameter}")
                    .AsEnumerable()
                    .SingleOrDefault();

                if (customer == null)
                    throw new Exception("Email Errata");

                return customer;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
