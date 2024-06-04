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
    /// <summary>
    /// JWT Bearer authentication handler.
    /// </summary>
    public class JwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
    {
        /// <summary>
        /// Token validation parameters injected from Program.cs.
        /// </summary>
        private readonly TokenValidationParameters _tokenValidationParameters;

        /// <summary>
        /// Configuration for generating JWT tokens.
        /// </summary>
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Logger for JWT authentication.
        /// </summary>
        private static Logger JwtNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerHandler"/> class.
        /// </summary>
        /// <param name="options">Monitor for JWT bearer options.</param>
        /// <param name="logger">Logger factory.</param>
        /// <param name="encoder">URL encoder.</param>
        /// <param name="tokenValidationParameters">Token validation parameters.</param>
        /// <param name="jwtSettings">JWT settings.</param>
        public JwtBearerHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            TokenValidationParameters tokenValidationParameters,
            IOptions<JwtSettings> jwtSettings) :
            base(options, logger, encoder)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Handles the JWT authentication process.
        /// </summary>
        /// <returns>The result of the authentication.</returns>
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

            JwtNlogLogger.Info("Jwt Bearer Handler - Checking Authorizations");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken); // saves into validatedToken 

                var jwtToken = (JwtSecurityToken)validatedToken;

                var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;

                // we do not check the email in the database, we assume that passing all previous authorization checks is sufficient

                var claims = new[] { new Claim(ClaimTypes.Email, email) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                JwtNlogLogger.Info("Jwt Bearer Handler authenticated the provided Token");
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                JwtNlogLogger.Error(ex, "Jwt Authentication Handler raised an Exception");
                return AuthenticateResult.Fail("Invalid token");
            }
        }
    }
}
