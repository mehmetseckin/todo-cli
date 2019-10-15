using Todo.CLI.Commands;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core;
using Microsoft.Graph;
using Todo.CLI.Auth;
using Todo.Core.Repository;

namespace Todo.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var todoCliConfig = new TodoCliConfiguration();
            config.Bind("TodoCliConfiguration", todoCliConfig);

            var services = new ServiceCollection()
                .AddSingleton(typeof(TodoCliConfiguration), todoCliConfig)
                .AddTransient<ITodoItemRepository>(factory => new TodoItemRepository(TodoCliAuthenticationProviderFactory.GetAuthenticationProvider(factory)));

            var serviceProvider = services.BuildServiceProvider();

            return new TodoCommand(serviceProvider)
                .InvokeAsync(args)
                .Result;
        }
    }
}
