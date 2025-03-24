using System;
using InquirerCS;
using System.Collections.Generic;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public class InquirerUserInteraction : IUserInteraction
{
    public IEnumerable<TodoItem> SelectItems(string message, IEnumerable<TodoItem> items)
    {
        return Question
            .Checkbox(message, items)
            .Page(50)
            .Prompt();
    }

    public bool Confirm(string message)
    {
        return Question.Confirm(message).Prompt();
    }

    public void ShowError(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public void ShowSuccess(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void ClearScreen()
    {
        Console.Clear();
    }
} 