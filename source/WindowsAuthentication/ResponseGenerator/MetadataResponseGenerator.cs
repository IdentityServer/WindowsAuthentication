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
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using IdentityModel.Constants;

namespace IdentityServer.WindowsAuthentication
{
    internal class MetadataResponseGenerator
    {
        private readonly WindowsAuthenticationOptions _options;

        public MetadataResponseGenerator(WindowsAuthenticationOptions options)
        {
            _options = options;
        }

        public EntityDescriptor Generate(string wsfedEndpoint)
        {
            var tokenServiceDescriptor = GetTokenServiceDescriptor(wsfedEndpoint);

            var id = new EntityId(_options.IssuerUri);
            var entity = new EntityDescriptor(id);
            entity.SigningCredentials = new X509SigningCredentials(_options.SigningCertificate);
            entity.RoleDescriptors.Add(tokenServiceDescriptor);

            return entity;
        }

        private SecurityTokenServiceDescriptor GetTokenServiceDescriptor(string wsfedEndpoint)
        {
            var tokenService = new SecurityTokenServiceDescriptor();
            tokenService.ServiceDescription = _options.IssuerName;
            tokenService.Keys.Add(GetSigningKeyDescriptor());

            tokenService.PassiveRequestorEndpoints.Add(new EndpointReference(wsfedEndpoint));
            tokenService.SecurityTokenServiceEndpoints.Add(new EndpointReference(wsfedEndpoint));

            tokenService.TokenTypesOffered.Add(new Uri(TokenTypes.OasisWssSaml11TokenProfile11));
            tokenService.TokenTypesOffered.Add(new Uri(TokenTypes.OasisWssSaml2TokenProfile11));
            tokenService.TokenTypesOffered.Add(new Uri(TokenTypes.JsonWebToken));

            tokenService.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            return tokenService;
        }

        private KeyDescriptor GetSigningKeyDescriptor()
        {
            var certificate = _options.SigningCertificate;

            var clause = new X509SecurityToken(certificate).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>();
            var key = new KeyDescriptor(new SecurityKeyIdentifier(clause));
            key.Use = KeyType.Signing;

            return key;
        }
    }
}