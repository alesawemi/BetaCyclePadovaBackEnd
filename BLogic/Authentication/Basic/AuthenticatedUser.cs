using System.Security.Principal;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    /// <summary>
    /// Rappresenta un utente autenticato.
    /// </summary>
    public class AuthenticatedUser : IIdentity
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="AuthenticatedUser"/>.
        /// </summary>
        /// <param name="authType">Il tipo di autenticazione utilizzato.</param>
        /// <param name="isAuthenticated">Indica se l'utente è autenticato.</param>
        /// <param name="name">Il nome dell'utente.</param>
        public AuthenticatedUser(string authType, bool isAuthenticated, string name)
        {
            AuthenticationType = authType;
            IsAuthenticated = isAuthenticated;
            Name = name;
        }

        /// <summary>
        /// Ottiene il tipo di autenticazione utilizzato.
        /// </summary>
        public string? AuthenticationType { get; set; }

        /// <summary>
        /// Ottiene un valore che indica se l'utente è autenticato.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Ottiene il nome dell'utente.
        /// </summary>
        public string? Name { get; set; }
    }
}
