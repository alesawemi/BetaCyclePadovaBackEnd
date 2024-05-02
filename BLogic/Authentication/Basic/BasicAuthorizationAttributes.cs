using Microsoft.AspNetCore.Authorization;

namespace BetaCycle_Padova.BLogic.Authentication.Basic
{
    public class BasicAuthorizationAttributes : AuthorizeAttribute
    {
        public BasicAuthorizationAttributes() { Policy = "BasicAuthorization"; }
    }
}
