using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Todo.CLI.Auth
{
    static class TodoCliAuthenticationProviderFactory
    {
        public static IAuthenticationProvider GetAuthenticationProvider(IServiceProvider factory)
        {
            var config = (TodoCliConfiguration)factory.GetService(typeof(TodoCliConfiguration));

            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(config.ClientId)
                .WithRedirectUri("todo://auth-callback")
                .Build();
            
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);

            return new InteractiveAuthenticationProvider(app, config.Scopes);
        }
    }
}
