using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Security.Cases.EdiscoveryCases.Item.SecurityReopen;
using MSTTool.Graph;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using Todo.Core.Repository;

namespace MSTTool.Commands
{
    public class ListsCommand : Command
    {
        public ListsCommand(IServiceProvider serviceProvider) : base("lists")
        {
            Description = "Retrieves a list of all ToDo Lists.";

            this.SetHandler(async () =>
            {
                var client = serviceProvider.GetService<GraphClient>();
                var repo = serviceProvider.GetService<TodoItemRepository>();
                var uri = "lists";
                do
                {
                    var response = await client.RequestAsync(uri);
                    // this parses the json and populates the repo
                    uri = repo.AddLists(response);
                } while (uri != null);

                foreach (var list in repo.Lists)
                {
                    Console.WriteLine(list.displayName);
                }
            });
        }

    }
}
