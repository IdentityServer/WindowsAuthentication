using IdentityModel;
using IdentityServer.WindowsAuthentication.Configuration;
using Microsoft.Owin;
using Owin;
using System.Linq;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAuthenticationService(new WindowsAuthenticationOptions
            {
                IdpRealm = "urn:idp",
                IdpReplyUrl = "https://localhost:44333/core/was",

                SigningCertificate = X509.LocalMachine.My.SubjectDistinguishedName.Find("CN=sts").First()
            });
        }
    }
}
