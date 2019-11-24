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
    public class CompleteCommandHandler
    {
        private const string PromptMessage = "Which item(s) would you like to delete?";
        private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";

        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create<string>(async (itemId) =>
            {
                var todoItemRepository = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));
                
                if (!string.IsNullOrEmpty(itemId))
                {
                    var item = new TodoItem { Id = itemId };
                    await todoItemRepository.CompleteAsync(item);
                }
                else
                {
                    // Retrieve items that are not completed
                    var items = await todoItemRepository.ListAsync(listAll: false);

                    // Ask user which items to complete
                    var message = PromptMessage
                                + Environment.NewLine
                                + Environment.NewLine
                                + UIHelpMessage;
                    
                    var selectedItems = Question
                        .Checkbox(message, items)
                        .Prompt();

                    CompleteItems(todoItemRepository, selectedItems);
                }
                
                Console.Clear();
            });
        }

        private static void CompleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
        {
            Task.WaitAll(selectedItems.Select(item => todoItemRepository.CompleteAsync(item)).ToArray());
        }
    }
}
