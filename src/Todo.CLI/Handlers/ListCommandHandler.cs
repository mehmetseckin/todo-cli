using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Todo.Core;

namespace Todo.CLI.Handlers
{
    public class ListCommandHandler
    {
        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create(async () =>
            {
                var todoItemRetriever = (ITodoItemRetriever)serviceProvider.GetService(typeof(ITodoItemRetriever));
                var todoItems = await todoItemRetriever.ListAsync();
                foreach (var item in todoItems)
                {
                    Console.WriteLine(item.Subject);
                }
            });
        }
    }
}
