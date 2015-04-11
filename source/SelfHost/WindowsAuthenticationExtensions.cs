using Owin;
using System.Net;

namespace SelfHost
{
    public static class WindowsAuthenticationExtensions
    {
        public static IAppBuilder UseWindowsAuthentication(this IAppBuilder app)
        {
            object value;
            if (app.Properties.TryGetValue("System.Net.HttpListener", out value))
            {
                var listener = value as HttpListener;
                if (listener != null)
                {
                    listener.AuthenticationSchemes = 
                        AuthenticationSchemes.Anonymous |
                        AuthenticationSchemes.Negotiate;
                }
            }

            return app;
        }
    }
}