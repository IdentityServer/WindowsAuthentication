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

using System.Collections.Generic;

namespace IdentityServer.WindowsAuthentication.Configuration
{
    internal static class HelperExtensions
    {
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        internal static void SetIdentityServerHost(this IDictionary<string, object> env, string value)
        {
            env[Constants.OwinEnvironment.ServerHost] = value;
        }

        internal static void SetIdentityServerBasePath(this IDictionary<string, object> env, string value)
        {
            env[Constants.OwinEnvironment.ServerBasePath] = value;
        }

        internal static string GetServerHost(this IDictionary<string, object> env)
        {
            return env[Constants.OwinEnvironment.ServerHost] as string;
        }

        internal static string GetServerBasePath(this IDictionary<string, object> env)
        {
            return env[Constants.OwinEnvironment.ServerBasePath] as string;
        }

        internal static string GetServerBaseUrl(this IDictionary<string, object> env)
        {
            return env.GetServerHost() + env.GetServerBasePath();
        }
    }
}
