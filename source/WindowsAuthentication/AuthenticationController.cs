/*
 * Copyright 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer.WindowsAuthentication.Configuration;
using IdentityServer.WindowsAuthentication.Logging;
using System.IdentityModel.Services;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace IdentityServer.WindowsAuthentication
{
    [RoutePrefix("")]
    internal class AuthenticationController : ApiController
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        WindowsAuthenticationOptions _options;

        public AuthenticationController(WindowsAuthenticationOptions options)
        {
            _options = options;
        }
            
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            WSFederationMessage message;
            if (WSFederationMessage.TryCreateFromUri(Request.RequestUri, out message))
            {
                Logger.Info("Start WS-Federation request");

                var signin = message as SignInRequestMessage;
                if (signin != null)
                {
                    if (User == null || !User.Identity.IsAuthenticated)
                    {
                        Logger.Info("User is anonymous. Triggering authentication");
                        return Unauthorized();
                    }

                    var windowsUser = User as WindowsPrincipal;

                    if (windowsUser == null)
                    {
                        var windowsUserError = "Invalid authentication method. Only Windows authentication is supported.";

                        Logger.Error(windowsUserError);
                        return BadRequest(windowsUserError);
                    }

                    Logger.Info("Sign-in request");
                    return await ProcessSignInAsync(signin, windowsUser);
                }

                var signout = message as SignOutRequestMessage;
                if (signout != null)
                {
                    Logger.Info("Sign-in request");

                    // no support for signout
                    return Ok();
                }
            }

            if (_options.EnableWsFederationMetadata)
            {
                Logger.Info("Start WS-Federation metadata request");
                return ProcessMetadataRequest();
            }

            var requestError = "Invalid WS-Federation request";

            Logger.Error(requestError);
            return BadRequest(requestError);
        }

        private IHttpActionResult ProcessMetadataRequest()
        {
            var url = Request.GetOwinContext().Environment.GetServerBaseUrl();

            var generator = new MetadataResponseGenerator(_options);
            var entity = generator.Generate(url);

            return new MetadataResult(entity);
        }

        private async Task<IHttpActionResult> ProcessSignInAsync(SignInRequestMessage request, WindowsPrincipal windowsUser)
        {
            var generator = new SignInResponseGenerator(_options);
            var response = await generator.GenerateAsync(request, windowsUser);

            return new SignInResult(response);
        }
    }
}