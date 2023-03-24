using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Todo.CLI.Handlers;
using Todo.Core;
using Todo.Core.Model;

namespace Todo.CLI.Commands
{
    public class ListCommand : Command
    {
        public ListCommand(IServiceProvider serviceProvider) : base("list")
        {
            Description = "Retrieves a list of the ToDo items.";

            /* fnord - all, vs specific list
            AddOption(GetAllOption());
            AddOption(GetNoStatusOption());
            */

            // fnord - can we get list by displayname using the API, or do we need to get full list and cache?
            // fnord .NETCore no Option<T>?
            //new Option
            //var listOption = new Option<string>("-l", "Get all tasks in the specified list");

            // fnord is there even a "get all"?
            //Handler = CommandHandler.Create<>
        }


        /*fnordwip
        private Option GetAllOption()
        {
            return new Option(new string[] { "-a", "--all" }, "Lists all to do items including the completed ones.");
        }

        private Option GetNoStatusOption()
        {
            return new Option(new string[] { "--no-status" }, "Suppresses the bullet indicating whether the item is completed or not.");
        }
        */

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