using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Web.Helpers
{
    public static class TextExtensions
    {
        public static string ToTitleCase(this string value)
        {
            if (value == null || value.Length == 0)
                return value;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string FirstLetterToUpper(this string value)
        {
            if (value == null || value.Length == 0)
                return value;
            value = value.ToLower();
            value = value[0].ToString().ToUpper() + value.Substring(1);
            return value;
        }

        public static string ToAbbreviation(this string value)
        {
            if (value == null || value.Length == 0)
                return value;
            var words = value.Split(' ');
            words = words.Where(x => x.Length >= 4).ToArray();
            var abbv = words.Select(x => x.ToUpper().FirstOrDefault()).ToArray();
            
            return string.Join("", abbv);
        }
    }
}