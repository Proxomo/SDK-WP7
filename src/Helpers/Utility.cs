using System;
//using System.Web; //TODO 123
using System.Text;
using System.Net;

namespace Proxomo
{
    internal class Utility
    {

        internal static double GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(ts.TotalSeconds);
        }

        internal static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        internal static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        internal static string FormatQueryString(string address = "", string latitude = "", string longitude = "", string q = "", string category = "", double radius = 25, LocationSearchScope scope = LocationSearchScope.All, int maxResults = 25, string personID = "")
        {
            StringBuilder sbuilder = new StringBuilder();

            sbuilder.Append("?");
            if (!(String.IsNullOrEmpty(address) || address.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&address={0}", HttpUtility.UrlEncode(address));
            }

            /*if (! (string.IsNullOrWhiteSpace(address))) {
                sbuilder.AppendFormat("&address={0}", HttpUtility.UrlEncode(address));
            }*/

            if (!(String.IsNullOrEmpty(latitude) || latitude.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&latitude={0}", HttpUtility.UrlEncode(latitude));
            }

            if (!(String.IsNullOrEmpty(longitude) || longitude.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&longitude={0}", HttpUtility.UrlEncode(longitude));
            }

            if (!(String.IsNullOrEmpty(q) || q.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&q={0}", HttpUtility.UrlEncode(q));
            }

            if (!(String.IsNullOrEmpty(category) || category.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&category={0}", HttpUtility.UrlEncode(category));
            }

            if (radius > 0)
            {
                sbuilder.AppendFormat("&radius={0}", Convert.ToInt16(radius));
            }

            if (scope > 0)
            {
                sbuilder.AppendFormat("&scope={0}", Convert.ToInt16(scope));
            }

            if (maxResults > 0)
            {
                sbuilder.AppendFormat("&maxresults={0}", Convert.ToInt16(maxResults));
            }

            if (!(String.IsNullOrEmpty(personID) || personID.Trim().Length == 0))
            {
                sbuilder.AppendFormat("&personid={0}", HttpUtility.UrlEncode(personID));
            }

            if (sbuilder.Length > 1)
            {
                return sbuilder.ToString().Replace("?&", "?");
            }
            else
            {
                sbuilder = null;
                return string.Empty;
            }

        }

    }
}