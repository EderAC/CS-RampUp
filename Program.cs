using TaskManagerApp.Interfaces;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

ITaskFileStorage storage = new TaskFileStorage();
ITaskManager taskManager = new TaskManager(storage);
var option = "";

do
{
    Console.WriteLine("\n--------TASK MANAGER------");
    Console.WriteLine("1. Add task");
    Console.WriteLine("2. View Tasks");
    Console.WriteLine("3. Update Task Status");
    Console.WriteLine("4. View Tasks By Status");
    Console.WriteLine("5. View Tasks By Priority");
    Console.WriteLine("6. Delete Task");
    Console.WriteLine("7. Exit");
    Console.WriteLine("Choose an option:");
    option = Console.ReadLine();

    switch (option)
    {
        case "1":
            TaskMenuHelper.MenuAddTask(taskManager);
            break;

        case "2":
            TaskMenuHelper.MenuGetAllTasks(taskManager);
            break;
        
        case "3":
            TaskMenuHelper.MenuUpdateTaskStatus(taskManager);
            break;
        
        case "4":
            TaskMenuHelper.MenuGetTasksByStatus(taskManager);
            break;
        
        case "5":
            TaskMenuHelper.MenuGetTasksByPriority(taskManager);
            break;

        case "6":
            TaskMenuHelper.MenuDeleteTask(taskManager);
            break;

        case "7":
            Console.WriteLine("Exiting the program...");
            break;

        default:
            Console.WriteLine("Wrong option selected, please try again");
            break;
    }
} while (option != "8");

