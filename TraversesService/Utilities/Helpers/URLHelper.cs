using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TraversesService.Utilities.Helpers
{
    public static class URLHelper
    {
        public static bool ValidateUrl(string url)
        {
            bool result = false;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                result = (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            //string pattern = @"^(http|https)://([a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3})(/S*)?$";
            //Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //result = regex.IsMatch(url);

            return result;
        }

        public static string GetFileNameFromUrl(string url)
        {
            string result = string.Empty;

            if (ValidateUrl(url))
            {
                Uri uri = new Uri(url);
                result = System.IO.Path.GetFileName(uri.LocalPath);

            }

            return result;
        }

    }
}
