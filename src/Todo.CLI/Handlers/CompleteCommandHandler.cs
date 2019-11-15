using InquirerCS;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Todo.Core;
using Todo.Core.Model;

namespace Todo.CLI.Handlers
{
    public class CompleteCommandHandler
    {
        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create<string>(async (itemId) =>
            {
                var todoItemRepository = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));
                if (!string.IsNullOrEmpty(itemId))
                {
                    await CompleteItem(todoItemRepository, new TodoItem { Id = itemId });
                }
                else
                {
                    // Retrieve items that are not completed
                    var items = await todoItemRepository.ListAsync(listAll: false);

                    // Ask user which item to complete
                    var selectedItem = Question
                        .List("Which item would you like to complete?", items)
                        .Prompt();
                    await CompleteItem(todoItemRepository, selectedItem);
                }
            });
        }

        private static async Task CompleteItem(ITodoItemRepository todoItemRepository, TodoItem selectedItem)
        {
            await todoItemRepository.CompleteAsync(selectedItem);
            Console.Clear();
        }
    }
}
