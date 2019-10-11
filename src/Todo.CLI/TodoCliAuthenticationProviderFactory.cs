using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Todo.CLI
{
    static class TodoCliAuthenticationProviderFactory
    {
        public static IAuthenticationProvider GetAuthenticationProvider(IServiceProvider factory)
        {
            var config = (TodoCliConfiguration)factory.GetService(typeof(TodoCliConfiguration));

            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                .Create(config.ClientId)
                .WithRedirectUri("http://localhost")
                .Build();

            return new InteractiveAuthenticationProvider(publicClientApplication, config.Scopes);
        }
    }
}
