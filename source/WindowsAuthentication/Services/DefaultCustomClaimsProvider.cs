using System.Threading.Tasks;

namespace IdentityServer.WindowsAuthentication.Services
{
    /// <summary>
    /// Default custom claims provider implementation (nop)
    /// </summary>
    public class DefaultCustomClaimsProvider : ICustomClaimsProvider
    {
        /// <summary>
        /// Claims transforms logic
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task TransformAsync(CustomClaimsProviderContext context)
        {
            return Task.FromResult(0);
        }
    }
}