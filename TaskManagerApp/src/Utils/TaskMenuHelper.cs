using System.Globalization;
using TaskManagerApp.Interfaces;
using TaskManagerApp.Models;

namespace TaskManagerApp.Utils
{
    public static class TaskMenuHelper
    {
        private static T PromptEnumChoice<T>(string prompt) where T : struct, Enum
        {
            var options = Enum.GetValues<T>().Cast<T>().ToList();
            Console.WriteLine(prompt);
            Console.WriteLine(string.Join(", ", options.Select((option, index) => $"{index + 1}. {option}")));

            var input = Console.ReadLine();
            if(int.TryParse(input, out int index) && index >= 1 && index <= options.Count)
                return options[index - 1];
                
            Console.WriteLine($"Invalid {typeof(T).FullName} selected. Defaulting to {options[0]}.");
            StringHelper.WaitForUser();
            return options[0];
        }
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
            StringHelper.DisplayTaskHeader();
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
                
                var selectedStatus = PromptEnumChoice<Models.TaskStatus>("Choose the new status:");

                taskManager.UpdateTaskStatus(int.Parse(taskId), selectedStatus);
                Console.WriteLine($"Task {taskId} status updated to {selectedStatus}.");

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
            if(!StringHelper.ConfirmationDialog($"Are you sure you want to delete task #{taskId}?"))
                return;
            

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
            var selectedStatus = PromptEnumChoice<Models.TaskStatus>("Choose the status to filter by:");

            var filteredTasks = taskManager.GetTasksByStatus(selectedStatus);
            if (filteredTasks.Count == 0)
            {
                Console.WriteLine($"No tasks found with status {selectedStatus}.");
            }
            else
            {
                Console.WriteLine($"\nTasks with status {selectedStatus}:");
                StringHelper.DisplayTaskHeader();
                filteredTasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
            }
            StringHelper.WaitForUser();

        }

        public static void MenuGetTasksByPriority(ITaskManager taskManager){
            Console.WriteLine("\n--------FILTER BY PRIORITY------");
            var selectedPriority = PromptEnumChoice<TaskPriority>("Choose the priority to filter by: ");

            var filteredTasks = taskManager.GetTasksByPriority(selectedPriority);
            if (filteredTasks.Count == 0)
            {
                Console.WriteLine($"No tasks found with priority {selectedPriority}.");
            }
            else
            {
                Console.WriteLine($"\nTasks with priority {selectedPriority}:");
                StringHelper.DisplayTaskHeader();
                filteredTasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
            }
            StringHelper.WaitForUser();

        }
    
        public static void MenuGetNextTasks(ITaskManager taskManager){
            Console.WriteLine("\n--------VIEW NEXT TASKS------");
            try
            {
                var nextTasks = taskManager.GetNextTasks();
                Console.WriteLine("Upcoming tasks:");
                StringHelper.DisplayTaskHeader();
                nextTasks.ForEach(t => Console.WriteLine($"{t.Id,-3}| {t.Name,-10}| {t.Description,-20}| {t.DueDate:dd/MM/yyyy} | {t.Status,-10}| {t.Priority}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something unexpected happened: {ex.Message}");
            }
            StringHelper.WaitForUser();
        }
    }


}
