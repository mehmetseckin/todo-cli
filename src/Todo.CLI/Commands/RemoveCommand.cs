using System;
using System.CommandLine;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands;

public class RemoveCommand : Command
{
    private static readonly Option<string> ListOpt = new(["--list", "-l"], "The name of the list to remove the item from.");
    private static readonly Option<DateTime?> OlderThanOpt = new(["--older-than"], "Only items completed before this date.");
    private static readonly Option<bool> AllOpt = new(new[] { "--all", "-a" }, "Remove all items fitting the filter. You will be prompted before removal.");
    private static readonly Option<bool> CompletedOpt = new(new[] { "--completed", "-c" }, "Remove only completed items.");
    
    public RemoveCommand(IServiceProvider serviceProvider) : base("remove", "Deletes to do items.")
    {
        Add(ListOpt);
        Add(OlderThanOpt);
        Add(AllOpt);
        Add(CompletedOpt);
        this.SetHandler(RemoveCommandHandler.Create(serviceProvider), ListOpt, OlderThanOpt, AllOpt, CompletedOpt);
    }
}