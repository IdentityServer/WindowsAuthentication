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

using Microsoft.Owin.Security;
using System;
using System.IdentityModel.Tokens;

namespace IdentityServer.WindowsAuthentication.Configuration
{
    internal class JwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly WindowsAuthenticationOptions _options;

        public JwtFormat(WindowsAuthenticationOptions options)
        {
            _options = options;
        }

        public string Protect(AuthenticationTicket data)
        {
            var token = new JwtSecurityToken(
                _options.IssuerUri,
                _options.IdpRealm,
                data.Identity.Claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(_options.TokenLifeTime),
                new X509SigningCredentials(_options.SigningCertificate));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
