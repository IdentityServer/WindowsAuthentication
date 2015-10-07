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
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;
using IdentityModel.Tokens;

namespace IdentityServer.WindowsAuthentication
{
    internal static class SubjectGenerator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public static ClaimsIdentity Create(WindowsPrincipal principal, WindowsAuthenticationOptions options)
        {
            var claims = new List<Claim>();
            string sub = null;

            if (options.SubjectType == SubjectType.WindowsAccountName)
            {
                Logger.Debug("Using WindowsAccountName as subject");

                sub = principal.Identity.Name;
            }
            else if (options.SubjectType == SubjectType.Sid)
            {
                Logger.Debug("Using primary SID as subject");

                sub = principal.FindFirst(ClaimTypes.PrimarySid).Value;
            }

            claims.Add(new Claim("sub", sub));

            if (options.EmitWindowsAccountNameAsName)
            {
                Logger.Debug("Emitting WindowsAccountName as name claim");

                claims.Add(new Claim("name", principal.Identity.Name));
            }

            if (options.EmitGroups)
            {
                Logger.Debug("Using Windows groups as role claims");

                claims.AddRange(CreateGroupClaims(principal));
            }

            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Windows));
            claims.Add(AuthenticationInstantClaim.Now);

            return new ClaimsIdentity(claims, "Windows");
        }

        private static IEnumerable<Claim> CreateGroupClaims(WindowsPrincipal principal)
        {
            var groupSidClaims = principal.FindAll(ClaimTypes.GroupSid);

            var sids = new IdentityReferenceCollection();
            foreach (var sidClaim in groupSidClaims)
            {
                sids.Add(new SecurityIdentifier(sidClaim.Value));
            }

            var groupNames = sids.Translate(typeof(NTAccount));

            var groupNameClaims = new List<Claim>(
                from n in groupNames select new Claim("role", n.Value));

            return groupNameClaims;
        }
    }
}