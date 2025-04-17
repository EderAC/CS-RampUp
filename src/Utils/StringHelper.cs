namespace TaskManagerApp.Utils
{
    public static class StringHelper
    {
        public static void WaitForUser(){
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
        
        public static string GetString(string message, string defaultValue)
        {
            Console.WriteLine(message);
            var input = Console.ReadLine();
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }
    }
}