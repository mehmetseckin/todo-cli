using System;
using System.CommandLine;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands;

public class RemoveCommand : Command
{
    private static readonly Option<string> ListOpt = new(["--list", "-l"], "The name of the list to remove the item from.");
    private static readonly Option<DateTime?> OlderThanOpt = new(new[] { "--older-than" }, "Only items completed before this date.");
    public RemoveCommand(IServiceProvider serviceProvider) : base("remove", "Deletes a to do item.")
    {
        Add(ListOpt);
        Add(OlderThanOpt);
        this.SetHandler(RemoveCommandHandler.Create(serviceProvider), ListOpt, OlderThanOpt);
    }
}