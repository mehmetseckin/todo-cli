using System;
using System.CommandLine;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands;

public class RemoveCompletedCommand : Command
{
    private static readonly Argument<string> ListNameArgument = new("list-name", "The name of the list from where to remove completed items.")
    {
        Arity = ArgumentArity.ExactlyOne
    };


    public RemoveCompletedCommand(IServiceProvider serviceProvider) : base("remove-completed")
    {
        Description = "Removes completed items from a list.";

        Add(ListNameArgument);

        this.SetHandler(RemoveCompletedCommandHandler.Create(serviceProvider), ListNameArgument);
    }
}
