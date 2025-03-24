using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Model;
using Todo.Core.Repository;
using Todo.CLI.UI;

namespace Todo.CLI.Handlers;

public class CompleteCommandHandler
{
    public static Func<string, Task<int>> Create(IServiceProvider serviceProvider)
    {
        return async id =>
        {
            var userInteraction = serviceProvider.GetRequiredService<IUserInteraction>();
            
            if (string.IsNullOrEmpty(id))
            {
                userInteraction.ShowError("Please provide an item ID to complete.");
                return 1;
            }

            try
            {
                var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
                
                var items = await todoItemRepository.ListAllAsync(false);
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item == null)
                {
                    userInteraction.ShowError($"Item with ID {id} not found.");
                    return 1;
                }
                
                await todoItemRepository.CompleteAsync(item);
                Console.WriteLine($"Item {id} marked as complete.");
                return 0;
            }
            catch (Exception ex)
            {
                userInteraction.ShowError($"Error completing item: {ex.Message}");
                return 1;
            }
        };
    }
}