using System;
using System.CommandLine;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands;

public class CompleteCommand : Command
{
    private static readonly Argument<string> ItemArg =
        new("name",
                "The name of the todo item to complete. If multiple lists have this item, the first one will be completed.")
            { Arity = ArgumentArity.ZeroOrOne };
    private static readonly Option<string> ListOpt = new(["--list", "-l"], "The name of the list to complete the item in.")
    { Arity = ArgumentArity.ZeroOrOne };

    public CompleteCommand(IServiceProvider serviceProvider) : base("complete")
    {
        Description = "Completes a to do item.";

        Add(ItemArg);
        Add(ListOpt);

        this.SetHandler(CompleteCommandHandler.Create(serviceProvider), ItemArg, ListOpt);
    }
}