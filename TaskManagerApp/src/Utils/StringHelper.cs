namespace TaskManagerApp.Utils
{
    public static class StringHelper
    {
        public static void WaitForUser(){
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        public static bool ConfirmationDialog(string text){
            Console.WriteLine($"{text} (y/n)");
            var confirm = Console.ReadLine();
            return confirm?.Trim().ToLower() == "y";
        }

        public static void DisplayTaskHeader(){
            Console.WriteLine("ID | Name       | Description           | Due Date   | Status     | Priority");
            Console.WriteLine("---------------------------------------------------------------------------");
        }
        
        public static string GetString(string message, string defaultValue)
        {
            Console.WriteLine(message);
            var input = Console.ReadLine();
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }
    }
}