using Microsoft.Extensions.Configuration;
using SistemaHorario.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaHorario.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //generate jwt con modelo AD
        public string GenerateJWT(string IdUser,string userName, string group, string? rol) 
        {
            string typerol = string.Empty;


            var userClaims = new[] //las claims son datos del cual se va a generar el token
            {
                new Claim(ClaimTypes.NameIdentifier, IdUser),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, rol.Trim()),
                new Claim("Group", group),//claim personalizado mas semantico
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único del token
            };

            //configuracion de firma
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));//crea la la clave de firma usando la clave secreta
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);//usa el algoritmo HmacSha256Signature para firmar el token

            //create detail token
            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"] ?? "60")
                    ),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);//convierte el token a string que se envia al cliente
        }

        public bool TokenValidate(string token)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //vefica que la firma sea correcta
                ValidateIssuer = true, //valida quien emitio el token
                ValidateAudience = true, //valida pra quien es el token
                ValidateLifetime = true,//verifica que no haya expirado
                ValidIssuer = _configuration["Jwt:Issuer"], //quien emite el token (la api)
                ValidAudience = _configuration["Jwt:Audience"],//para quien es el token(cliente)
                ClockSkew = TimeSpan.FromMinutes(1),//1 minuto de tolerancia
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!)) //clave secreta para verificar la firma
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true; //si es token valido
            }
            catch (SecurityTokenException)
            {
                return false; //token invalido (expirado, firma incorrecta, formato malo, etc.)
            }
        }
    }
}
