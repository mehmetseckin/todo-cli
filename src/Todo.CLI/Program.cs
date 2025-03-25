using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Todo.CLI.Commands;
using Todo.CLI.Handlers;
using Todo.CLI.UI;
using Todo.Core;
using Todo.CLI.Auth;
using Todo.CLI;
using Todo.Core.Repository;

var services = new ServiceCollection()
    .AddSingleton<TodoCliConfiguration>()
    .AddSingleton(TodoCliAuthenticationProviderFactory.GetAuthenticationProvider)
    .AddTodoRepositories()
    .AddSingleton<IUserInteraction>(sp => 
    {
        var outputFormat = sp.GetRequiredService<OutputFormat>();
        return new InquirerUserInteraction(outputFormat);
    });

var serviceProvider = services.BuildServiceProvider();

var todoCommand = new TodoCommand(serviceProvider);
return await todoCommand
    .InvokeAsync(args);