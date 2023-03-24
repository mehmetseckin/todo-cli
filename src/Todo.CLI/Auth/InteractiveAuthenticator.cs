using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.CLI.Auth
{
    // TODO: interface
    public class InteractiveAuthenticator
    {
        public AsyncLazy<AuthenticationResult> AuthenticationResultAsync { get; }

        public AsyncLazy<string> AccessTokenAsync { get; }

        public InteractiveAuthenticator(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<TodoCliConfiguration>();

            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(config.ClientId)
                .WithRedirectUri("http://localhost") // Only loopback redirect uri is supported, see https://aka.ms/msal-net-os-browser for details
                .Build();

            //fnordnotwroking
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);

            AuthenticationResultAsync = new AsyncLazy<AuthenticationResult>(() => app.AcquireTokenInteractive(config.Scopes).ExecuteAsync());
            AccessTokenAsync = new AsyncLazy<string>(async () =>
            {
                var authResult = await AuthenticationResultAsync;
                return authResult.AccessToken; 
            });
        }

        public async Task ConfigureClient(HttpClient client)
        {
            var accessToken = await AccessTokenAsync;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
