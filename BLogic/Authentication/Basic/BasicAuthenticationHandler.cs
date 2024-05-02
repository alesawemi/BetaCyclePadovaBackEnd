using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder
            ) :
            base(options, logger, encoder)
        {
        }

        //Questo è il motore che ci permette di fare l'autenticazione basica.
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Ogni messaggio è composto da uno o più headers (intestazioni del messaggio)
            //Ti avviso tu chiamante che ho bisogno di un'autenticazione basic
            Response.Headers.Add("WWW-Authenticate", "Basic");

            //Dall'intestazione che hai nella chiamata cercami un "Authorization"
            if (!Request.Headers.ContainsKey("Authorization")) //--DA RISOLVERE IN FUTURO
            {              
               return Task.FromResult(AuthenticateResult.Fail("Missing Authorization"));//se non ti trovo - esci fuori subito
                                                                                        
            } //else vai avanti

            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authorHeaderRegEx = new Regex("Basic (.*)"); //nella stringa che mi hai scaricato, dovrebbe iniziare con Basic 'spazio' qualcosa
           

            if (!authorHeaderRegEx.IsMatch(authorizationHeader)) //se la regex non corrisponde con quanto trovato
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization")); //esce fuori
            }

            //BASIC AUTHENTICATION ENCODING-64 - della stringa che corrisponde a (.*)
            var auth64 = Encoding.UTF8.GetString(
                Convert.FromBase64String(authorHeaderRegEx
                .Replace(authorizationHeader, "$1")));


            //Se l'encoding è stato eseguito correttamente ci dovrebbe essere "nomeutente:password", ma se io ho due elementi
            //è un array
            var authArraySplit = auth64.Split(Convert.ToChar(":"), 2); //array fatto da due elementi,  [0]: username, [1]: password
            var authUser = authArraySplit[0];
            //non è detto che la password me l'abbia presa, quindi faccio un controllo in più.
            var authPassword = authArraySplit.Length > 1 ? authArraySplit[1] : throw new Exception("Password NON presente");

            //Se va tutto bene ho username e password - qui inizia il lavoro dietro le quinte da implementare
            if ((string.IsNullOrEmpty(authUser)) || (string.IsNullOrEmpty(authPassword.Trim()))) //Trim butta via gli spazi non significativi
            {
                return Task.FromResult(AuthenticateResult.Fail("Username e/o Password NON validi"));
            }
            else
            {
                // Ottieni l'email dall'autorizzazione di base
                var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authArraySplit[0].ToString()); //devi definire nuova classe AuthenticatedUser

                //framework definisce entità che permetterà all'utente di entrare
                var claimsMain = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsMain, Scheme.Name)));

            }           
        }
    }
}
