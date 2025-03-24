using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.CLI.UI;

namespace Todo.CLI.Commands;

public class CompleteCommand : Command
{
    private static readonly Argument<string> IdArg =
        new("id", "The ID of the todo item to complete.")
            { Arity = ArgumentArity.ExactlyOne };

    public CompleteCommand(IServiceProvider serviceProvider) : base("complete")
    {
        Description = "Completes a to do item.";

        Add(IdArg);

        this.SetHandler(CompleteCommandHandler.Create(serviceProvider), IdArg);
    }
}