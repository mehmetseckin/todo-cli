using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Security.Cases.EdiscoveryCases.Item.SecurityReopen;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using Todo.Core.Repository;
using Todo.MSTTool.Graph;

namespace Todo.MSTTool.Commands
{
    public class ListsCommand : Command
    {
        public ListsCommand(IServiceProvider serviceProvider) : base("lists")
        {
            Description = "Retrieves a list of all ToDo Lists.";

            this.SetHandler(async () =>
            {
                var repo = serviceProvider.GetService<TodoItemRepository>();
                await repo.PopulateListsAsync();

                foreach (var list in repo.Lists)
                {
                    Console.WriteLine(list.displayName);
                }
            });
        }

    }
}
