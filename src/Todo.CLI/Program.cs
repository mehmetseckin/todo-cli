using Todo.CLI.Commands;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace Todo.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            return new TodoCommand()
                .InvokeAsync(args)
                .Result;
        }
    }
}
