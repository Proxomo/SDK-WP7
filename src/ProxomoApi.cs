using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;

using System.Collections.Generic;

namespace Proxomo
{


    // DELETE.... This delegate is used to receive the actual event handler (callback) that the Top App wants to call
    // when initialization is complete. This is necessary since the user cannot register to listen until AFTER the 
    // SDK instance is created (constructor). Therefore, we need to register it for him within the constructor -- AFTER the 
    // SDK instance is created and BEFORE we make the call to init (to get the token).
    //public delegate void UserInitCompleteHandler(object value);

    // DELETE... This delegate is used INTERNALLY within this SDK to implement callbacks to raise appropriate events
    // that the use can register to listen to for each Proxomo method
    //public delegate void ProxomoCallback(object value);

    public delegate void ProxomoUserCallbackDelegate<T>(ItemCompletedEventArgs<T> results);     
     
    public partial class ProxomoApi
    {
        internal static string _applicationID = string.Empty;
        internal static string _proxomoAPIKey = string.Empty;
        internal static string baseURL = string.Empty;
        private string contentType = "application/json";

        public string ApplicationID
        {
            get
            {
                return _applicationID;
            }
        }
        public string ProxomoAPIKey
        {
            get
            {
                return _proxomoAPIKey;
            }
        }

        private string _APIVersion = "v09";
        public string APIVersion
        {
            get
            {
                return _APIVersion;
            }
            set
            {
                _APIVersion = value;
            }
        }
        static private Token _AuthToken = new Token();
        public Token AuthToken
        {
            get
            {
                return _AuthToken;
            }
            set
            {
                _AuthToken = value;
            }
        }
        private bool _ValidateSSLCert = true;
        public bool ValidateSSLCert
        {
            get
            {
                return _ValidateSSLCert;
            }
            set
            {
                _ValidateSSLCert = value;
            }
        }
        private CommunicationType _Format = CommunicationType.JSON;
        public CommunicationType Format
        {
            get
            {
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }

        #region SDK Constructors and AuthToken management

        public ProxomoApi(string applicationID, string proxomoAPIKey,CommunicationType format = CommunicationType.JSON, bool validatessl = true, string url = "")
        {
            // We will pass into Init a delegate callback to call when the Init operation is complete.
            // The reason is that we need to set the private fields we use to store both the Auth Token value as well as its expiration date before returning back to user
            ProxomoUserCallbackDelegate<Token> InitComplete_Callback = new ProxomoUserCallbackDelegate<Token>(InitializationReady);
            Init(applicationID, proxomoAPIKey, "v09", format, validatessl, url, InitComplete_Callback);
        }
        public ProxomoApi(string applicationID, string proxomoAPIKey, string version, CommunicationType format = CommunicationType.JSON, bool validatessl = true, string url = "")
        {
            // We will pass into Init a delegate callback to call when the Init operation is complete.
            // The reason is that we need to set the private fields we use to store both the Auth Token value as well as its expiration date before returning back to user
            ProxomoUserCallbackDelegate<Token> InitComplete_Callback = new ProxomoUserCallbackDelegate<Token>(InitializationReady);
            Init(applicationID, proxomoAPIKey, version, format, validatessl, url, InitComplete_Callback);
        }

        private void Init(string applicationID, string proxomoAPIKey, string version, CommunicationType format, bool validatessl, string url, ProxomoUserCallbackDelegate<Token> initCompleteCallback)
        {
            APIVersion = version;
            _applicationID = applicationID;
            _proxomoAPIKey = proxomoAPIKey;
            this.ValidateSSLCert = validatessl;
            this.Format = format;

            if (format == CommunicationType.XML)
            {
                contentType = "text/xml";

                if ((String.IsNullOrEmpty(url) || url.Trim().Length == 0))  // Not supported for WP7: (String.IsNullOrWhiteSpace(url))
                {
                    baseURL = String.Format("https://service.proxomo.com/{0}/xml", APIVersion);
                }
                else
                {
                    baseURL = String.Format("{0}/{1}/xml", url, APIVersion);
                }

            }
            else if (format == CommunicationType.JSON)
            {
                contentType = "application/json";

                if ((String.IsNullOrEmpty(url) || url.Trim().Length == 0))
                {
                    baseURL = string.Format("https://service.proxomo.com/{0}/json", APIVersion);
                }
                else
                {
                    baseURL = String.Format("{0}/{1}/json", url, APIVersion);
                }
            }

            GetAuthToken(initCompleteCallback);
        }

        private void GetAuthToken(ProxomoUserCallbackDelegate<Token> initCompleteCallback)
        {
            string url = string.Format("{0}/security/accesstoken/get?applicationid={1}&proxomoAPIKey={2}", baseURL, HttpUtility.UrlEncode(_applicationID), HttpUtility.UrlEncode(_proxomoAPIKey));

            using (ProxomoWebRequest<Token> p = new ProxomoWebRequest<Token>(ValidateSSLCert, Format))
            {
                // We will pass into Init a delegate callback to call when the Init operation is complete.
                // The reason is that we need to set the private fields we use to store both the Auth Token value as well as its expiration date before returning back to user
                p.GetDataItem(url, "GET", contentType, "", initCompleteCallback);
                // add timeout handling... 
            }
        }
        public void RefreshAuthToken()
        {
            // We will pass into Init a delegate callback to call when the Init operation is complete.
            // The reason is that we need to set the private fields we use to store both the Auth Token value as well as its expiration date before returning back to user
            ProxomoUserCallbackDelegate<Token> InitComplete_Callback = new ProxomoUserCallbackDelegate<Token>(InitializationReady);
            GetAuthToken(InitComplete_Callback);
        }
        public bool IsAuthTokenExpired()
        {
            if (DateTime.Now < AuthToken.ExpiresDate)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
 
        private void InitializationReady(ItemCompletedEventArgs<Token> e)
        {
            _AuthToken = (Token)e.Result;
            AuthToken.ExpiresDate = Utility.ConvertFromUnixTimestamp(AuthToken.Expires);
        }

        #region Proxomo Methods
        #region AppData Methods

        public void AppDataAdd(AppData appData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(appData, Format, false), userCallback);
            }
        }
        public void AppDataDelete(string appDataID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/appdata/{1}", baseURL, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }
        public void AppDataGet(string appDataID, ProxomoUserCallbackDelegate<AppData> userCallback)
        {
            string url = string.Format("{0}/appdata/{1}", baseURL, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void AppDataGetAll(ProxomoUserCallbackDelegate<List<AppData>> userCallback)
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void AppDataUpdate(AppData appData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(appData, Format, false), userCallback);
            }
        }
        public void AppDataSearch(string objectType, ProxomoUserCallbackDelegate<List<AppData>> userCallback)
        {
            string url = string.Format("{0}/appdata/search/objecttype/{1}", baseURL, objectType);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }

        #endregion

        #region CustomDataStorage

        public void CustomDataAdd<T>(T data, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/customdata", baseURL);
                
            ContinuationTokens cTokens = new ContinuationTokens("", "");
            
            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(data, Format, false), userCallback);
            }
        }
        public void CustomDataUpdate<T>(T data, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/customdata", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(data, Format, false), userCallback);
            }
        }
        public void CustomDataDelete(string tableName, string customDataID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/customdata/table/{1}/{2}", baseURL, tableName, customDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }
        //public void CustomDataSearch<T>(string tableName, string query, int maxResults, ref ContinuationTokens cTokens)
        //{
        //    string url = string.Format("{0}/customdata/search/table/{1}?q={2}&maxresults={3}", baseURL, tableName, query, maxResults);

        //    using (ProxomoWebRequest<object> p = new ProxomoWebRequest<object>(AuthToken.AccessToken, ValidateSSLCert, Format))
        //    {
        //        //p.GetDataItem("", "", "", "", CustomDataSearchReady, ref cTokens);
        //    }

        //}
        public void CustomDataSearch<T>(string tableName, string query, int maxResults, ProxomoUserCallbackDelegate<T> userCallback, ref ContinuationTokens cTokens)
        {
            string url = string.Format("{0}/customdata/search/table/{1}?q={2}&maxresults={3}", baseURL, tableName, query, maxResults);

            using (ProxomoWebRequest<T> p = new ProxomoWebRequest<T>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback, ref cTokens);
            }

        }
        //public void CustomDataGet<T>(string tableName, string customDataID)
        //{
        //    string url = string.Format("{0}/customdata/table/{1}/{2}", baseURL, tableName, customDataID);

        //    using (ProxomoWebRequest<object> p = new ProxomoWebRequest<object>(AuthToken.AccessToken, ValidateSSLCert, Format))
        //    {
        //        p.GetDataItem(url, "GET", contentType, CustomDataGetReady);
        //    }
        //}
        public void CustomDataGet<T>(string tableName, string customDataID, ProxomoUserCallbackDelegate<T> userCallback)
        { 
            string url = string.Format("{0}/customdata/table/{1}/{2}", baseURL, tableName, customDataID);

            using (ProxomoWebRequest<T> p = new ProxomoWebRequest<T>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                ContinuationTokens cTokens = new ContinuationTokens("", "");

                p.GetDataItem(url, "GET", contentType, "", userCallback);

            }
        }

        #endregion

        #region    Event Methods

        public void EventAdd(Event evt, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(evt, Format, false), userCallback);
            }
        }
        public void EventGet(string eventID, ProxomoUserCallbackDelegate<Event> userCallback)
        {
            string url =
                string.Format("{0}/event/{1}",
                baseURL,
                eventID);

            using (ProxomoWebRequest<Event> p = new ProxomoWebRequest<Event>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", "EventGet: ...", "", userCallback);
            }
        }
        public void EventUpdate(Event evt, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(evt, Format, false), userCallback);
            }
        }
        public void EventsSearchByDistance(decimal latitude, decimal longitude, decimal distance, DateTime starttime, DateTime endtime, ProxomoUserCallbackDelegate<List<Event>> userCallback, string eventType = "")
        {
            string url = null;

            //if (string.IsNullOrWhiteSpace(eventType)) //TODO
            if (string.IsNullOrEmpty(eventType)) //TODO
            {
                url = string.Format("{0}/events/search/latitude/{1}/longitude/{2}/distance/{3}/start/{4}/end/{5}", baseURL, latitude, longitude, distance, Utility.ConvertToUnixTimestamp(starttime), Utility.ConvertToUnixTimestamp(endtime));
            }
            else
            {
                url = string.Format("{0}/events/search/latitude/{1}/longitude/{2}/distance/{3}/start/{4}/end/{5}?eventtype={6}", baseURL, latitude, longitude, distance, Utility.ConvertToUnixTimestamp(starttime), Utility.ConvertToUnixTimestamp(endtime), eventType);
            }

            using (ProxomoWebRequest<List<Event>> p = new ProxomoWebRequest<List<Event>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void EventsSearchByPersonID(string personID, DateTime starttime, DateTime endtime, ProxomoUserCallbackDelegate<List<Event>> userCallback, string eventType = "")
        {
            string url = null;

            //if (string.IsNullOrWhiteSpace(eventType)) //TODO
            if (string.IsNullOrEmpty(eventType)) //TODO
            {
                url = string.Format("{0}/events/search/personid/{1}/start/{2}/end/{3}", baseURL, personID, Utility.ConvertToUnixTimestamp(starttime), Utility.ConvertToUnixTimestamp(endtime));
            }
            else
            {
                url = string.Format("{0}/events/search/personid/{1}/start/{2}/end/{3}?eventtype={4}", baseURL, personID, Utility.ConvertToUnixTimestamp(starttime), Utility.ConvertToUnixTimestamp(endtime), eventType);
            }

            using (ProxomoWebRequest<List<Event>> p = new ProxomoWebRequest<List<Event>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }

        #endregion

        #region EventComment Methods

        public void EventCommentAdd(string eventID, EventComment comment, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/comment", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(comment, Format, false), userCallback);
            }
        }
        public void EventCommentsGet(string eventID, ProxomoUserCallbackDelegate<List<EventComment>> userCallback)
        {
            string url = string.Format("{0}/event/{1}/comments", baseURL, eventID);

            using (ProxomoWebRequest<List<EventComment>> p = new ProxomoWebRequest<List<EventComment>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void EventCommentUpdate(string eventID, EventComment comment, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/comment", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(comment, Format, false), userCallback);
            }
        }
        public void EventCommentDelete(string eventID, string commentID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/comment/{2}", baseURL, eventID, commentID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }

        #endregion

        #region Event Participant Methods

        public void EventParticipantsGet(string eventID, ProxomoUserCallbackDelegate<List<EventParticipant>> userCallback)
        {
            string url = string.Format("{0}/event/{1}/participants", baseURL, eventID);

            using (ProxomoWebRequest<List<EventParticipant>> p = new ProxomoWebRequest<List<EventParticipant>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void EventParticipantInvite(string eventID, string personID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/participant/invite/personid/{2}", baseURL, eventID, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }

        public void EventParticipantsInvite(string eventID, string[] personIDs, ProxomoUserCallbackDelegate<string> userCallback)
        {

            var personIDstoStrArray = string.Join(",", personIDs);

            string url = string.Format("{0}/event/{1}/participants/invite/personids/{2}", baseURL, eventID, personIDstoStrArray);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }

        }
        public void EventParticipantDelete(string eventID, string participantID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/participant/{2}", baseURL, eventID, participantID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }

        public void EventRequestInvitation(string eventID, string personID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/requestinvite/personid/{2}", baseURL, eventID, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }
        public void EventRSVP(string eventID, EventParticipantStatus participantStatus, string personID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/RSVP/personid/{2}/participantstatus/{3}", baseURL, eventID, personID, Convert.ToInt16(participantStatus));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }


        #endregion

        # region EventAppData Methods

        public void EventAppDataAdd(string eventID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }
        public void EventAppDataDelete(string eventID, string appDataID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/appdata/{2}", baseURL, eventID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }
        public void EventAppDataGet(string eventID, string appDataID, ProxomoUserCallbackDelegate<AppData> userCallback)
        {
            string url = string.Format("{0}/event/{1}/appdata/{2}", baseURL, eventID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void EventAppDataGetAll(string eventID, ProxomoUserCallbackDelegate<List<AppData>> userCallback)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void EventAppDataUpdate(string eventID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }

        # endregion

        #region Friend Methods

        public void FriendsGet(string personID, ProxomoUserCallbackDelegate<List<Friend>> userCallback)
        {
            string url = string.Format("{0}/friends/personid/{1}", baseURL, personID);

            using (ProxomoWebRequest<List<Friend>> p = new ProxomoWebRequest<List<Friend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void FriendInvite(string friendA, string friendB, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/friend/invite/frienda/{1}/friendb/{2}", baseURL, friendA, friendB);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }
        public void FriendBySocialNetworkInvite(SocialNetwork socialnetwork, string frienda, string friendb, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/friend/invite/frienda/{1}/friendb/{2}/socialnetwork/{3}", baseURL, frienda, friendb, Convert.ToInt16(socialnetwork));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }
        public void FriendRespond(FriendResponse response, string friendA, string friendB, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/friend/respond/frienda/{1}/friendb/{2}/friendresponse/{3}", baseURL, friendA, friendB, Convert.ToInt16(response));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", userCallback);
            }
        }
        public void FriendsSocialNetworkGet(SocialNetwork socialNetwork, string personID, ProxomoUserCallbackDelegate<List<SocialNetworkFriend>> userCallback)
        {

            string url = string.Format("{0}/friends/personid/{1}/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkFriend>> p = new ProxomoWebRequest<List<SocialNetworkFriend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void FriendsSocialNetworkAppGet(SocialNetwork socialNetwork, string personID, ProxomoUserCallbackDelegate<List<SocialNetworkPFriend>> userCallback)
        {
            string url = string.Format("{0}/friends/app/personid/{1}/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkPFriend>> p = new ProxomoWebRequest<List<SocialNetworkPFriend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }

        #endregion

        #region GeoCode Methods

        public void GeoCodebyAddress(string address, ProxomoUserCallbackDelegate<GeoCode> userCallback)
        {
            string url = string.Format("{0}/geo/lookup/address/{1}", baseURL, HttpUtility.UrlEncode(address)); // Perform URL encoding since address can have spaces, commas, etc.

            using (ProxomoWebRequest<GeoCode> p = new ProxomoWebRequest<GeoCode>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void ReverseGeoCode(string latitude, string longitude, ProxomoUserCallbackDelegate<Location> userCallback)
        {
            string url = string.Format("{0}/geo/lookup/latitude/{1}/longitude/{2}", baseURL, latitude, longitude);

            using (ProxomoWebRequest<Location> p = new ProxomoWebRequest<Location>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void GeoCodeByIPAddress(string ipAddress, ProxomoUserCallbackDelegate<GeoIP> userCallback)
        {
            string url = string.Format("{0}/geo/lookup/ip/{1}", baseURL, ipAddress);

            using (ProxomoWebRequest<GeoIP> p = new ProxomoWebRequest<GeoIP>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }

        #endregion

        #region Location Methods

        public void LocationAdd(Location location, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(location, Format, false), userCallback);
            }
        }
        public void LocationGet(string locationID, ProxomoUserCallbackDelegate < Location > userCallback)
        {
            string url = string.Format("{0}/location/{1}", baseURL, locationID);

            using (ProxomoWebRequest<Location> p = new ProxomoWebRequest<Location>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void LocationUpdate(Location location, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(location, Format, false), userCallback);
            }
        }
        public void LocationDelete(string locationID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location/{1}", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }
        public void LocationCategoriesGet(ProxomoUserCallbackDelegate<List<Category>> userCallback)
        {
            string url = string.Format("{0}/location/categories", baseURL);

            using (ProxomoWebRequest<List<Category>> p = new ProxomoWebRequest<List<Category>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void LocationsSearchByAddress(string address, ProxomoUserCallbackDelegate<List<Location>> userCallback, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search", baseURL) + Utility.FormatQueryString(HttpUtility.UrlEncode(address), string.Empty, string.Empty, q, category, radius, scope, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void LocationsSearchByGPS(string latitude, string longitude, ProxomoUserCallbackDelegate<List<Location>> userCallback, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search/latitude/{1}/longitude/{2}", baseURL, latitude, longitude) + Utility.FormatQueryString(string.Empty, string.Empty, string.Empty, q, category, radius, scope, maxresults, personid);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void LocationsSearchByIPAddress(string ipAddress, ProxomoUserCallbackDelegate<List<Location>> userCallback, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search/ip/{1}", baseURL, ipAddress) + Utility.FormatQueryString(string.Empty, string.Empty, string.Empty, q, category, radius, scope, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
     
        #endregion

        #region Location AppData Methods

        public void LocationAppDataAdd(string locationID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }
        public void LocationAppDataDelete(string locationID, string appDataID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location/{1}/appdata/{2}", baseURL, locationID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }
        public void LocationAppDataGet(string locationID, string appDataID, ProxomoUserCallbackDelegate<AppData> userCallback)
        {
            string url = string.Format("{0}/location/{1}/appdata/{2}", baseURL, locationID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void LocationAppDataGetAll(string locationID, ProxomoUserCallbackDelegate<List<AppData>> userCallback)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void LocationAppDataUpdate(string locationID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }

        #endregion

        #region Notification Methods

        public void NotificationSend(Notification notification, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/notification", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(notification, Format, false), userCallback);
            }
        }

        #endregion

        #region Person Methods

        public void PersonGet(string personID, ProxomoUserCallbackDelegate<Person> userCallback)
        {
            string url = string.Format("{0}/person/{1}", baseURL, personID);

            using (ProxomoWebRequest<Person> p = new ProxomoWebRequest<Person>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", string.Empty, "", userCallback);
            }
        }
        public void PersonUpdate(Person person, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/person", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(person, Format, false), userCallback);
            }
        }

        public void PersonAppDataAdd(string personID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }
        public void PersonAppDataGet(string personID, string appDataID, ProxomoUserCallbackDelegate<AppData> userCallback)
        {
            string url = string.Format("{0}/person/{1}/appdata/{2}", baseURL, personID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void PersonAppDataGetAll(string personID, ProxomoUserCallbackDelegate<List<AppData>> userCallback)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }
        public void PersonAppDataDelete(string personID, string appDataID, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/person/{1}/appdata/{2}", baseURL, personID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", userCallback);
            }
        }

        public void PersonAppDataUpdate(string personID, AppData aData, ProxomoUserCallbackDelegate<string> userCallback)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), userCallback);
            }
        }
        public void PersonLocationsGet(string personID, ProxomoUserCallbackDelegate<List<Location>> userCallback, string latitude = "", string longitude = "", double radius = 25, int maxresults = 25)
        {
            string url = string.Format("{0}/person/{1}/locations", baseURL, personID) + Utility.FormatQueryString(string.Empty, latitude, longitude, string.Empty, string.Empty, radius, LocationSearchScope.ApplicationOnly, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }

        }
        public void PersonSocialNetworkInfoGet(string personID, SocialNetwork socialNetwork, ProxomoUserCallbackDelegate<List<SocialNetworkInfo>> userCallback)
        {
            string url = string.Format("{0}/person/{1}/socialnetworkinfo/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkInfo>> p = new ProxomoWebRequest<List<SocialNetworkInfo>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", userCallback);
            }
        }

        #endregion

        #endregion
    }
}
