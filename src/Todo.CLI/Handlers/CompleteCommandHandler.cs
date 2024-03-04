using InquirerCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Core.Model;

namespace Todo.CLI.Handlers;

using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

public class CompleteCommandHandler
{
    private const string PromptMessage = "Which item(s) would you like to delete?";
    private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";

    public static Func<string, string, Task<int>> Create(IServiceProvider serviceProvider)
    {
        return async (itemName, listName) =>
        {
            try
            {
                var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
                var items = string.IsNullOrEmpty(listName)
                    ? await todoItemRepository.ListAllAsync(false)
                    : await todoItemRepository.ListByListNameAsync(listName, false);

                if (!string.IsNullOrEmpty(itemName))
                {
                    var item = items.FirstOrDefault(i => i.Subject == itemName);
                    if (item is null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        await Console.Error.WriteLineAsync($"Item called \"{itemName}\" not found.");
                        Console.ResetColor();
                        return 1;
                    }
                    await todoItemRepository.CompleteAsync(item);
                }
                else
                {
                    // Ask user which items to complete
                    var message = PromptMessage
                                  + Environment.NewLine
                                  + Environment.NewLine
                                  + UIHelpMessage;

                    var selectedItems = Question
                        .Checkbox(message, items)
                        .Page(50)
                        .Prompt();

                    CompleteItems(todoItemRepository, selectedItems);
                }

                Console.Clear();
                return 0;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                if (exc.ParamName == "top" && exc.Message.Contains("The value must be greater than or equal to zero and less than the console's buffer size in that dimension.", StringComparison.Ordinal))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Error.WriteLineAsync("Too many tasks to display on the current console. Filter tasks by passing a specific list using the --list parameter, or increase buffer size of the console.");
                    Console.ResetColor();
                    return 1;
                }

                throw;
            }
        };
    }

    private static void CompleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
    {
        Task.WaitAll(selectedItems.Select(todoItemRepository.CompleteAsync).ToArray());
    }
}