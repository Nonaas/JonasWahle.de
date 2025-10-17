namespace JonasWahle.de.Domain.Utilities
{
    public class Extensions
    {
        public static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
