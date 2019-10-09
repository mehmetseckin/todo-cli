using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace Microsoft.Todo.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            // Define the root command
            var rootCommand = new RootCommand
            {
                new Option(new string[] { "-v", "--version" } , "Prints out the todo CLI version.")
                {
                    Argument = new Argument<bool>()
                }
            };

            rootCommand.Description = "A CLI to manage to do items.";

            // Define handler
            rootCommand.Handler = CommandHandler.Create<bool>((version) =>
            {
                if (version)
                {
                    PrintVersion();
                    return;
                }
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void PrintVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var entryAssemblyName = entryAssembly.GetName();
            var description = entryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            Console.WriteLine($"{entryAssemblyName.Name} {entryAssemblyName.Version} - {description}");
        }
    }
}
