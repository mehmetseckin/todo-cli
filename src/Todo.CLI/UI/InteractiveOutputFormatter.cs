using System;
using System.Collections.Generic;
using System.Text;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public class InteractiveOutputFormatter : IOutputFormatter
{
    private const char TodoBullet = '-';
    private const char CompletedBullet = '\u2713'; // Sqrt - check mark

    public string FormatSuccess(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
        return message;
    }

    public string FormatError(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
        return message;
    }

    public string FormatList(TodoList list, bool noStatus)
    {
        var output = new StringBuilder();
        output.AppendLine($"{list.Name} ({list.Count}):");
        foreach (var item in list.Tasks)
        {
            output.Append(FormatItem(item, noStatus));
        }
        Console.Write(output.ToString());
        return output.ToString();
    }

    public string FormatItem(TodoItem item, bool noStatus)
    {
        var output = new StringBuilder();
        if (!noStatus)
        {
            var bulletColor = item.IsCompleted ? ConsoleColor.Green : ConsoleColor.Red;
            var bullet = item.IsCompleted ? CompletedBullet : TodoBullet;
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = bulletColor;
            Console.Write(bullet);
            Console.ForegroundColor = previousColor;
            Console.Write(" ");
        }
        var itemText = item.ToString(noStatus);
        Console.WriteLine(itemText);
        output.AppendLine(itemText);
        return output.ToString();
    }

    public string FormatItems(IEnumerable<TodoItem> items, bool noStatus)
    {
        var output = new StringBuilder();
        foreach (var item in items)
        {
            output.Append(FormatItem(item, noStatus));
        }
        return output.ToString();
    }
} 