using System.Text.RegularExpressions;

namespace Logic
{
    public class DataValidator
    {
        public static bool IsValid(string str, Regex regex)
        {
            bool ret = false;
            if (str != null)
            {
                if (regex.IsMatch(str))
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}