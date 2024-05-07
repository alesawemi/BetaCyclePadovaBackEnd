using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebAca5CodeFirst.Models;

namespace CodeFirst.BLogic.Authentication.Jwt
{
    public class JwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
    //public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly OldCustomersController _customersController;
        //private readonly UsersController _usersController;

        private readonly TokenValidationParameters _tokenValidationParameters; //ignetto i parametri definiti nel Program.cs

        private readonly JwtSettings _jwtSettings; // Configurazione per la generazione dei token JWT

        private static Logger JwtNlogLogger = LogManager.GetCurrentClassLogger();

        public JwtBearerHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, TokenValidationParameters tokenValidationParameters,
            IOptions<JwtSettings> jwtSettings) :
            base(options, logger, encoder)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                JwtNlogLogger.Warn("Jwt Bearer Handler - Missing Authorization header");
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            var authorizationHeader = Request.Headers["Authorization"].ToString();

            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                JwtNlogLogger.Warn("Jwt Bearer Handler - Invalid Authorization header");
                return AuthenticateResult.Fail("Invalid Authorization header");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            JwtNlogLogger.Info("Jwt Bearer Handler - Controllo Autorizzazioni");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken); //salva dentro validatedToken 

                var jwtToken = (JwtSecurityToken)validatedToken;

                var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;

                // non facciamo controlli di email nel db, assumiamo che sia sufficiente aver passato tutti i controlli precedenti sulle autorizzazioni

                var claims = new[] { new Claim(ClaimTypes.Email, email) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                
                JwtNlogLogger.Info("Jwt Bearer Handler ha autenticato il Token fornito");
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                JwtNlogLogger.Error(ex, "Jwt Authentication Handler ha sollevato un'Eccezione");
                return AuthenticateResult.Fail("Invalid token");
            }
        }
    }


}