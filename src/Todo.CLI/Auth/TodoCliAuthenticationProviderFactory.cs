using System;
using System.Linq;

namespace Todo.CLI.Auth;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

static class TodoCliAuthenticationProviderFactory
{
    public static IAuthenticationProvider GetAuthenticationProvider(IServiceProvider factory)
    {
        var config = factory.GetRequiredService<TodoCliConfiguration>();

        IPublicClientApplication app = PublicClientApplicationBuilder
            .Create(config.ClientId)
            .WithRedirectUri("http://localhost") // Only loopback redirect uri is supported, see https://aka.ms/msal-net-os-browser for details
            .Build();

        TokenCacheHelper.EnableSerialization(app.UserTokenCache);

        // Try to get token silently from cache first
        var accounts = app.GetAccountsAsync().GetAwaiter().GetResult();
        AuthenticationResult? result = null;

        if (accounts.Any())
        {
            try
            {
                result = app.AcquireTokenSilent(config.Scopes, accounts.First())
                    .ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MsalUiRequiredException)
            {
                // Token expired or invalid, will fall through to interactive login
            }
        }

        // If silent acquisition failed or no accounts found, do interactive login
        if (result == null)
        {
            result = app.AcquireTokenInteractive(config.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        }

        return new ApiKeyAuthenticationProvider("Bearer " + result.AccessToken, "Authorization",
            ApiKeyAuthenticationProvider.KeyLocation.Header);
    }
}