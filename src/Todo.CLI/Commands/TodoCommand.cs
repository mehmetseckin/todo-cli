using Todo.CLI.Handlers;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace Todo.CLI.Commands
{
    public class TodoCommand : RootCommand
    {
        public TodoCommand(IServiceProvider serviceProvider, TodoCliConfiguration config)
        {
            // Add static parameters
            Description = "A CLI to manage Microsoft to do items.";
            
            // Add options
            AddOption(GetVersionOption());
            
            // Add handlers
            Handler = TodoCommandHandler.Create();

            // Add subcommands
            AddCommand(new ListCommand(serviceProvider));
            if (config.SupportsWrite)
            {
                AddCommand(new AddCommand(serviceProvider));
                AddCommand(new CompleteCommand(serviceProvider));
                AddCommand(new RemoveCommand(serviceProvider));
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
