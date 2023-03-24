using Microsoft.Extensions.DependencyInjection;
using MSTTool.Graph;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;

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
                var response = await client.RequestAsync("lists");
                // fnord json parse
            });
        }

    }
}
