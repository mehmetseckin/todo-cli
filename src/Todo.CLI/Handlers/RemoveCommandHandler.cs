using InquirerCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Core.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.CLI.Handlers;

using Core.Repository;

public class RemoveCommandHandler
{
    private const string PromptMessage = "Which item(s) would you like to delete?";
    private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";

    public static Func<string, DateTime?, Task<int>> Create(IServiceProvider serviceProvider)
    {
        return async (listName, olderThan) =>
        {
            var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();

            // Retrieve items
            var items = (string.IsNullOrEmpty(listName)
                ? await todoItemRepository.ListAllAsync(includeCompleted: true)
                : await todoItemRepository.ListByListNameAsync(listName, includeCompleted: true)).ToList();

            if (olderThan.HasValue)
            {
                items = items.Where(item =>
                    item.IsCompleted && item.Completed.HasValue && 
                    item.Completed.Value < olderThan.Value
                    )
                    .ToList();
            }

            // Ask user which item to delete
            var message = PromptMessage
                          + Environment.NewLine
                          + Environment.NewLine
                          + UIHelpMessage;

            try
            {
                var selectedItems = Question
                    .Checkbox(message, items)
                    .Prompt();

                DeleteItems(todoItemRepository, selectedItems);
                return 0;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                if (exc.ParamName == "top" &&
                    exc.Message.Contains(
                        "The value must be greater than or equal to zero and less than the console's buffer size in that dimension.",
                        StringComparison.Ordinal))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Error.WriteLineAsync(
                        $"Too many tasks ({items.Count}) to display on the current console. Filter tasks by passing a specific list using the --list parameter, or increase buffer size of the console.");
                    Console.ResetColor();
                    return 1;
                }

                throw;
            }
        };
    }

    private static void DeleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
    {
        Task.WaitAll(selectedItems.Select(todoItemRepository.DeleteAsync).ToArray());
        Console.Clear();
    }
}