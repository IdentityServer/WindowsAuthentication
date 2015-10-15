using System.Threading.Tasks;

namespace IdentityServer.WindowsAuthentication.Services
{
    /// <summary>
    /// Service for providing custom claims
    /// </summary>
    public interface ICustomClaimsProvider
    {
        /// <summary>
        /// Claims transforms logic
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task TransformAsync(CustomClaimsProviderContext context);
    }
}