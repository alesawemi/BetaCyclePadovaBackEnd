using System.Security.Principal;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    /// <summary>
    /// Represents an authenticated user.
    /// </summary>
    public class AuthenticatedUser : IIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedUser"/> class.
        /// </summary>
        /// <param name="authType">The type of authentication used.</param>
        /// <param name="isAuthenticated">Indicates whether the user is authenticated.</param>
        /// <param name="name">The name of the user.</param>
        public AuthenticatedUser(string authType, bool isAuthenticated, string name)
        {
            AuthenticationType = authType;
            IsAuthenticated = isAuthenticated;
            Name = name;
        }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        public string? AuthenticationType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string? Name { get; set; }
    }
}
