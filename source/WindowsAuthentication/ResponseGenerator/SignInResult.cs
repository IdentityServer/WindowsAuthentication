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

using System.IdentityModel.Services;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace IdentityServer.WindowsAuthentication
{
    internal class SignInResult : IHttpActionResult
    {
        //private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly SignInResponseMessage _message;

        public SignInResult(SignInResponseMessage message)
        {
            _message = message;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        HttpResponseMessage Execute()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(_message.WriteFormPost(), Encoding.UTF8, "text/html");

            //Logger.Debug("Returning WS-Federation signin response");
            return response;
        }
    }
}