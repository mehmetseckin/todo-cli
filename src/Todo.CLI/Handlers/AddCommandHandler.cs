namespace Todo.CLI.Handlers;

using System;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

public class AddCommandHandler
{
    internal class List
    {
        public static Func<string, Task> Create(IServiceProvider serviceProvider)
        {
            return async name =>
            {
                if (string.IsNullOrEmpty(name))
                    throw new InvalidOperationException("name is required to add a list.");

                var todoListRepository = serviceProvider.GetRequiredService<ITodoListRepository>();
                await todoListRepository.AddAsync(new TodoList
                {
                    Name = name
                });
            };
        }
    }

    internal class Item
    {
        public static Func<string, string, Task> Create(IServiceProvider serviceProvider)
        {
            return async (listName, subject) =>
            {
                if (string.IsNullOrEmpty(listName))
                    throw new InvalidOperationException("list is required to add an item.");

                var todoListRepo = serviceProvider.GetRequiredService<ITodoListRepository>();
                var list = await todoListRepo.GetByNameAsync(listName) ?? throw new InvalidOperationException($"No list found with the name '{listName}'.");
                var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
                await todoItemRepository.AddAsync(new TodoItem
                {
                    Subject = subject,
                    ListId = list.Id
                });
            };
        }
    }
}