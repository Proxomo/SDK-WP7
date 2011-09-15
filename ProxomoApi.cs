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

using System.Collections.Generic;

namespace Proxomo //ProxomoWP7SDK
{
    // This delegate is used to receive the actual event handler (callback) that the Top App wants to call
    // when initialization is complete. This is necessary since the user cannot register to listen until AFTER the 
    // SDK instance is created (constructor). Therefore, we need to register it for him within the constructor -- AFTER the 
    // SDK instance is created and BEFORE we make the call to init (to get the token).
    public delegate void UserInitCompleteHandler(object value);

    // This delegate is used INTERNALLY within this SDK to implement callbacks to raise appropriate events
    // that the use can register to listen to for each Proxomo method
    public delegate void ProxomoCallback(object value);

    #region Proxomo Events: (1) Define a delegate type FOR EACH TYPE OF RESULT that methods can return

    public delegate void ProxomoStringResultEventHandler(ItemCompletedEventArgs<string> results);
    public delegate void ProxomoTokenResultEventHandler(ItemCompletedEventArgs<Token> results);
    public delegate void ProxomoAppDataResultEventHandler(ItemCompletedEventArgs<AppData> results);
    public delegate void ProxomoListAppDataResultEventHandler(ItemCompletedEventArgs<List<AppData>> results);
    public delegate void ProxomoEventResultEventHandler(ItemCompletedEventArgs<Event> results);
    public delegate void ProxomoListOfEventResultEventHandler(ItemCompletedEventArgs<List<Event>> results);
    public delegate void ProxomoEventCommentsResultEventHandler(ItemCompletedEventArgs<List<EventComment>> results);
    public delegate void ProxomoListOfEventParticipantResultEventHandler(ItemCompletedEventArgs<List<EventParticipant>> results);
    public delegate void ProxomoEventParticipantResultEventHandler(ItemCompletedEventArgs<EventParticipant> results);
    public delegate void ProxomoListOfFriendResultEventHandler(ItemCompletedEventArgs<List<Friend>> results);
    public delegate void ProxomoListOfSocialNetworkFriendEventHandler(ItemCompletedEventArgs<List<SocialNetworkFriend>> results);
    public delegate void ProxomoListOfSocialNetworkPFriendEventHandler(ItemCompletedEventArgs<List<SocialNetworkPFriend>> results);
    public delegate void ProxomoGeoCodeResultEventHandler(ItemCompletedEventArgs<GeoCode> results);
    public delegate void ProxomoLocationResultEventHandler(ItemCompletedEventArgs<Location> results);
    public delegate void ProxomoGeoCodeByIPAddressEventHandler(ItemCompletedEventArgs<GeoIP> results);
    public delegate void ProxomoListOfCategoryResultEventHandler(ItemCompletedEventArgs<List<Category>> results);
    public delegate void ProxomoListOfLocationResultEventHandler(ItemCompletedEventArgs<List<Location>> results);
    public delegate void ProxomoPersonResultEventHandler(ItemCompletedEventArgs<Person> results);
    public delegate void ProxomoListOfSocialNetworkInfoEventHandler(ItemCompletedEventArgs<List<SocialNetworkInfo>> results);

    #endregion

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

        public ProxomoApi(string applicationID, string proxomoAPIKey, ProxomoTokenResultEventHandler callback, CommunicationType format = CommunicationType.JSON, bool validatessl = true, string url = "")
        {
            this.Initialization_Complete += new ProxomoTokenResultEventHandler(callback);
            Init(applicationID, proxomoAPIKey, "v09", format, validatessl, url);
        }
        public ProxomoApi(string applicationID, string proxomoAPIKey, string version, ProxomoTokenResultEventHandler callback, CommunicationType format = CommunicationType.JSON, bool validatessl = true, string url = "")
        {
            this.Initialization_Complete += new ProxomoTokenResultEventHandler(callback);
            Init(applicationID, proxomoAPIKey, version, format, validatessl, url);
        }

        private void Init(string applicationID, string proxomoAPIKey, string version, CommunicationType format, bool validatessl, string url)
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

            GetAuthToken();
        }

        private void GetAuthToken()
        {
            string url = string.Format("{0}/security/accesstoken/get?applicationid={1}&proxomoAPIKey={2}", baseURL, HttpUtility.UrlEncode(_applicationID), HttpUtility.UrlEncode(_proxomoAPIKey));

            using (ProxomoWebRequest<Token> p = new ProxomoWebRequest<Token>(ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, InitializationReady);
                // add timeout handling... 
            }
        }
        public void RefreshAuthToken()
        {
            GetAuthToken();
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

        #region Proxomo Events: (2) Declare the events that the user can listen to

        public event ProxomoTokenResultEventHandler Initialization_Complete;

        public event ProxomoStringResultEventHandler AppDataAdd_Complete;
        public event ProxomoStringResultEventHandler AppDataDelete_Complete;
        public event ProxomoAppDataResultEventHandler AppDataGet_Complete;
        public event ProxomoListAppDataResultEventHandler AppDataGetAll_Complete;
        public event ProxomoStringResultEventHandler AppDataUpdate_Complete;
        public event ProxomoListAppDataResultEventHandler AppDataSearch_Complete;

        public event ProxomoStringResultEventHandler EventAdd_Complete;
        public event ProxomoEventResultEventHandler EventGet_Complete;
        public event ProxomoStringResultEventHandler EventUpdate_Complete;
        public event ProxomoListOfEventResultEventHandler EventsSearchByDistance_Complete;
        public event ProxomoListOfEventResultEventHandler EventsSearchByPersonID_Complete;

        public event ProxomoStringResultEventHandler EventCommentAdd_Complete;
        public event ProxomoStringResultEventHandler EventCommentDelete_Complete;
        public event ProxomoEventCommentsResultEventHandler EventCommentsGet_Complete;
        public event ProxomoStringResultEventHandler EventCommentUpdate_Complete;

        public event ProxomoListOfEventParticipantResultEventHandler EventParticipantsGet_Complete;
        public event ProxomoStringResultEventHandler EventParticipantInvite_Complete;
        public event ProxomoStringResultEventHandler EventParticipantsInvite_Complete;
        public event ProxomoStringResultEventHandler EventParticipantsDelete_Complete;
        public event ProxomoStringResultEventHandler EventRequestInvitation_Complete;
        public event ProxomoStringResultEventHandler EventRSVP_Complete;

        public event ProxomoStringResultEventHandler EventAppDataAdd_Complete;
        public event ProxomoStringResultEventHandler EventAppDataDelete_Complete;
        public event ProxomoAppDataResultEventHandler EventAppDataGet_Complete;
        public event ProxomoListAppDataResultEventHandler EventAppDataGetAll_Complete;
        public event ProxomoStringResultEventHandler EventAppDataUpdate_Complete;

        public event ProxomoListOfFriendResultEventHandler FriendsGet_Complete;
        public event ProxomoStringResultEventHandler FriendInvite_Complete;
        public event ProxomoStringResultEventHandler FriendBySocialNetworkInvite_Complete;
        public event ProxomoStringResultEventHandler FriendRespond_Complete;
        public event ProxomoListOfSocialNetworkFriendEventHandler FriendsSocialNetworkGet_Complete;
        public event ProxomoListOfSocialNetworkPFriendEventHandler FriendsSocialNetworkAppGet_Complete;

        public event ProxomoGeoCodeResultEventHandler GeoCodebyAddress_Complete;
        public event ProxomoLocationResultEventHandler ReverseGeoCode_Complete;
        public event ProxomoGeoCodeByIPAddressEventHandler GeoCodeByIPAddress_Complete;

        public event ProxomoStringResultEventHandler LocationAdd_Complete;
        public event ProxomoStringResultEventHandler LocationDelete_Complete;
        public event ProxomoLocationResultEventHandler LocationGet_Complete;
        public event ProxomoStringResultEventHandler LocationUpdate_Complete;
        public event ProxomoListOfCategoryResultEventHandler LocationCategoriesGet_Complete;
        public event ProxomoListOfLocationResultEventHandler LocationsSearchByAddress_Complete;
        public event ProxomoListOfLocationResultEventHandler LocationsSearchByGPS_Complete;
        public event ProxomoListOfLocationResultEventHandler LocationsSearchByIPAddress_Complete;

        public event ProxomoStringResultEventHandler LocationAppDataAdd_Complete;
        public event ProxomoStringResultEventHandler LocationAppDataDelete_Complete;
        public event ProxomoAppDataResultEventHandler LocationAppDataGet_Complete;
        public event ProxomoListAppDataResultEventHandler LocationAppDataGetAll_Complete;
        public event ProxomoStringResultEventHandler LocationAppDataUpdate_Complete;

        public event ProxomoStringResultEventHandler NotificationSend_Complete;

        public event ProxomoPersonResultEventHandler PersonGet_Complete;
        public event ProxomoStringResultEventHandler PersonUpdate_Complete;
        public event ProxomoStringResultEventHandler PersonAppDataAdd_Complete;
        public event ProxomoStringResultEventHandler PersonAppDataDelete_Complete;
        public event ProxomoAppDataResultEventHandler PersonAppDataGet_Complete;
        public event ProxomoListAppDataResultEventHandler PersonAppDataGetAll_Complete;
        public event ProxomoStringResultEventHandler PersonAppDataUpdate_Complete;
        public event ProxomoListOfLocationResultEventHandler PersonLocationsGet_Complete;
        public event ProxomoListOfSocialNetworkInfoEventHandler PersonSocialNetworkInfoGet_Complete;

        #endregion

        #region Proxomo Events: (3) Define the internal callbacks to raise each of the events declared earlier

        private void InitializationReady(ItemCompletedEventArgs<Token> e)
        {
            _AuthToken = (Token)e.Result;
            AuthToken.ExpiresDate = Utility.ConvertFromUnixTimestamp(AuthToken.Expires);

            if (this.Initialization_Complete != null)
            {
                this.Initialization_Complete(e);
            }
        }

        private void AppDataAddReady(ItemCompletedEventArgs<string> e)
        {
            if (this.AppDataAdd_Complete != null)
            {
                this.AppDataAdd_Complete(e);
            }
        }
        private void AppDataDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.AppDataDelete_Complete != null) { this.AppDataDelete_Complete(e); } }
        private void AppDataGetReady(ItemCompletedEventArgs<AppData> e)
        {
            if (this.AppDataGet_Complete != null)
            {
                this.AppDataGet_Complete(e);
            }
        }
        private void AppDataGetAllReady(ItemCompletedEventArgs<List<AppData>> e)
        { if (this.AppDataGetAll_Complete != null) { this.AppDataGetAll_Complete(e); } }
        private void AppDataUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.AppDataUpdate_Complete != null) { this.AppDataUpdate_Complete(e); } }
        private void AppDataSearchReady(ItemCompletedEventArgs<List<AppData>> e)
        { if (this.AppDataSearch_Complete != null) { this.AppDataSearch_Complete(e); } }

        private void EventAddReady(ItemCompletedEventArgs<string> e)
        { if (this.EventAdd_Complete != null) { this.EventAdd_Complete(e); } }
        private void EventGetReady(ItemCompletedEventArgs<Event> e)
        { if (this.EventGet_Complete != null) { this.EventGet_Complete(e); } }
        private void EventUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.EventUpdate_Complete != null) { this.EventUpdate_Complete(e); } }
        private void EventsSearchByDistanceReady(ItemCompletedEventArgs<List<Event>> e)
        { if (this.EventsSearchByDistance_Complete != null) { this.EventsSearchByDistance_Complete(e); } }
        private void EventsSearchByPersonIDReady(ItemCompletedEventArgs<List<Event>> e)
        { if (this.EventsSearchByPersonID_Complete != null) { this.EventsSearchByPersonID_Complete(e); } }

        private void EventCommentAddReady(ItemCompletedEventArgs<string> e)
        { if (this.EventCommentAdd_Complete != null) { this.EventCommentAdd_Complete(e); } }
        private void EventCommentDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.EventCommentDelete_Complete != null) { this.EventCommentDelete_Complete(e); } }
        private void EventCommentsGetReady(ItemCompletedEventArgs<List<EventComment>> e)
        {
            if (this.EventCommentsGet_Complete != null)
            {
                this.EventCommentsGet_Complete(e);
            }
        }
        private void EventCommentUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.EventCommentUpdate_Complete != null) { this.EventCommentUpdate_Complete(e); } }

        private void EventParticipantsGetReady(ItemCompletedEventArgs<List<EventParticipant>> e)
        { if (this.EventParticipantsGet_Complete != null) { this.EventParticipantsGet_Complete(e); } }
        private void EventParticipantInviteReady(ItemCompletedEventArgs<string> e)
        { if (this.EventParticipantInvite_Complete != null) { this.EventParticipantInvite_Complete(e); } }
        private void EventParticipantsInviteReady(ItemCompletedEventArgs<string> e)
        { if (this.EventParticipantsInvite_Complete != null) { this.EventParticipantsInvite_Complete(e); } }
        private void EventParticipantDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.EventParticipantsDelete_Complete != null) { this.EventParticipantsDelete_Complete(e); } }
        private void EventRequestInvitationReady(ItemCompletedEventArgs<string> e)
        { if (this.EventRequestInvitation_Complete != null) { this.EventRequestInvitation_Complete(e); } }
        private void EventRSVPReady(ItemCompletedEventArgs<string> e)
        { if (this.EventRSVP_Complete != null) { this.EventRSVP_Complete(e); } }

        private void EventAppDataAddReady(ItemCompletedEventArgs<string> e)
        {
            if (this.EventAppDataAdd_Complete != null)
            {
                this.EventAppDataAdd_Complete(e);
            }
        }
        private void EventAppDataDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.EventAppDataDelete_Complete != null) { this.EventAppDataDelete_Complete(e); } }
        private void EventAppDataGetReady(ItemCompletedEventArgs<AppData> e)
        {
            if (this.EventAppDataGet_Complete != null)
            {
                this.EventAppDataGet_Complete(e);
            }
        }
        private void EventAppDataGetAllReady(ItemCompletedEventArgs<List<AppData>> e)
        { if (this.EventAppDataGetAll_Complete != null) { this.EventAppDataGetAll_Complete(e); } }
        private void EventAppDataUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.EventAppDataUpdate_Complete != null) { this.EventAppDataUpdate_Complete(e); } }

        private void FriendsGetReady(ItemCompletedEventArgs<List<Friend>> e)
        { if (this.FriendsGet_Complete != null) { this.FriendsGet_Complete(e); } }
        private void FriendInviteReady(ItemCompletedEventArgs<string> e)
        { if (this.FriendInvite_Complete != null) { this.FriendInvite_Complete(e); } }
        private void FriendBySocialNetworkInviteReady(ItemCompletedEventArgs<string> e)
        { if (this.FriendBySocialNetworkInvite_Complete != null) { this.FriendBySocialNetworkInvite_Complete(e); } }
        private void FriendRespondReady(ItemCompletedEventArgs<string> e)
        { if (this.FriendRespond_Complete != null) { this.FriendRespond_Complete(e); } }
        private void FriendsSocialNetworkGetReady(ItemCompletedEventArgs<List<SocialNetworkFriend>> e)
        { if (this.FriendsSocialNetworkGet_Complete != null) { this.FriendsSocialNetworkGet_Complete(e); } }
        private void FriendsSocialNetworkAppGetReady(ItemCompletedEventArgs<List<SocialNetworkPFriend>> e)
        { if (this.FriendsSocialNetworkAppGet_Complete != null) { this.FriendsSocialNetworkAppGet_Complete(e); } }

        private void GeoCodebyAddressReady(ItemCompletedEventArgs<GeoCode> e)
        { if (this.GeoCodebyAddress_Complete != null) { this.GeoCodebyAddress_Complete(e); } }
        private void ReverseGeoCodeReady(ItemCompletedEventArgs<Location> e)
        { if (this.ReverseGeoCode_Complete != null) { this.ReverseGeoCode_Complete(e); } }
        private void GeoCodeByIPAddressReady(ItemCompletedEventArgs<GeoIP> e)
        { if (this.GeoCodeByIPAddress_Complete != null) { this.GeoCodeByIPAddress_Complete(e); } }

        private void LocationAddReady(ItemCompletedEventArgs<string> e)
        { if (this.LocationAdd_Complete != null) { this.LocationAdd_Complete(e); } }
        private void LocationDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.LocationDelete_Complete != null) { this.LocationDelete_Complete(e); } }
        private void LocationGetReady(ItemCompletedEventArgs<Location> e)
        { if (this.LocationGet_Complete != null) { this.LocationGet_Complete(e); } }
        private void LocationUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.LocationUpdate_Complete != null) { this.LocationUpdate_Complete(e); } }
        private void LocationCategoriesGetReady(ItemCompletedEventArgs<List<Category>> e)
        { if (this.LocationCategoriesGet_Complete != null) { this.LocationCategoriesGet_Complete(e); } }
        private void LocationsSearchByAddressReady(ItemCompletedEventArgs<List<Location>> e)
        { if (this.LocationsSearchByAddress_Complete != null) { this.LocationsSearchByAddress_Complete(e); } }
        private void LocationsSearchByGPSReady(ItemCompletedEventArgs<List<Location>> e)
        { if (this.LocationsSearchByGPS_Complete != null) { this.LocationsSearchByGPS_Complete(e); } }
        private void LocationsSearchByIPAddressReady(ItemCompletedEventArgs<List<Location>> e)
        { if (this.LocationsSearchByIPAddress_Complete != null) { this.LocationsSearchByIPAddress_Complete(e); } }

        private void LocationAppDataAddReady(ItemCompletedEventArgs<string> e)
        {
            if (this.LocationAppDataAdd_Complete != null)
            {
                this.LocationAppDataAdd_Complete(e);
            }
        }
        private void LocationAppDataDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.LocationAppDataDelete_Complete != null) { this.LocationAppDataDelete_Complete(e); } }
        private void LocationAppDataGetReady(ItemCompletedEventArgs<AppData> e)
        {
            if (this.LocationAppDataGet_Complete != null)
            {
                this.LocationAppDataGet_Complete(e);
            }
        }
        private void LocationAppDataGetAllReady(ItemCompletedEventArgs<List<AppData>> e)
        { if (this.LocationAppDataGetAll_Complete != null) { this.LocationAppDataGetAll_Complete(e); } }
        private void LocationAppDataUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.LocationAppDataUpdate_Complete != null) { this.LocationAppDataUpdate_Complete(e); } }

        private void NotificationSendReady(ItemCompletedEventArgs<string> e)
        { if (this.NotificationSend_Complete != null) { this.NotificationSend_Complete(e); } }

        private void PersonGetReady(ItemCompletedEventArgs<Person> e)
        { if (this.PersonGet_Complete != null) { this.PersonGet_Complete(e); } }
        private void PersonUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.PersonUpdate_Complete != null) { this.PersonUpdate_Complete(e); } }
        private void PersonAppDataAddReady(ItemCompletedEventArgs<string> e)
        { if (this.PersonAppDataAdd_Complete != null) { this.PersonAppDataAdd_Complete(e); } }
        private void PersonAppDataDeleteReady(ItemCompletedEventArgs<string> e)
        { if (this.PersonAppDataDelete_Complete != null) { this.PersonAppDataDelete_Complete(e); } }
        private void PersonAppDataGetReady(ItemCompletedEventArgs<AppData> e)
        { if (this.PersonAppDataGet_Complete != null) { this.PersonAppDataGet_Complete(e); } }
        private void PersonAppDataGetAllReady(ItemCompletedEventArgs<List<AppData>> e)
        { if (this.PersonAppDataGetAll_Complete != null) { this.PersonAppDataGetAll_Complete(e); } }
        private void PersonAppDataUpdateReady(ItemCompletedEventArgs<string> e)
        { if (this.PersonAppDataUpdate_Complete != null) { this.PersonAppDataUpdate_Complete(e); } }
        private void PersonLocationsGetReady(ItemCompletedEventArgs<List<Location>> e)
        { if (this.PersonLocationsGet_Complete != null) { this.PersonLocationsGet_Complete(e); } }
        private void PersonSocialNetworkInfoGetReady(ItemCompletedEventArgs<List<SocialNetworkInfo>> e)
        { if (this.PersonSocialNetworkInfoGet_Complete != null) { this.PersonSocialNetworkInfoGet_Complete(e); } }

        #endregion

        #region Proxomo Methods
        #region AppData Methods

        public void AppDataAdd(AppData appData)
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(appData, Format, false), AppDataAddReady);
            }
        }
        public void AppDataDelete(string appDataID)
        {
            string url = string.Format("{0}/appdata/{1}", baseURL, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", AppDataDeleteReady);
            }
        }
        public void AppDataGet(string appDataID)
        {
            string url = string.Format("{0}/appdata/{1}", baseURL, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", AppDataGetReady);
            }
        }
        public void AppDataGetAll()
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", AppDataGetAllReady);
            }
        }
        public void AppDataUpdate(AppData appData)
        {
            string url = string.Format("{0}/appdata", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(appData, Format, false), AppDataUpdateReady);
            }
        }
        public void AppDataSearch(string objectType)
        {
            string url = string.Format("{0}/appdata/search/objecttype/{1}", baseURL, objectType);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", AppDataSearchReady);
            }
        }

        #endregion

        #region    Event Methods

        public void EventAdd(Event evt)
        {
            string url = string.Format("{0}/event", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(evt, Format, false), EventAddReady);
            }
        }
        public void EventGet(string eventID)
        {
            string url =
                string.Format("{0}/event/{1}",
                baseURL,
                eventID);

            using (ProxomoWebRequest<Event> p = new ProxomoWebRequest<Event>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", "EventGet: ...", "", EventGetReady);
            }
        }
        public void EventUpdate(Event evt)
        {
            string url = string.Format("{0}/event", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(evt, Format, false), EventUpdateReady);
            }
        }
        public void EventsSearchByDistance(decimal latitude, decimal longitude, decimal distance, DateTime starttime, DateTime endtime, string eventType = "")
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
                p.GetDataItem(url, "GET", contentType, "", EventsSearchByDistanceReady);
            }
        }
        public void EventsSearchByPersonID(string personID, DateTime starttime, DateTime endtime, string eventType = "")
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
                p.GetDataItem(url, "GET", contentType, "", EventsSearchByPersonIDReady);
            }
        }

        #endregion

        #region EventComment Methods

        public void EventCommentAdd(string eventID, EventComment comment)
        {
            string url = string.Format("{0}/event/{1}/comment", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(comment, Format, false), EventCommentAddReady);
            }
        }
        public void EventCommentsGet(string eventID)
        {
            string url = string.Format("{0}/event/{1}/comments", baseURL, eventID);

            using (ProxomoWebRequest<List<EventComment>> p = new ProxomoWebRequest<List<EventComment>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", EventCommentsGetReady);
            }
        }
        public void EventCommentUpdate(string eventID, EventComment comment)
        {
            string url = string.Format("{0}/event/{1}/comment", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(comment, Format, false), EventUpdateReady);
            }
        }
        public void EventCommentDelete(string eventID, string commentID)
        {
            string url = string.Format("{0}/event/{1}/comment/{2}", baseURL, eventID, commentID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", EventCommentDeleteReady);
            }
        }

        #endregion

        #region Event Participant Methods

        public void EventParticipantsGet(string eventID)
        {
            string url = string.Format("{0}/event/{1}/participants", baseURL, eventID);

            using (ProxomoWebRequest<List<EventParticipant>> p = new ProxomoWebRequest<List<EventParticipant>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", EventParticipantsGetReady);
            }
        }
        public void EventParticipantInvite(string eventID, string personID)
        {
            string url = string.Format("{0}/event/{1}/participant/invite/personid/{2}", baseURL, eventID, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", EventParticipantInviteReady);
            }
        }

        public void EventParticipantsInvite(string eventID, string[] personIDs)
        {

            var personIDstoStrArray = string.Join(",", personIDs);

            string url = string.Format("{0}/event/{1}/participants/invite/personids/{2}", baseURL, eventID, personIDstoStrArray);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", EventParticipantsInviteReady);
            }

        }
        public void EventParticipantDelete(string eventID, string participantID)
        {
            string url = string.Format("{0}/event/{1}/participant/{2}", baseURL, eventID, participantID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", EventParticipantDeleteReady);
            }
        }

        public void EventRequestInvitation(string eventID, string personID)
        {
            string url = string.Format("{0}/event/{1}/requestinvite/personid/{2}", baseURL, eventID, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", EventRequestInvitationReady);
            }
        }
        public void EventRSVP(string eventID, EventParticipantStatus participantStatus, string personID)
        {
            string url = string.Format("{0}/event/{1}/RSVP/personid/{2}/participantstatus/{3}", baseURL, eventID, personID, Convert.ToInt16(participantStatus));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", EventRSVPReady);
            }
        }


        #endregion

        # region EventAppData Methods

        public void EventAppDataAdd(string eventID, AppData aData)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), EventAppDataAddReady);
            }
        }
        public void EventAppDataDelete(string eventID, string appDataID)
        {
            string url = string.Format("{0}/event/{1}/appdata/{2}", baseURL, eventID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, EventAppDataDeleteReady);
            }
        }
        public void EventAppDataGet(string eventID, string appDataID)
        {
            string url = string.Format("{0}/event/{1}/appdata/{2}", baseURL, eventID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, EventAppDataGetReady);
            }
        }
        public void EventAppDataGetAll(string eventID)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, EventAppDataGetAllReady);
            }
        }
        public void EventAppDataUpdate(string eventID, AppData aData)
        {
            string url = string.Format("{0}/event/{1}/appdata", baseURL, eventID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), EventAppDataUpdateReady);
            }
        }

        # endregion

        #region Friend Methods

        public void FriendsGet(string personID)
        {
            string url = string.Format("{0}/friends/personid/{1}", baseURL, personID);

            using (ProxomoWebRequest<List<Friend>> p = new ProxomoWebRequest<List<Friend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", FriendsGetReady);
            }
        }
        public void FriendInvite(string friendA, string friendB)
        {
            string url = string.Format("{0}/friend/invite/frienda/{1}/friendb/{2}", baseURL, friendA, friendB);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", FriendInviteReady);
            }
        }
        public void FriendBySocialNetworkInvite(SocialNetwork socialnetwork, string frienda, string friendb)
        {
            string url = string.Format("{0}/friend/invite/frienda/{1}/friendb/{2}/socialnetwork/{3}", baseURL, frienda, friendb, Convert.ToInt16(socialnetwork));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", FriendBySocialNetworkInviteReady);
            }
        }
        public void FriendRespond(FriendResponse response, string friendA, string friendB)
        {
            string url = string.Format("{0}/friend/respond/frienda/{1}/friendb/{2}/friendresponse/{3}", baseURL, friendA, friendB, Convert.ToInt16(response));

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, "", FriendRespondReady);
            }
        }
        public void FriendsSocialNetworkGet(SocialNetwork socialNetwork, string personID)
        {

            string url = string.Format("{0}/friends/personid/{1}/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkFriend>> p = new ProxomoWebRequest<List<SocialNetworkFriend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", FriendsSocialNetworkGetReady);
            }
        }
        public void FriendsSocialNetworkAppGet(SocialNetwork socialNetwork, string personID)
        {
            string url = string.Format("{0}/friends/app/personid/{1}/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkPFriend>> p = new ProxomoWebRequest<List<SocialNetworkPFriend>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", FriendsSocialNetworkAppGetReady);
            }
        }

        #endregion

        #region GeoCode Methods

        public void GeoCodebyAddress(string address)
        {
            string url = string.Format("{0}/geo/lookup/address/{1}", baseURL, HttpUtility.UrlEncode(address)); // Perform URL encoding since address can have spaces, commas, etc.

            using (ProxomoWebRequest<GeoCode> p = new ProxomoWebRequest<GeoCode>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", GeoCodebyAddressReady);
            }

        }
        public void ReverseGeoCode(string latitude, string longitude)
        {
            string url = string.Format("{0}/geo/lookup/latitude/{1}/longitude/{2}", baseURL, latitude, longitude);

            using (ProxomoWebRequest<Location> p = new ProxomoWebRequest<Location>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", ReverseGeoCodeReady);
            }

        }
        public void GeoCodeByIPAddress(string ipAddress)
        {
            string url = string.Format("{0}/geo/lookup/ip/{1}", baseURL, ipAddress);

            using (ProxomoWebRequest<GeoIP> p = new ProxomoWebRequest<GeoIP>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", GeoCodeByIPAddressReady);
            }

        }

        #endregion

        #region Location Methods

        public void LocationAdd(Location location)
        {
            string url = string.Format("{0}/location", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(location, Format, false), LocationAddReady);
            }
        }
        public void LocationGet(string locationID)
        {
            string url = string.Format("{0}/location/{1}", baseURL, locationID);

            using (ProxomoWebRequest<Location> p = new ProxomoWebRequest<Location>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", LocationGetReady);
            }

        }
        public void LocationUpdate(Location location)
        {
            string url = string.Format("{0}/location", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(location, Format, false), LocationUpdateReady);
            }
        }
        public void LocationDelete(string locationID)
        {
            string url = string.Format("{0}/location/{1}", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", LocationDeleteReady);
            }
        }
        public void LocationCategoriesGet()
        {
            string url = string.Format("{0}/location/categories", baseURL);

            using (ProxomoWebRequest<List<Category>> p = new ProxomoWebRequest<List<Category>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", LocationCategoriesGetReady);
            }
        }
        public void LocationsSearchByAddress(string address, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search", baseURL) + Utility.FormatQueryString(HttpUtility.UrlEncode(address), string.Empty, string.Empty, q, category, radius, scope, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", LocationsSearchByAddressReady);
            }

        }
        public void LocationsSearchByGPS(string latitude, string longitude, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search/latitude/{1}/longitude/{2}", baseURL, latitude, longitude) + Utility.FormatQueryString(string.Empty, string.Empty, string.Empty, q, category, radius, scope, maxresults, personid);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", LocationsSearchByGPSReady);
            }

        }
        public void LocationsSearchByIPAddress(string ipAddress, string q = "", string category = "", double radius = 2, LocationSearchScope scope = LocationSearchScope.ApplicationOnly, int maxresults = 10, string personid = "")
        {
            string url = string.Format("{0}/locations/search/ip/{1}", baseURL, ipAddress) + Utility.FormatQueryString(string.Empty, string.Empty, string.Empty, q, category, radius, scope, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", LocationsSearchByIPAddressReady);
            }

        }
     
        #endregion

        #region Location AppData Methods

        public void LocationAppDataAdd(string locationID, AppData aData)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), LocationAppDataAddReady);
            }
        }
        public void LocationAppDataDelete(string locationID, string appDataID)
        {
            string url = string.Format("{0}/location/{1}/appdata/{2}", baseURL, locationID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, LocationAppDataDeleteReady);
            }
        }
        public void LocationAppDataGet(string locationID, string appDataID)
        {
            string url = string.Format("{0}/location/{1}/appdata/{2}", baseURL, locationID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, LocationAppDataGetReady);
            }
        }
        public void LocationAppDataGetAll(string locationID)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, LocationAppDataGetAllReady);
            }
        }
        public void LocationAppDataUpdate(string locationID, AppData aData)
        {
            string url = string.Format("{0}/location/{1}/appdata", baseURL, locationID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), LocationAppDataUpdateReady);
            }
        }

        #endregion

        #region Notification Methods

        public void NotificationSend(Notification notification)
        {
            string url = string.Format("{0}/notification", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(notification, Format, false), NotificationSendReady);
            }
        }

        #endregion

        #region Person Methods

        public void PersonGet(string personID)
        {
            string url = string.Format("{0}/person/{1}", baseURL, personID);

            using (ProxomoWebRequest<Person> p = new ProxomoWebRequest<Person>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", string.Empty, "", PersonGetReady);
            }
        }
        public void PersonUpdate(Person person)
        {
            string url = string.Format("{0}/person", baseURL);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(person, Format, false), PersonUpdateReady);
            }
        }

        public void PersonAppDataAdd(string personID, AppData aData)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "POST", contentType, Converter.Convert(aData, Format, false), PersonAppDataAddReady);
            }
        }
        public void PersonAppDataGet(string personID, string appDataID)
        {
            string url = string.Format("{0}/person/{1}/appdata/{2}", baseURL, personID, appDataID);

            using (ProxomoWebRequest<AppData> p = new ProxomoWebRequest<AppData>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", PersonAppDataGetReady);
            }
        }
        public void PersonAppDataGetAll(string personID)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<List<AppData>> p = new ProxomoWebRequest<List<AppData>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", PersonAppDataGetAllReady);
            }
        }
        public void PersonAppDataDelete(string personID, string appDataID)
        {
            string url = string.Format("{0}/person/{1}/appdata/{2}", baseURL, personID, appDataID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "DELETE", contentType, "", PersonAppDataDeleteReady);
            }
        }

        public void PersonAppDataUpdate(string personID, AppData aData)
        {
            string url = string.Format("{0}/person/{1}/appdata", baseURL, personID);

            using (ProxomoWebRequest<string> p = new ProxomoWebRequest<string>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "PUT", contentType, Converter.Convert(aData, Format, false), PersonAppDataUpdateReady);
            }
        }
        public void PersonLocationsGet(string personID, string latitude = "", string longitude = "", double radius = 25, int maxresults = 25)
        {
            string url = string.Format("{0}/person/{1}/locations", baseURL, personID) + Utility.FormatQueryString(string.Empty, latitude, longitude, string.Empty, string.Empty, radius, LocationSearchScope.ApplicationOnly, maxresults);

            using (ProxomoWebRequest<List<Location>> p = new ProxomoWebRequest<List<Location>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", PersonLocationsGetReady);
            }

        }
        public void PersonSocialNetworkInfoGet(string personID, SocialNetwork socialNetwork)
        {
            string url = string.Format("{0}/person/{1}/socialnetworkinfo/socialnetwork/{2}", baseURL, personID, Convert.ToInt16(socialNetwork));

            using (ProxomoWebRequest<List<SocialNetworkInfo>> p = new ProxomoWebRequest<List<SocialNetworkInfo>>(AuthToken.AccessToken, ValidateSSLCert, Format))
            {
                p.GetDataItem(url, "GET", contentType, "", PersonSocialNetworkInfoGetReady);
            }
        }

        #endregion

        #endregion
    }
}
