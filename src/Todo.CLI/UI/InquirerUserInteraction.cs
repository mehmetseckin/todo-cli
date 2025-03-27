using System;
using InquirerCS;
using System.Collections.Generic;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public class InquirerUserInteraction : IUserInteraction
{
    private readonly IOutputFormatter _outputFormatter;

    public InquirerUserInteraction(OutputFormat format)
    {
        _outputFormatter = format switch
        {
            OutputFormat.Interactive => new InteractiveOutputFormatter(),
            OutputFormat.Json => new JsonOutputFormatter(),
            _ => throw new ArgumentException($"Unsupported output format: {format}", nameof(format))
        };
    }

    public IOutputFormatter OutputFormatter => _outputFormatter;

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
        _outputFormatter.FormatError(message);
    }

    public void ShowSuccess(string message)
    {
        _outputFormatter.FormatSuccess(message);
    }
} 