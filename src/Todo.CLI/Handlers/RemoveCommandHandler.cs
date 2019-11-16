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
    public class RemoveCommandHandler
    {
        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create(async () =>
            {
                var todoItemRepository = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));

                // Retrieve all items
                var items = await todoItemRepository.ListAsync(listAll: true);

                // Ask user which item to delete
                var selectedItem = Question
                    .List("Which item(s) would you like to delete?", items)
                    .Prompt();
            
                await DeleteItem(todoItemRepository, selectedItem);
            });
        }

        private static async Task DeleteItem(ITodoItemRepository todoItemRepository, TodoItem selectedItem)
        {
            await todoItemRepository.DeleteAsync(selectedItem);
            Console.Clear();
        }
    }
}
