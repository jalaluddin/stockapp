using System.Data.SqlTypes;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace StockMarketSharedLibrary
{
    public static class CommonUtility
    {
        public static string PascalToSpaceSeperated(string pascalCaseString)
        {
            StringBuilder spaceSeperatedString = new StringBuilder();
            bool smallCaseFound = false;
            foreach (char c in pascalCaseString.ToCharArray())
            {
                if (char.IsLower(c))
                {
                    smallCaseFound = true;
                }
                else
                {
                    if (smallCaseFound)
                        spaceSeperatedString = spaceSeperatedString.Append(' ');
                }
                spaceSeperatedString = spaceSeperatedString.Append(c);
            }
            return spaceSeperatedString.ToString();
        }

        public static object FilterNull(object value, Type type)
        {
            if (value == DBNull.Value)
            {
                if (type == typeof(int))
                    return 0;
                else if (type == typeof(double))
                    return 0.0;
                else if (type == typeof(string))
                    return string.Empty;
                else if (type == typeof(DateTime))
                    return null;
                else if (type == typeof(decimal))
                    return 0;
                else if (type == typeof(bool))
                    return false;
                else
                    return value;
            }
            else
                return value;
        }

        public static string FormatedDateString(DateTime dateTime)
        {
            if (dateTime == SqlDateTime.MinValue.Value)
                return string.Empty;
            else
                return dateTime.ToString("dd/MM/yyyy");
        }

        public static string FormatedDateString(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;
            else
                return dateTime.Value.ToString("dd/MM/yyyy");
        }

        public static string FormatedDateTimeString(DateTime dateTime)
        {
            if (dateTime == SqlDateTime.MinValue.Value)
                return string.Empty;
            else
                return string.Format("{0}M",dateTime.ToString("dd/MM/yyyy hh:mm t"));
        }

        public static string FormatedDateTimeString(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;
            else
                return string.Format("{0}M", dateTime.Value.ToString("dd/MM/yyyy hh:mm t"));
        } 

        public static string MakeAlphaNumeric(string text)
        {
            StringBuilder filteredText = new StringBuilder();
            StringBuilder newText = new StringBuilder(text);
            for (int i = 0; i < newText.Length; i++)
            {
                if (char.IsLetterOrDigit(newText[i]))
                    filteredText = filteredText.Append(newText[i]);
            }
            return filteredText.ToString();
        }

        public static String MakeCSV(IList<Int32> items)
        {
            StringBuilder result = new StringBuilder(String.Empty);

            for (int i = 0; i < items.Count; i++)
            {
                if (i != 0)
                    result = result.Append(",").Append(items[i]);
                else
                    result = result.Append(items[i]);
            }

            return result.ToString();
        }

        public static String MakeCSV(IList<String> items)
        {
            StringBuilder result = new StringBuilder(String.Empty);

            for (int i = 0; i < items.Count; i++)
            {
                if (i != 0)
                    result = result.Append(",").Append(items[i]);
                else
                    result = result.Append(items[i]);
            }

            return result.ToString();
        }

        public static IList<String> SplitCSV(String csv)
        {
            List<String> result = new List<String>();
            String[] parts = csv.Split(',');

            foreach (String part in parts)
            {
                result.Add(part);
            }

            return result;
        }

        public static IList<Int32> SplitCSVToInt(String csv)
        {
            List<Int32> result = new List<Int32>();
            String[] parts = csv.Split(',');

            foreach (String part in parts)
            {
                result.Add(Int32.Parse(part));
            }

            return result;
        }

        /// <summary>
        /// Shortens a long text according to desired length
        /// and ends the text with ... to indicate continual.
        /// If the text is shorter than maxLength, then the 
        /// original text is returned
        /// </summary>
        /// <param name="text">The long text to be shorten</param>
        /// <param name="maxLength">Maximum length of the shorter text</param>
        /// <returns></returns>
        public static string ShortenText(string text, int maxLength)
        {
            StringBuilder newText = new StringBuilder();
            if (text.Length > maxLength)
            {
                newText.Append(text.Substring(0, maxLength - 3));
                newText.Append("...");
                return newText.ToString();
            }
            else
                return text;
        }

        /// <summary>
        /// Checks whether the image exists or not, can be used
        /// in conjunction of a method that replaces the image
        /// path with a default image path if this method return
        /// false, meaning the image is missing
        /// </summary>
        /// <param name="imagePath">Path of the image to be checked</param>
        /// <returns>true if the image is found, false otherwise</returns>
        public static bool ValidateImagePath(string imagePath)
        {
            bool result = true;
            if (string.IsNullOrEmpty(imagePath))
                result = false;

            if (result)
            {
                string filePath = HttpContext.Current.Server.MapPath(imagePath);
                if (!File.Exists(filePath))
                    result = false;
            }

            return result;
        }

        public static string GetDomainName()
        {
            //return System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return "127.0.0.1";
            else
            {
                string domain = HttpContext.Current.Request.Url.Authority + (HttpContext.Current.Request.ApplicationPath ?? string.Empty).TrimEnd('/');
                if (domain.Contains(":"))
                {
                    string[] parts = domain.Split(new char[] { ':' });
                    domain = parts[0];
                }
                return domain;
            }
        }

        public static string GetBaseUrl()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return "http://127.0.0.1/";
            else
                return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + (HttpContext.Current.Request.ApplicationPath ?? string.Empty).TrimEnd('/') + '/';
        }

        public static TimeSpan ConvertTimeZoneToTimeSpan(string timeZoneName)
        {
            if (!string.IsNullOrEmpty(timeZoneName) && timeZoneName != "-1")
            {
                string timeOffset = timeZoneName.Substring(timeZoneName.IndexOf("(GMT") + 4, 6);
                string[] parts = timeOffset.Split(':');
                return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
            }
            else
                return new TimeSpan();
        }

        public static string GenerateThemedImageUrl(string imageName, string currentTheme)
        {
            if (!imageName.Contains("~/App_Themes/"))
                imageName = "~/App_Themes/" + currentTheme + "/images/" + imageName;
            return imageName;
        }

        public static string GetUserIPAddress()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return "127.0.0.1";
            else
            {
                //return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                string VisitorsIPAddr = string.Empty;
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
                }
                return VisitorsIPAddr;
            }
        }

        public static int GenerateRandomNumber(int howManyDigitToUse)
        {
            if (howManyDigitToUse > 0)
            {
                int min = (int)Math.Pow(10, howManyDigitToUse - 1);
                int max = (int)Math.Pow(10, howManyDigitToUse) - 1;
                return GenerateRandomNumber(min, max);
            }
            else
                return 0;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int GenerateRandomNumber(int min, int max)
        {
            return new Random(DateTime.Now.Millisecond).Next(min, max);
        }

        public static HtmlLink CreateStylesheetLink(string url)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes.Add("href", url);
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            return link;
        }

        public static string MakeUrlFriendlyText(string plainText)
        {
            StringBuilder work = new StringBuilder(plainText.ToLower());

            for (int i = 0; i < work.Length; i++)
            {
                if (!Char.IsLetterOrDigit(work[i]))
                    work[i] = '_';

                work.Replace("__", "_");
            }

            string result = work.ToString();

            result = result.Trim('_');

            return result;
        }
    }
}
