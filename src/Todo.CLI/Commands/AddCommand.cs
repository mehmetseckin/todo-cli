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
        private static readonly Argument<string> ListArgument = new("list", "The list to add the to do item to.");
        private static readonly Argument<string> SubjectArgument = new("subject", "The subject of the new to do item.");

        public AddItemCommand(IServiceProvider serviceProvider) : base("item", "Adds a new to do item to the given list.")
        {
            AddArgument(ListArgument);
            AddArgument(SubjectArgument);

            this.SetHandler(AddCommandHandler.Item.Create(serviceProvider), ListArgument, SubjectArgument);
        }
    }
}