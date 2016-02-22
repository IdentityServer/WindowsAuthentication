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

using Autofac;
using Autofac.Integration.WebApi;
using IdentityServer.WindowsAuthentication;
using IdentityServer.WindowsAuthentication.Configuration;
using IdentityServer.WindowsAuthentication.Logging;
using IdentityServer.WindowsAuthenticationService.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace Owin
{
    /// <summary>
    /// Helper class for pipeline configuration
    /// </summary>
    public static class WindowsAuthenticationAppBuilderExtensions
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Extension method for adding the windows authentication service to the pipeline
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="options">The options class.</param>
        /// <returns></returns>
        public static IAppBuilder UseWindowsAuthenticationService(this IAppBuilder app, WindowsAuthenticationOptions options)
        {
            Logger.Info("Starting configuration.");

            app.ConfigureBaseUrl(options.PublicOrigin);

            if (options.EnableWsFederationEndpoint)
            {
                Logger.Info("Adding WS-Federation endpoint");

                var webApiConfig = new HttpConfiguration();
                webApiConfig.MapHttpAttributeRoutes();
                webApiConfig.Services.Add(typeof(IExceptionLogger), new LogProviderExceptionLogger());
                webApiConfig.Services.Replace(typeof(IHttpControllerTypeResolver), new ControllerResolver());

                var builder = new ContainerBuilder();
                builder.RegisterInstance(options);
                builder.RegisterApiControllers(typeof(AuthenticationController).Assembly);

                webApiConfig.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
                app.UseWebApi(webApiConfig);
            }

            if (options.EnableOAuth2Endpoint)
            {
                Logger.Info("Adding OAuth2 endpoint");

                app.Use(async (context, next) =>
                    {
                        if (context.Request.Uri.AbsolutePath.EndsWith("/token", StringComparison.OrdinalIgnoreCase))
                        {
                            if (context.Authentication.User == null || 
                                !context.Authentication.User.Identity.IsAuthenticated)
                            {
                                context.Response.StatusCode = 401;
                                return;
                            }
                        }

                        await next();
                    });

                app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
                    {
                        AllowInsecureHttp = true,
                        TokenEndpointPath = new PathString("/token"),
                        Provider = new WindowsAuthenticationTokenProvider(options),
                        AccessTokenFormat = new JwtFormat(options),
                        AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(options.TokenLifeTime)
                    });
            }

            SignatureConversions.AddConversions(app);

            Logger.Info("Configuration done.");
            return app;
        }
    }
}