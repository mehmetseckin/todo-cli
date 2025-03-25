using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public class JsonOutputFormatter : IOutputFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public string FormatSuccess(string message)
    {
        var output = new { success = true, message };
        var json = JsonSerializer.Serialize(output, Options);
        Console.WriteLine(json);
        return json;
    }

    public string FormatError(string message)
    {
        var output = new { success = false, message };
        var json = JsonSerializer.Serialize(output, Options);
        Console.Error.WriteLine(json);
        return json;
    }

    public string FormatList(TodoList list, bool noStatus)
    {
        var output = new
        {
            list.Name,
            list.Count,
            Tasks = list.Tasks.Select(t => FormatItemToJson(t, noStatus))
        };
        var json = JsonSerializer.Serialize(output, Options);
        Console.WriteLine(json);
        return json;
    }

    public string FormatItem(TodoItem item, bool noStatus)
    {
        var json = JsonSerializer.Serialize(FormatItemToJson(item, noStatus), Options);
        Console.WriteLine(json);
        return json;
    }

    public string FormatItems(IEnumerable<TodoItem> items, bool noStatus)
    {
        var output = items.Select(item => FormatItemToJson(item, noStatus));
        var json = JsonSerializer.Serialize(output, Options);
        Console.WriteLine(json);
        return json;
    }

    private static object FormatItemToJson(TodoItem item, bool noStatus)
    {
        return new
        {
            item.Id,
            item.Subject,
            item.IsCompleted,
            item.Status,
            Completed = item.Completed?.ToString("yyyy-MM-dd"),
            item.ListId,
            DisplayText = noStatus ? item.Subject : item.ToString()
        };
    }
} 