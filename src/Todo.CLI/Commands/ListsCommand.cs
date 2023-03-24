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
                var response = await client.RequestAsync("lists");
                repo.AddLists(response);

                // fnord json parse
            });
        }

    }
}
