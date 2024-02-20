using System;
using System.CommandLine;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands;

public class ListCommand : Command
{
    private static readonly Option<bool> GetAllOption = new(["-a", "--all"], "Lists all to do items including the completed ones.");
    private static readonly Option<bool> NoStatusOption = new(["--no-status"], "Suppresses the bullet indicating whether the item is completed or not.");
    private static readonly Argument<string> ListNameArgument = new("list-name", "Only list tasks of this To-Do list.")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    public ListCommand(IServiceProvider serviceProvider) : base("list")
    {
        Description = "Retrieves a list of the to do items across all To-Do lists.";

        Add(GetAllOption);
        Add(NoStatusOption);
        Add(ListNameArgument);

        this.SetHandler(ListCommandHandler.Create(serviceProvider), GetAllOption, NoStatusOption, ListNameArgument);
    }
}
