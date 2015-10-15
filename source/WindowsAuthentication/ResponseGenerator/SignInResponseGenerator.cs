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
using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel.Constants;
using System.Threading.Tasks;
using IdentityServer.WindowsAuthentication.Services;

namespace IdentityServer.WindowsAuthentication
{
    internal class SignInResponseGenerator
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly WindowsAuthenticationOptions _options;

        public SignInResponseGenerator(WindowsAuthenticationOptions options)
        {
            _options = options;
        }

        public async Task<SignInResponseMessage> GenerateAsync(SignInRequestMessage request, WindowsPrincipal windowsPrincipal)
        {
            Logger.Info("Creating WS-Federation signin response");

            // create subject
            var outgoingSubject = SubjectGenerator.Create(windowsPrincipal, _options);

            // call custom claims tranformation logic
            var context = new CustomClaimsProviderContext
            {
                WindowsPrincipal = windowsPrincipal,
                OutgoingSubject = outgoingSubject
            };
            await _options.CustomClaimsProvider.TransformAsync(context);

            // create token for user
            var token = CreateSecurityToken(context.OutgoingSubject);

            // return response
            var rstr = new RequestSecurityTokenResponse
            {
                AppliesTo = new EndpointReference(_options.IdpRealm),
                Context = request.Context,
                ReplyTo = _options.IdpReplyUrl,
                RequestedSecurityToken = new RequestedSecurityToken(token)
            };

            var serializer = new WSFederationSerializer(
                new WSTrust13RequestSerializer(),
                new WSTrust13ResponseSerializer());

            var mgr = SecurityTokenHandlerCollectionManager.CreateEmptySecurityTokenHandlerCollectionManager();
            mgr[SecurityTokenHandlerCollectionManager.Usage.Default] = CreateSupportedSecurityTokenHandler();

            var responseMessage = new SignInResponseMessage(
                new Uri(_options.IdpReplyUrl),
                rstr,
                serializer,
                new WSTrustSerializationContext(mgr));

            return responseMessage;
        }

        private SecurityToken CreateSecurityToken(ClaimsIdentity outgoingSubject)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = _options.IdpRealm,
                Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(_options.TokenLifeTime)),
                ReplyToAddress = _options.IdpReplyUrl,
                SigningCredentials = new X509SigningCredentials(_options.SigningCertificate),
                Subject = outgoingSubject,
                TokenIssuerName = _options.IssuerUri,
                TokenType = TokenTypes.JsonWebToken
            };

            return CreateSupportedSecurityTokenHandler().CreateToken(descriptor);
        }

        private SecurityTokenHandlerCollection CreateSupportedSecurityTokenHandler()
        {
            return new SecurityTokenHandlerCollection(new SecurityTokenHandler[]
            {
                new JwtSecurityTokenHandler()
            });
        }
    }
}