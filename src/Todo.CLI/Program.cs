
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
using System.Threading.Tasks;
using MSTTool.Graph;

namespace Todo.CLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var todoCliConfig = new TodoCliConfiguration();
            config.Bind("TodoCliConfiguration", todoCliConfig);

            var services = new ServiceCollection()
                .AddSingleton(todoCliConfig)
                .AddSingleton(factory => new InteractiveAuthenticator(factory))
                .AddSingleton(factory => new GraphClient(factory))
                .AddSingleton(factory => new TodoItemRepository(factory));

            var serviceProvider = services.BuildServiceProvider();

            return await new TodoRootCommand(serviceProvider)
                .InvokeAsync(args);
        }
    }
}
