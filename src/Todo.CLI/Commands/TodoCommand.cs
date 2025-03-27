using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.CLI.UI;

namespace Todo.CLI.Commands;

public class TodoCommand : RootCommand
{
    public static readonly Option<OutputFormat> OutputFormat = new(
        aliases: ["-o", "--output"],
        description: "Specifies the output format.",
        getDefaultValue: () => UI.OutputFormat.Interactive);

    public TodoCommand(IServiceProvider serviceProvider)
    {
        // Add static parameters
        Description = "A CLI to manage Microsoft to do items.";

        // Add output format option
        Add(OutputFormat);

        // Add subcommands
        Add(new AddCommand(serviceProvider));
        Add(new ListCommand(serviceProvider));
        Add(new CompleteCommand(serviceProvider));
        Add(new RemoveCommand(serviceProvider));
    }
}