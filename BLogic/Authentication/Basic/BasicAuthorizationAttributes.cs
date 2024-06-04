using Microsoft.AspNetCore.Authorization;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    /// <summary>
    /// Attribute for basic authorization.
    /// </summary>
    public class BasicAuthorizationAttributes : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthorizationAttributes"/> class.
        /// </summary>
        public BasicAuthorizationAttributes()
        {
            Policy = "BasicAuthorization";
        }
    }
}
