using InquirerCS;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Todo.Core;
using Todo.Core.Model;

namespace Todo.CLI.Handlers
{
    public class RemoveCommandHandler
    {
        private const string PromptMessage = "Which item(s) would you like to delete?";
        private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";

        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create(async () =>
            {
                var todoItemRepository = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));

                // Retrieve all items
                var items = await todoItemRepository.ListAsync(listAll: true);

                // Ask user which item to delete
                var message = PromptMessage 
                            + Environment.NewLine 
                            + Environment.NewLine
                            + UIHelpMessage;

                var selectedItems = Question
                    .Checkbox(message, items)
                    .Prompt();
            
                DeleteItems(todoItemRepository, selectedItems);
            });
        }

        private static void DeleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
        {
            Task.WaitAll(selectedItems.Select(item => todoItemRepository.DeleteAsync(item)).ToArray());
            Console.Clear();
        }
    }
}
