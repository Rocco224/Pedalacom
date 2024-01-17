namespace Pedalacom.Authentication
{
    public class Jwt
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public Jwt(string secretKey, string issuer, string audience)
        {
            SecretKey = secretKey;
            Issuer = issuer;
            Audience = audience;
        }
    }
}
