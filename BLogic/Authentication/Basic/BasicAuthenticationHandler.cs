using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using NLog;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    /// <summary>
    /// Basic authentication handler.
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">Options monitor for the authentication scheme.</param>
        /// <param name="logger">Logger factory.</param>
        /// <param name="encoder">URL encoder.</param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder
            ) :
            base(options, logger, encoder)
        {
        }

        /// <summary>
        /// Logger for basic authentication.
        /// </summary>
        private static Logger BasicNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes basic authentication.
        /// </summary>
        /// <returns>The result of the authentication.</returns>
        /// <remarks>
        /// <para>Each message consists of one or more headers.</para>
        /// <para>This is the engine that allows us to perform basic authentication.</para>
        /// <para>Let me warn you, caller, that I need basic authentication.</para>
        /// </remarks>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Each message consists of one or more headers.
            // Let me warn you, caller, that I need basic authentication.
            Response.Headers.Add("WWW-Authenticate", "Basic");

            // From the header in the call, look for an "Authorization".
            if (!Request.Headers.ContainsKey("Authorization")) //--TO BE RESOLVED IN THE FUTURE
            {
                BasicNlogLogger.Warn("Basic Authentication Handler - Missing Authorization");
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization")); // if not found - exit immediately
            } // else continue

            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authorHeaderRegEx = new Regex("Basic (.*)"); // in the string you provided, it should start with Basic 'space' something

            if (!authorHeaderRegEx.IsMatch(authorizationHeader)) // if the regex does not match what was found
            {
                BasicNlogLogger.Warn("Basic Authentication Handler - Invalid Authorization");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization")); // exit
            }

            // BASIC AUTHENTICATION ENCODING-64 - of the string that matches (.*)
            var auth64 = Encoding.UTF8.GetString(
                Convert.FromBase64String(authorHeaderRegEx
                .Replace(authorizationHeader, "$1")));

            // If the encoding was done correctly there should be "username:password", but if I have two elements
            // it's an array
            var authArraySplit = auth64.Split(Convert.ToChar(":"), 2); // array made of two elements,  [0]: username, [1]: password
            var authUser = authArraySplit[0];
            // it is not certain that the password was taken, so I do an extra check.
            var authPassword = authArraySplit.Length > 1 ? authArraySplit[1] : throw new Exception("Password NOT present");

            // If all goes well, I have username and password - here begins the behind-the-scenes work to be implemented
            if ((string.IsNullOrEmpty(authUser)) || (string.IsNullOrEmpty(authPassword.Trim()))) // Trim removes insignificant spaces
            {
                BasicNlogLogger.Warn("Basic Authentication Handler - Invalid Username and/or Password");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username and/or Password"));
            }
            else
            {
                // Get the email from basic authorization
                var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authArraySplit[0].ToString()); // you need to define new AuthenticatedUser class

                // framework defines entity that will allow the user to enter
                var claimsMain = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

                BasicNlogLogger.Info("Basic Authentication Handler authorized the operation");
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsMain, Scheme.Name)));
            }
        }
    }
}
