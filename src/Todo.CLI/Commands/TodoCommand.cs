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
        public TodoCommand()
        {
            Description = "A CLI to manage to do items.";

            AddOption(new Option(new string[] { "-v", "--version" }, "Prints out the todo CLI version.")
            {
                Argument = new Argument<bool>()
            });

            Handler = TodoCommandHandler.Create();
        }
    }
}
