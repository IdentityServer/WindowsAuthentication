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

using IdentityServer.WindowsAuthentication.Services;
using Microsoft.Owin.Security.OAuth;
using System.Security.Principal;
using System.Threading.Tasks;

namespace IdentityServer.WindowsAuthentication.Configuration
{
    internal class WindowsAuthenticationTokenProvider : OAuthAuthorizationServerProvider
    {
        private readonly WindowsAuthenticationOptions _options;

        public WindowsAuthenticationTokenProvider(WindowsAuthenticationOptions options)
        {
            _options = options;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

            return Task.FromResult(0);
        }

        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            var windowsPrincipal = context.OwinContext.Authentication.User as WindowsPrincipal;

            if (windowsPrincipal == null)
            {
                context.SetError("User is not a Windows user");
                return;
            }

            var subject = SubjectGenerator.Create(windowsPrincipal, _options);
            var transformationContext = new CustomClaimsProviderContext
            {
                WindowsPrincipal = windowsPrincipal,
                OutgoingSubject = subject
            };
            await _options.CustomClaimsProvider.TransformAsync(transformationContext);
            context.Validated(transformationContext.OutgoingSubject);
        }
    }
}