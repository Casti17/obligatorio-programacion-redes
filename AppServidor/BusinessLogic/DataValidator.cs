using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AppServidor.BusinessLogic
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
