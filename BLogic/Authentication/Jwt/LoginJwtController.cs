using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAca5CodeFirst.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims; //pacchetto per il mio token

namespace WebAca5CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginJwtController : ControllerBase
    {
        private JwtSettings _jwtSettings;

        public LoginJwtController(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        public IActionResult GenerateToken(string username, string password)
        {        
            //da sistemare - inserire un metodo per l'autenticazione al db

            if(username.ToLower() == "claudio" &&  password.ToLower() == "orloff")
            {
                var token = generateJwtToken(username);
                return Ok(new { token });
            }
            else
            {
                return BadRequest();
            }
            
        }

        private string generateJwtToken(string username)
        {
            
            var secretKey = _jwtSettings.SecretKey;
            //ho bisogno di creare un gestore di token, un token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey); //faccio un encoding della chiave segreta           

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //qui dobbiamo fare una cosa simile al CLAIMS nella basic authentication
                //payload
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); //uso il descriptor per creare il Token
            string tokenString = tokenHandler.WriteToken(token); //Mi serve convertire il mio token in stringa
            return tokenString; //restituisco la stringa
        }
    }
}
