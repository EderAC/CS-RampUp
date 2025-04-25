using System.Globalization;

namespace TaskManagerApp.Utils
{

    public static class DateHelper
    {
        public static DateTime ParseDate(string dateString, string[] formats)
        {
            if (DateTime.TryParseExact(dateString, formats, new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            throw new Exception("\nInvalid date format. Please use MM/dd/yyyy.");
        }
    }
}
