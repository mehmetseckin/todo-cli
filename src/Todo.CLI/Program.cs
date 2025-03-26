using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Todo.CLI.Commands;
using Todo.CLI.UI;
using Todo.Core;
using Todo.CLI.Auth;
using Todo.CLI;
using System;

var services = new ServiceCollection()
    .AddSingleton<TodoCliConfiguration>()
    .AddSingleton(TodoCliAuthenticationProviderFactory.GetAuthenticationProvider)
    .AddTodoRepositories()
    .AddSingleton<IUserInteraction>(sp => 
    {
        var outputFormat = OutputFormat.Interactive;
        if (args != null && args.Length > 1)
        {
            for (var i = 0; i < args.Length - 1; i++)
            {
                if ((args[i] == "--output" || args[i] == "-o") && 
                    Enum.TryParse<OutputFormat>(args[i + 1], true, out var format))
                {
                    outputFormat = format;
                    break;
                }
            }
        }
        return new InquirerUserInteraction(outputFormat);
    });

var serviceProvider = services.BuildServiceProvider();

var todoCommand = new TodoCommand(serviceProvider);
return await todoCommand
    .InvokeAsync(args);