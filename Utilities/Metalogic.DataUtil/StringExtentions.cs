using System.Collections.Generic;
using System.Text;

namespace Metalogic.DataUtil
{
    public static class StringExtentions
    {
        public static string SplitByUpperCase(this string data)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < data.Length; ++i)
            {
                var c = data[i];

                if (i != 0 && char.IsUpper(c) && (!char.IsUpper(data[i - 1]) || char.IsLower(data[i < data.Length-1 ? i + 1 : i])))
                {
                    builder.Append(' ');
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string ToLegalFileName(this string data)
        {
            var builder = new StringBuilder();

            var set = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());

            foreach (var c in data)
            {
                builder.Append(set.Contains(c) ? '_' : c);
            }
            return builder.ToString();
        }
    }
}
