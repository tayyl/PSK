using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Utils
    {
        public static string GenerateRandomText(this Random random, int length)
        {
            var answerString = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                answerString.Append((char)random.Next(99, 123));
            }
            return answerString.ToString();
        }
        public static byte[] TryConvertFromBase64(this string data)
        {
            try
            {
                return Convert.FromBase64String(data);
            }catch (Exception ex)
            {
                return null;
            }
        }
    }
}
