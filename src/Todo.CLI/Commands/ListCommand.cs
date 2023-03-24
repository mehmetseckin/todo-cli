using Microsoft.Extensions.Configuration.CommandLine;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Todo.CLI.Handlers;
using Todo.Core;
using Todo.Core.Model;

namespace MSTTool.Commands
{
    public class ListCommand : Command
    {
        public ListCommand(IServiceProvider serviceProvider) : base("list")
        {
            Description = "Retrieves a list of the ToDo items.";

            var targetListArg = new Argument<string>("listName");
            AddArgument(targetListArg);

            this.SetHandler<string>((a) =>
            {
                Console.WriteLine("List Argument:{0}", a);
                throw new NotImplementedException("TODO");
            },
            targetListArg);
        }

        /*fnordwip
        #region ICommandHandler

        private const char TodoBullet = '-';
        private const char CompletedBullet = '\u2713'; // Sqrt - check mark

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            context.
        }

        public static ICommandHandler Create(IServiceProvider serviceProvider)
        {
            CommandHandler.Create()
            return CommandHandler.Create<bool, bool>(async (all, noStatus) =>
            {
                var todoItemRetriever = (ITodoItemRepository)serviceProvider.GetService(typeof(ITodoItemRepository));
                var todoItemsAsync = todoItemRetriever.ListAsyncEnumerable(all);

                await foreach (var item in todoItemsAsync)
                {
                    if (!noStatus)
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

        #endregion
        */

    }
}