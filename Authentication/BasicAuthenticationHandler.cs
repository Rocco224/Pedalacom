using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using PedalacomLibrary;
using System.Configuration;
using System.Security.Cryptography;
using Pedalacom.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedalacom.Models;
using Pedalacom.Models.CustomerModel;

namespace Pedalacom.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AdventureWorks2019Context _context;
        public Customer _authUser = new ();
        public KeyValuePair<string,string> _saltedPassword;

        public BasicAuthenticationHandler(
            AdventureWorks2019Context context,
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock) 
        {
            _context = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione mancante: Impossibile accedere al servizio"));
            }
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authorizationRegEx = new Regex(@"Basic (.*)");
            if (!authorizationRegEx.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione non valida: Impossibile accedere al servizio"));
            }

            var authorizationBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationRegEx.Replace(authorizationHeader, "$1")));
            var authorizationSplit = authorizationBase64.Split(":", 2);
            if(authorizationSplit.Length!=2) {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione non valida: Impossibile accedere al servizio"));

            }
            string email = authorizationSplit[0];
            string password = authorizationSplit[1];

            // Query DB
            var user = new SqlParameter("email", email);

            var customers = _context.Customers
                .FromSql($"EXEC [dbo].[sp_CheckEmail] @email={user}")
                .ToList();

            if(customers == null )
                return Task.FromResult(AuthenticateResult.Fail("Utente non trovato"));

            customers.ForEach((customer) => {
                _authUser = new Customer(
                    customer.CustomerId,
                    customer.NameStyle,
                    customer.Title,
                    customer.FirstName,
                    customer.MiddleName,
                    customer.LastName,
                    customer.Suffix,
                    customer.CompanyName,
                    customer.SalesPerson,
                    customer.EmailAddress,
                    customer.Phone,
                    customer.PasswordHash,
                    customer.PasswordSalt,
                    customer.Rowguid,
                    customer.ModifiedDate,
                    customer.Valid,
                    customer.CustomerAddresses,
                    customer.SalesOrderHeaders
                    );
                });

            string ash256Password = Password.EncryptPassword(password);
            _saltedPassword = Password.DecryptSaltPassword(_authUser.PasswordSalt, ash256Password);

            if (_authUser.PasswordHash != _saltedPassword.Value)
                return Task.FromResult(AuthenticateResult.Fail("Password errata"));

            var authenticationUser = new AuthenticationUser(_authUser.FirstName, "BasicAuthentication",true);
            var claims = new ClaimsPrincipal(new ClaimsIdentity(authenticationUser));

            Console.WriteLine("Login effettuato con successo");

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claims, "BasicAuthentication")));
        }
    }
}
