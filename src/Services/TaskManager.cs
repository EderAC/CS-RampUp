using TaskManagerApp.Interfaces;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services{
    
    public class TaskManager : ITaskManager
    {
        private readonly ITaskFileStorage _storage;
        private readonly List<UserTask> _tasks;

        public TaskManager(ITaskFileStorage storage)
        {
            _storage = storage;
            _tasks = _storage.LoadTasksFromFile();
        }

        public void AddTask(UserTask task)
        {
            try
            {
                task.Id = _tasks.Count != 0 ? _tasks.Max(task => task.Id) + 1 : 1;
                _tasks.Add(task);
                _storage.SaveTasksToFile(_tasks);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public bool DeleteTask(int taskId)
        {
            try{
                var task = _tasks.FirstOrDefault(t => t.Id == taskId) ?? throw new KeyNotFoundException("Task not found");
                _tasks.Remove(task);
                _storage.SaveTasksToFile(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateTaskStatus(int taskId, Models.TaskStatus status)
        {
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId) ?? throw new KeyNotFoundException("Task not found");
                task.Status = status;
                _storage.SaveTasksToFile(_tasks);
                return true;
                
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public List<UserTask> GetAllTasks() => [.. _tasks];

        public List<UserTask> GetTasksByStatus(Models.TaskStatus status)
        {
            return [.. _tasks.Where(t => t.Status == status)];
        }

        public List<UserTask> GetTasksByPriority(TaskPriority priority)
        {
            return [.. _tasks.Where(t => t.Priority == priority)];
        }
        public List<UserTask> GetTodayTasks()
        {
            return [.. _tasks.Where(t => t.DueDate.Date == DateTime.Today)];
        }

    }
}