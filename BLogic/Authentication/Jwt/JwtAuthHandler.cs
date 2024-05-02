using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebAca5CodeFirst.Models;

namespace CodeFirst.BLogic.Authentication.Jwt
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
    //public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly OldCustomersController _customersController;
        //private readonly UsersController _usersController;

        private readonly TokenValidationParameters _tokenValidationParameters; //ignetto i parametri definiti nel Program.cs

        private readonly JwtSettings _jwtSettings; // Configurazione per la generazione dei token JWT

        public JwtAuthenticationHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, TokenValidationParameters tokenValidationParameters,
            IOptions<JwtSettings> jwtSettings) :
            base(options, logger, encoder)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings.Value;
        }

        //public JwtAuthenticationHandler(
        //    //IOptionsMonitor<AuthenticationSchemeOptions> options,
        //    ILoggerFactory logger,
        //    UrlEncoder encoder,
        //    //OldCustomersController customersController,
        //    //UsersController usersController,
        //    TokenValidationParameters tokenValidationParameters,
        //    IOptions<Jwtsettings> jwtSettings) :
        //    base(options, logger, encoder)
        //{

        //    //_customersController = customersController;
        //    //_usersController = usersController;
        //    _tokenValidationParameters = tokenValidationParameters;
        //    _jwtSettings = jwtSettings.Value;
        //}

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid Authorization header");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            //fino a qui controlla le autorizzazioni

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken); //salva dentro validatedToken 

                var jwtToken = (JwtSecurityToken)validatedToken;

                var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;

                //NON CONTROLLIAMO LA PASSWORD PER IL MOMENTO - ASSUMIAMO CHE IL TOKEN GENERATO APPARTENGA ALL'USER EFFETTIVO

                // Controlla se l'email è presente nel database degli utenti
                //var customerResult = await _customersController.GetCustomerByEmail(email);


                //if (customerResult == null)
                //{
                //    return AuthenticateResult.Fail("User not found");
                //}


                var claims = new[] { new Claim(ClaimTypes.Email, email) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Invalid token");
            }
        }
    }


}