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
        public TodoCommand(IServiceProvider serviceProvider)
        {
            // Add static parameters
            Description = "A CLI to manage Microsoft to do items.";
            
            // Add options
            AddOption(GetVersionOption());
            
            // Add handlers
            Handler = TodoCommandHandler.Create();

            // Add subcommands
            AddCommand(new AddCommand(serviceProvider));
            AddCommand(new ListCommand(serviceProvider));
            AddCommand(new CompleteCommand(serviceProvider));
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
