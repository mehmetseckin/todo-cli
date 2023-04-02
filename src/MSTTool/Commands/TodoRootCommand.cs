using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MSTTool.Commands;
using Microsoft.Graph.Models.TermStore;

namespace Todo.MSTTool.Commands
{
    public class TodoRootCommand : RootCommand
    {
        // HACK: drp033123 - Not all Commands use the FolderOption, but including it as a GlobalOption has the advntage of it automatically appearing in the help display.
        //  We expose it as a static so child commands can access the Option object for SetHandler
        public static Option<string> FolderOption = new Option<string>("-folder", "Set the output folder. Can be enclosed in double quotes.");

        public TodoRootCommand(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<MSTConfiguration>();

            // Add static parameters
            Description = "A CLI to work with Microsoft ToDo items.";

            // TEST: drp032223 - seems "--version" is built in.
            //AddOption(GetVersionOption());

            // TODO: drp033123 - cleaner to AddOption() to base Command class, but then needs to appear in help.
            this.AddGlobalOption(FolderOption);

            // this is the handler that runs if no other command is specified
            this.SetHandler((context) =>
            {
                PrintVersion();
            });

            // Add subcommands
            AddCommand(new ListCommand(serviceProvider));
            AddCommand(new ListsCommand(serviceProvider));
            AddCommand(new ExportCommand(serviceProvider));
            AddCommand(new SyncCommand(serviceProvider));
            if (config.SupportsWrite)
            {
                throw new NotImplementedException();
                /* USE_WRITECOMMANDS
                AddCommand(new AddCommand(serviceProvider));
                AddCommand(new CompleteCommand(serviceProvider));
                AddCommand(new RemoveCommand(serviceProvider));
                */
            }
        }

        public void PrintVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var entryAssemblyName = entryAssembly.GetName();
            var description = entryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            Console.WriteLine($"{entryAssemblyName.Name} {entryAssemblyName.Version} - {description}");
            Console.WriteLine("\tUse -h to see commands");
        }

        /* UNUSED
        private Option GetVersionOption()
        {
            return new Option<string>(new string[] { "-v", "--version" }, "Prints out the CLI version.");
        }
        */
    }
}
