using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.CLI.UI;

namespace Todo.CLI.Commands;

public class AddCommand : Command
{
    public AddCommand(IServiceProvider serviceProvider) : base("add", "Adds a to do item or list.")
    {
        Add(new AddListCommand(serviceProvider));
        Add(new AddItemCommand(serviceProvider));
    }

    internal class AddListCommand : Command
    {
        private static readonly Argument<string> NameArgument = new("name", "The name of the new to do list.");

        public AddListCommand(IServiceProvider serviceProvider) : base("list", "Adds a new to do list.")
        {
            AddArgument(NameArgument);

            this.SetHandler(AddCommandHandler.List.Create(serviceProvider), NameArgument);
        }
    }

    internal class AddItemCommand : Command
    {
        private static readonly Argument<string> SubjectArgument = new("subject", "The subject of the new to do item.");
        private static readonly Option<string> ListOption = new("--list", "The list to add the to do item to.");
        private static readonly Option<bool> StarOption = new("--star", "Stars (marks as important) the new to do item.");
        private static readonly Option<string> DueDateOption = new("--due-date", "The due date (yyyy-MM-dd or MM-dd).");

        public AddItemCommand(IServiceProvider serviceProvider) : base("item", "Adds a new to do item.")
        {
            AddArgument(SubjectArgument);
            AddOption(ListOption);
            AddOption(StarOption);
            AddOption(DueDateOption);

            this.SetHandler(AddCommandHandler.Item.Create(serviceProvider), SubjectArgument, ListOption, StarOption, DueDateOption);
        }
    }
}