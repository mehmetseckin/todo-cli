using System;
using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.CLI.Commands;
using Todo.CLI.Handlers;
using Todo.CLI.UI;
using Todo.Core;
using Todo.CLI.Auth;
using Todo.CLI;
using Todo.Core.Repository;

var config = new ConfigurationBuilder()
    .AddJsonFile("./appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var todoCliConfig = new TodoCliConfiguration();
config.Bind("TodoCliConfiguration", todoCliConfig);

var services = new ServiceCollection()
    .AddSingleton(todoCliConfig)
    .AddSingleton(TodoCliAuthenticationProviderFactory.GetAuthenticationProvider)
    .AddTodoRepositories()
    .AddSingleton<IUserInteraction, InquirerUserInteraction>();

var serviceProvider = services.BuildServiceProvider();

var todoCommand = new TodoCommand(serviceProvider);
return await todoCommand
    .InvokeAsync(args); // Exception here