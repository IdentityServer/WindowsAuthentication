using IdentityModel;
using IdentityServer.WindowsAuthentication.Configuration;
using Owin;
using System.Linq;

namespace SelfHost
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAuthentication();

            app.UseWindowsAuthenticationService(new WindowsAuthenticationOptions
                {
                    SigningCertificate = X509.LocalMachine.My.SubjectDistinguishedName.Find("CN=sts").First(),
                    IdpReplyUrl = "https://localhost:44333/core/was"
                });
        }
    }
}
