using Todo.CLI.Handlers;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.CLI.Commands
{
    public class TodoRootCommand : RootCommand
    {
        public TodoRootCommand(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<TodoCliConfiguration>();

            // Add static parameters
            Description = "A CLI to manage Microsoft to do items.";
            
            // Add options
            AddOption(GetVersionOption());
            
            // Add handlers
            Handler = TodoCommandHandler.Create();

            // Add subcommands
            AddCommand(new ListCommand(serviceProvider));
            AddCommand(new ExportCommand(serviceProvider));
            if (config.SupportsWrite)
            {
                throw new NotImplementedException();
                /* USE_WRITECOMMANDS
                AddCommand(new AddCommand(serviceProvider));
                AddCommand(new CompleteCommand(serviceProvider));
                AddCommand(new RemoveCommand(serviceProvider));
                */
            }
        }

        private Option GetVersionOption()
        {
            return new Option(new string[] { "-v", "--version" }, "Prints out the todo CLI version.")
            {
                Argument = new Argument<bool>()
            };
        }
    }
}
