using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnkaCMS.Core.Helpers
{
    public static class StringHelper
    {
        public static string ClearForHtml(this string str)
        {
            return Regex.Replace(str, @"<[^>]+>|&nbsp;", "").Trim();
        }

        public static string ListToString(List<string> list, string delimeter)
        {
            return string.Join(delimeter, list.ToArray());

        }

        public static string TemplateParser(string templateText, string regExString, string value)
        {
            var regExToken = new Regex(regExString, RegexOptions.IgnoreCase);
            return regExToken.Replace(templateText, value);
        }
    }
}
