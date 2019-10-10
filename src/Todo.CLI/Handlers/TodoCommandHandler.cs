using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace Todo.CLI.Handlers
{
    public class TodoCommandHandler
    {
        public static ICommandHandler Create()
        {
            return CommandHandler.Create<bool>((version) =>
            {
                if (version)
                {
                    PrintVersion();
                    return;
                }
            });
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
