using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Todo.Core;
using Todo.Core.Model;

namespace Todo.CLI.Handlers
{
    public class AddCommandHandler
    {
        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create<string>(async (subject) =>
            {
                var todoItemRepository = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));
                await todoItemRepository.AddAsync(new TodoItem()
                {
                    Subject = subject
                });
            });
        }
    }
}
