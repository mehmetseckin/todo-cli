namespace Todo.Core.Model;

public class TodoItem
{
    public string? Id { get; set; }
    public string? Subject { get; set; }
    public bool IsCompleted { get; set; }
    public string Status { get; set; } = "NotStarted";
    public DateTime? Completed { get; set; }
    public DateTime? Created { get; set; }
    public string? ListId { get; set; }

    public override string ToString() => $"{Subject} - {Status} {(IsCompleted ? Completed?.ToString("yyyy-MM-dd") : string.Empty)}";
    public string ToString(bool noStatus) => noStatus ? Subject : ToString();
}