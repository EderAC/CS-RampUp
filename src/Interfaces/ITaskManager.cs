using TaskManagerApp.Models;

namespace TaskManagerApp.Interfaces
{
    public interface ITaskManager
    {
        void AddTask(UserTask task);
        bool DeleteTask(int taskId);
        bool UpdateTaskStatus(int taskId, Models.TaskStatus status);
        List<UserTask> GetAllTasks();
        List<UserTask> GetTasksByStatus(Models.TaskStatus status);
        List<UserTask> GetTasksByPriority(TaskPriority priority);
        List<UserTask> GetTodayTasks();
    }
}