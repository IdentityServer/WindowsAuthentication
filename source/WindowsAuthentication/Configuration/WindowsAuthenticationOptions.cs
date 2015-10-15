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
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.WindowsAuthentication.Configuration
{
    /// <summary>
    /// Configuration options
    /// </summary>
    public class WindowsAuthenticationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAuthenticationOptions"/> class.
        /// </summary>
        public WindowsAuthenticationOptions()
        {
            IssuerUri = "urn:windowsauthentication";
            IssuerName = "Windows Authentication";
            IdpRealm = "urn:idsrv3";

            EnableWsFederationEndpoint = true;
            EnableWsFederationMetadata = true;
            EnableOAuth2Endpoint = true;

            SubjectType = Configuration.SubjectType.Sid;
            EmitWindowsAccountNameAsName = true;
            EmitGroups = false;
            CustomClaimsProvider = new DefaultCustomClaimsProvider();
            
            TokenLifeTime = 60;
        }

        /// <summary>
        /// Gets or sets the issuer URI (defaults to urn:windowsauthentication)
        /// </summary>
        /// <value>
        /// The issuer URI.
        /// </value>
        public string IssuerUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the issuer name for the metadata document (defaults to 'Windows Authentication').
        /// </summary>
        /// <value>
        /// The name of the issuer.
        /// </value>
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the signing certificate for the identity token.
        /// </summary>
        /// <value>
        /// The signing certificate.
        /// </value>
        public X509Certificate2 SigningCertificate { get; set; }

        /// <summary>
        /// Gets or sets the realm name of identityserver (defaults to urn:idsrv3).
        /// </summary>
        /// <value>
        /// The idp realm.
        /// </value>
        public string IdpRealm { get; set; }

        /// <summary>
        /// Gets or sets the identityserver reply URL.
        /// </summary>
        /// <value>
        /// The idp reply URL.
        /// </value>
        public string IdpReplyUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the WS-Federation endpoint is enabled. Defaults to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if the WS-Federation endpoint is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableWsFederationEndpoint { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to enable the OAuth2 endpoint. Defaults to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if the OAuth2 endpoint is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOAuth2Endpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the metadata endpoint is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if you want to enable the metadata endoint; otherwise, <c>false</c>.
        /// </value>
        public bool EnableWsFederationMetadata { get; set; }

        /// <summary>
        /// Gets or sets the type of the subject. Either the Windows account name or the SID can be used.
        /// </summary>
        /// <value>
        /// The type of the subject.
        /// </value>
        public SubjectType SubjectType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Windows groups of the user will be inclided in the token (default to false).
        /// </summary>
        /// <value>
        ///   <c>true</c> if groups are emitted; otherwise, <c>false</c>.
        /// </value>
        public bool EmitGroups { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to emit the Windows account name as a name claim.
        /// </summary>
        /// <value>
        /// <c>true</c> if you want to emit the Windows account name as name claim; otherwise, <c>false</c>.
        /// </value>
        public bool EmitWindowsAccountNameAsName { get; set; }

        /// <summary>
        /// Gets or sets custom claims transformation logic.
        /// </summary>
        public ICustomClaimsProvider CustomClaimsProvider { get; set; }

        /// <summary>
        /// Gets or sets the public origin for the server (e.g. "https://yourserver:1234").
        /// </summary>
        /// <value>
        /// The name of the public origin.
        /// </value>
        public string PublicOrigin { get; set; }

        /// <summary>
        /// Gets or sets the token life time (defaults to 60 mins).
        /// </summary>
        /// <value>
        /// The token life time.
        /// </value>
        public int TokenLifeTime { get; set; }
    }
}