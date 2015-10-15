using System.Security.Claims;
using System.Security.Principal;

namespace IdentityServer.WindowsAuthentication.Services
{
    /// <summary>
    /// Context describing incoming and outgoing identities.
    /// </summary>
    public class CustomClaimsProviderContext
    {
        /// <summary>
        /// Incoming Windows user.
        /// </summary>
        public WindowsPrincipal WindowsPrincipal { get; set; }

        /// <summary>
        /// Claims identity containing all claims for outgoing token.
        /// </summary>
        public ClaimsIdentity OutgoingSubject { get; set; }
    }
}
