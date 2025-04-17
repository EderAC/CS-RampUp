using System.Globalization;
using TaskManagerApp.Interfaces;
using TaskManagerApp.Models;

namespace TaskManagerApp.Utils{
public static class TaskMenuHelper{
    public static void MenuAddTask(ITaskManager taskManager){
        Console.WriteLine("\n--------ADD TASK------");
        Console.WriteLine("\nPlease, enter the task Name");
        var taskName = Console.ReadLine();

        Console.WriteLine("\nEnter the task Due Date");
        var dueDate = Console.ReadLine();

        if (string.IsNullOrEmpty(dueDate))
        {
            Console.WriteLine("\n**Due date cannot be empty. Try again**");
            StringHelper.WaitForUser();
            return;
        }

        if (!DateTime.TryParseExact(dueDate, ["dd/MM/yyyy"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDueDate))
        {
            Console.WriteLine("\n**Invalid date format. Please use dd/MM/yyyy.**");
            StringHelper.WaitForUser();
            return;
        }

        if(DateTime.Compare(DateTime.Now, parsedDueDate) > 0)
        {
            Console.WriteLine("\n**Due date cannot be in the past. Try again**");
            StringHelper.WaitForUser();
            return;
        }

        Console.WriteLine("\nEnter the task Priority (1.Low, 2.Medium, 3.High)");
        var priority = Console.ReadLine();

        if (priority != "1" && priority != "2" && priority != "3")
        {
            Console.WriteLine("Invalid or none priority selected. Defaulting to Low.");
        }

        TaskPriority taskPriority = priority switch
        {
            "2" => TaskPriority.Medium,
            "3" => TaskPriority.High,
            _ => TaskPriority.Low,
        };
        
        Console.WriteLine("\nPlease, enter the task Description");
        var taskDescription = Console.ReadLine();

        try
        {
            string[] formats = ["dd/MM/yyyy"];
            var parsedDate = DateHelper.ParseDate(dueDate, formats);

            var newUserTask = new UserTask(taskName, parsedDate, taskPriority)
            {
                Description = taskDescription
            };

            taskManager.AddTask(newUserTask);
            Console.WriteLine("Task added Succesfully");

        }
        catch (FormatException fEx)
        {
            Console.WriteLine($"Error: {fEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something unexpected happened: {ex.Message}");
        }

        StringHelper.WaitForUser();
    }

    public static void MenuGetAllTasks(ITaskManager taskManager){
        Console.WriteLine("\n--------VIEW TASKS------");
        List<UserTask> tasks = taskManager.GetAllTasks();
        if(tasks.Count == 0)
        {
            Console.WriteLine("\nNo tasks found.");
            StringHelper.WaitForUser();
            return;
        }
        Console.WriteLine("\nID | Name       | Description           | Due Date   | Status     | Priority");
        Console.WriteLine("---------------------------------------------------------------------------");
        tasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
        StringHelper.WaitForUser();

    }

    public static void MenuUpdateTaskStatus(ITaskManager taskManager){
        Console.WriteLine("\n--------UPDATE TASK STATUS------");
        try {
            Console.WriteLine("Write the task ID to update");
            var taskId = Console.ReadLine();

            if(!int.TryParse(taskId, out int taskIdInt))
            {
                Console.WriteLine("Invalid task ID.");
                StringHelper.WaitForUser();
                return;
            }

            Console.WriteLine($"Choose the new status: {string.Join(", ", 
                Enum.GetValues<Models.TaskStatus>()
                .Cast<Models.TaskStatus>()
                .Select((value, index) => $"{index + 1}. {value}"))}");
             
            var taskStatus = Console.ReadLine();

            var values = Enum.GetValues<Models.TaskStatus>().Cast<Models.TaskStatus>().ToList();
            Models.TaskStatus taskStatusEnum;
            if(int.TryParse(taskStatus, out int index) && index > 0 && index <= values.Count)
            {
                taskStatusEnum = values[index - 1];
                Console.WriteLine($"Task status updated to: {taskStatusEnum}");
            }
            else
            {
                Console.WriteLine("Invalid task status. Creating as ToDo.");
                taskStatusEnum = Models.TaskStatus.ToDo;
            }

            taskManager.UpdateTaskStatus(int.Parse(taskId), taskStatusEnum);

            StringHelper.WaitForUser();

        }catch (KeyNotFoundException kEx){
            Console.WriteLine($"Error: {kEx.Message}");
            StringHelper.WaitForUser();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something unexpected happened: {ex.Message}");
            StringHelper.WaitForUser();
        }
    }

    public static void MenuDeleteTask(ITaskManager taskManager){
        Console.WriteLine("\n--------DELETE TASK------");
        Console.WriteLine("Enter the **ID** of the task you want to delete:");
        var deleteId = Console.ReadLine();
        if (!int.TryParse(deleteId, out int taskId))
        {
            Console.WriteLine("Invalid task ID. Please enter a number.");
            StringHelper.WaitForUser();
            return;
        }
        Console.WriteLine($"Are you sure you want to delete task #{taskId}? (y/n)");
        var confirm = Console.ReadLine();
        if (confirm?.Trim().ToLower() != "y") return;

        try{
            taskManager.DeleteTask(taskId);
            Console.WriteLine($"Task {taskId} deleted successfully");
        }catch(KeyNotFoundException kEx){
            Console.WriteLine($"Error: {kEx.Message}");
            StringHelper.WaitForUser();
        }catch(Exception ex)
        {
            Console.WriteLine($"Something unexpected happened: {ex.Message}");
            StringHelper.WaitForUser();
        }
    }

    public static void MenuGetTasksByStatus(ITaskManager taskManager){
        Console.WriteLine("\n--------FILTER BY STATUS------");
        Console.WriteLine("Choose the status to filter by: ");
        var statuses = Enum.GetValues<Models.TaskStatus>().Cast<Models.TaskStatus>().ToList();
        Console.WriteLine(string.Join(", ", statuses.Select((s, i) => $"{i + 1}. {s}")));
        var statusInput = Console.ReadLine();
        if (!int.TryParse(statusInput, out int statusIndex) || statusIndex < 1 || statusIndex > statuses.Count)
        {
            Console.WriteLine("Invalid status selected.");
            StringHelper.WaitForUser();
            return;
        }
        var selectedStatus = statuses[statusIndex - 1];
        var filteredTasks = taskManager.GetTasksByStatus(selectedStatus);
        if (filteredTasks.Count == 0)
        {
            Console.WriteLine($"No tasks found with status {selectedStatus}.");
        }
        else
        {
            Console.WriteLine($"\nTasks with status {selectedStatus}:");
            Console.WriteLine("ID | Name       | Description           | Due Date   | Status     | Priority");
            Console.WriteLine("---------------------------------------------------------------------------");
            filteredTasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
        }
        StringHelper.WaitForUser();

    }

    public static void MenuGetTasksByPriority(ITaskManager taskManager){
        Console.WriteLine("\n--------FILTER BY PRIORITY------");
        Console.WriteLine("Choose the priority to filter by: ");
        var priorities = Enum.GetValues<TaskPriority>().Cast<TaskPriority>().ToList();
        Console.WriteLine(string.Join(", ", priorities.Select((p, i) => $"{i + 1}. {p}")));
        var priorityInput = Console.ReadLine();
        if (!int.TryParse(priorityInput, out int priorityIndex) || priorityIndex < 1 || priorityIndex > priorities.Count)
        {
            Console.WriteLine("Invalid priority selected.");
            StringHelper.WaitForUser();
            return;
        }
        var selectedPriority = priorities[priorityIndex - 1];
        var filteredTasks = taskManager.GetTasksByPriority(selectedPriority);
        if (filteredTasks.Count == 0)
        {
            Console.WriteLine($"No tasks found with priority {selectedPriority}.");
        }
        else
        {
            Console.WriteLine($"\nTasks with priority {selectedPriority}:");
            Console.WriteLine("ID | Name       | Description           | Due Date   | Status     | Priority");
            Console.WriteLine("---------------------------------------------------------------------------");
            filteredTasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
        }
        StringHelper.WaitForUser();

    }
}


}
