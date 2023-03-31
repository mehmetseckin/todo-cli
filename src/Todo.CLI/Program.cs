
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Threading.Tasks;
using Todo.Core.Interfaces;
using Todo.Core.Repository;
using Todo.MSTTool.Auth;
using Todo.MSTTool.Commands;
using Todo.MSTTool.Graph;

namespace Todo.MSTTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var mstConfig = new MSTConfiguration();
            config.Bind(nameof(MSTConfiguration), mstConfig);

            var services = new ServiceCollection()
                .AddSingleton(mstConfig)
                .AddSingleton(factory => new InteractiveAuthenticator(factory))
                .AddSingleton<IGraphClient>(factory => new GraphClient(factory))
                .AddSingleton(factory => new TodoItemRepository(factory));

            var serviceProvider = services.BuildServiceProvider();

            return await new TodoRootCommand(serviceProvider)
                .InvokeAsync(args);
        }
    }
}
