
namespace TaskManagerApp.Models
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed,
        Canceled
    }
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
    public class UserTask(string name, DateTime dueDate, TaskPriority priority)
    {

        public int Id { get; set; }
        public string Name { get; set; } = name;
        public string? Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = dueDate;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = priority;
    }

}