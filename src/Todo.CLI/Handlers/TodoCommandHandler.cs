using System;
using System.Reflection;

namespace Todo.CLI.Handlers;

public class TodoCommandHandler
{
    public static Action<bool> Create()
    {
        return version =>
        {
            if (version) PrintVersion();
        };
    }

    private static void PrintVersion()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var entryAssemblyName = entryAssembly.GetName();
        var description = entryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        Console.WriteLine($"{entryAssemblyName.Name} {entryAssemblyName.Version} - {description}");
    }
}