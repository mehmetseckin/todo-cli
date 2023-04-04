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
using System.CommandLine.Parsing;
using Tavis.UriTemplates;

namespace Todo.MSTTool.Auth
{
    // TODO: interface
    public class InteractiveAuthenticator
    {
        public MSTConfiguration Config { get; }

        public AsyncLazy<AuthenticationResult> AuthenticationResultAsync { get; }

        public AsyncLazy<string> AccessTokenAsync { get; }

        public InteractiveAuthenticator(IServiceProvider serviceProvider)
        {
            Config = serviceProvider.GetService<MSTConfiguration>();

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

        private async Task<AuthenticationResult> LoginInteractiveAsync(IPublicClientApplication app)
        {
            Console.WriteLine("Login Interactive");
            var result = await app.AcquireTokenInteractive(Config.Scopes)
                    .ExecuteAsync();
            return result;
        }

        private async Task<AuthenticationResult> LoginAsync(IPublicClientApplication app, IAccount account)
        {
            Console.WriteLine("Login: {0}", account.Username);

            var result = await app.AcquireTokenSilent(Config.Scopes, account)
                              .ExecuteAsync();

            return result;
        }

        // REF: https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-acquire-token-silently
        private async Task<AuthenticationResult> GetAuthenticationResultAsync(IPublicClientApplication app)
        {
            // arbitrarily choose the first account actually signed in the token cache
            var accounts = await app.GetAccountsAsync();
            var account = accounts.FirstOrDefault();

            AuthenticationResult result = null;
            if (account != null)
            {
                try
                {
                    result = await LoginAsync(app, account);
                }
                catch (MsalUiRequiredException ex)
                {
                    // A MsalUiRequiredException happened on AcquireTokenSilent.
                    // This indicates you need to call AcquireTokenInteractive to acquire a token
                    Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                    result = await LoginInteractiveAsync(app);
                }
            }
            else
            {
                result = await LoginInteractiveAsync(app);
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
