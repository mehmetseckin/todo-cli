using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.CLI.UI;

namespace Todo.CLI.Commands;

public class CompleteCommand : Command
{
    private static readonly Argument<string[]> IdArg =
        new("id", "The ID of the todo item(s) to complete.")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
    private static readonly Option<string> ListOpt = new(["--list", "-l"], "The name of the list to complete items from.");
    private static readonly Option<DateTime?> OlderThanOpt = new(["--older-than"], "Only items created before this date.");
    private static readonly Option<bool> AllOpt = new(new[] { "--all", "-a" }, "Complete all items fitting the filter. You will be prompted before removal.");
  
    public CompleteCommand(IServiceProvider serviceProvider) : base("complete")
    {
        Description = "Completes a to do item.";

        Add(IdArg);
        Add(ListOpt);
        Add(OlderThanOpt);
        Add(AllOpt);

        this.SetHandler(CompleteCommandHandler.Create(serviceProvider), IdArg, ListOpt, OlderThanOpt, AllOpt);
    }
}