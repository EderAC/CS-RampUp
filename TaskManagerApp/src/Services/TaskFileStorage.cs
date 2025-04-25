using TaskManagerApp.Interfaces;
using TaskManagerApp.Models;
using System.Text.Json;

namespace TaskManagerApp.Services{
    public class TaskFileStorage : ITaskFileStorage
    {
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, options);
        }
        private readonly string filePath;

        public TaskFileStorage(string filePath = "tasks.json")
        {
            this.filePath = filePath;
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public void SaveTasksToFile(List<UserTask> tasks)
        {
            string jsonString = Serialize(tasks);
            File.WriteAllText(filePath, jsonString);
        }

        public List<UserTask> LoadTasksFromFile()
        {
            string userTasksJson = File.ReadAllText(filePath);
            return string.IsNullOrEmpty(userTasksJson)
                ? []
            : JsonSerializer.Deserialize<List<UserTask>>(userTasksJson) ?? [];
        }
    }
}