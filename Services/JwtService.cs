using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginApp.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        public JwtService()
        {
            Env.Load();
            string jwtKey = Env.GetString("JWT_KEY");
            string jwtIssuer = Env.GetString("JWT_ISSUER");
            _secretKey = jwtKey;
            _issuer = jwtIssuer;
        }
        public string GenerateSecurityToken(ClaimsPrincipal claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims.Claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);
            return validatedToken != null;
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (ValidateToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.Claims;
            }            
            return null;
        }
    }
}
