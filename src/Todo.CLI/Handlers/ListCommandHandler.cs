using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Todo.Core;
using Todo.Core.Model;

namespace Todo.CLI.Handlers
{
    /*fnordwip
    public class ListCommandHandler
    {
        private const char TodoBullet = '-';
        private const char CompletedBullet = '\u2713'; // Sqrt - check mark

        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            return CommandHandler.Create<bool, bool>(async (all, noStatus) =>
            {
                var todoItemRetriever = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));
                var todoItemsAsync = todoItemRetriever.ListAsyncEnumerable(all);

                await foreach (var item in todoItemsAsync)
                {
                    if(!noStatus)
                    {
                        RenderBullet(item);
                        Console.Write(" ");
                    }

                    Render(item);
                }
            });
        }

        private static void Render(TodoItem item)
        {
            Console.Write(item.Subject);
            Console.Write(Environment.NewLine);
        }

        private static void RenderBullet(TodoItem item)
        {
            ConsoleColor bulletColor;
            char bullet;

            if (item.IsCompleted)
            {
                bulletColor = ConsoleColor.Green;
                bullet = CompletedBullet;
            }
            else
            {
                bulletColor = ConsoleColor.Red;
                bullet = TodoBullet;
            }

            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = bulletColor;
            Console.Write(bullet);
            Console.ForegroundColor = previousColor;
        }
    }
    */
}
