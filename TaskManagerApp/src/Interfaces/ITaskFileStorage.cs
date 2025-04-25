using TaskManagerApp.Models;

namespace TaskManagerApp.Interfaces
{
    public interface ITaskFileStorage
    {
        void SaveTasksToFile(List<UserTask> tasks);
        List<UserTask> LoadTasksFromFile();
    }    
}