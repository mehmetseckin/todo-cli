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
using System.Diagnostics;
using System.Linq;

namespace Todo.CLI.Auth
{
    // TODO: interface
    public class InteractiveAuthenticator
    {
        public TodoCliConfiguration Config { get; }

        public AsyncLazy<AuthenticationResult> AuthenticationResultAsync { get; }

        public AsyncLazy<string> AccessTokenAsync { get; }

        public InteractiveAuthenticator(IServiceProvider serviceProvider)
        {
            Config = serviceProvider.GetService<TodoCliConfiguration>();

            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(Config.ClientId)
                .WithRedirectUri("http://localhost") // Only loopback redirect uri is supported, see https://aka.ms/msal-net-os-browser for details
                .Build();

            // TECH: use LoginHint or IAccount to access
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);

            AuthenticationResultAsync = new AsyncLazy<AuthenticationResult>(() => GetAuthenticationResultAsync(app));
            AccessTokenAsync = new AsyncLazy<string>(async () =>
            {
                var authResult = await AuthenticationResultAsync;
                return authResult.AccessToken; 
            });
        }

        // REF: https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-acquire-token-silently
        private async Task<AuthenticationResult> GetAuthenticationResultAsync(IPublicClientApplication app)
        {
            // arbitrarily choose the first account actually signed in the token cache
            var accounts = await app.GetAccountsAsync();

            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenSilent(Config.Scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                result = await app.AcquireTokenInteractive(Config.Scopes)
                                    .ExecuteAsync();
            }

            return result;
        }

        public async Task ConfigureClient(HttpClient client)
        {
            var accessToken = await AccessTokenAsync;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
