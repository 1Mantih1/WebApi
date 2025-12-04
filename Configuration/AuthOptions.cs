using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi.Configuration
{
    public class AuthOptions
    {
        public const string Issuer = "WebApp";               
        public const string Audience = "MyAuthClient";      
        private const string Key = "mysupersecret_secretsecretkey!123"; 

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
            => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
