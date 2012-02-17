using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Proxomo;
using System.Threading;


namespace ProxomoWP7Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Callback Helper functions

    // used by each callback to check for exceptions returned by SDK via the Error argument before processign Results
    // Not the most graceful approach but quick and dirty to get this in place for now
    private void CheckErrorReturned(Exception error)
    {

        return;
    }

        private string EventToString(Event myEvent)
        {
            string eventString = String.Format("Event name: {0}, ", myEvent.EventName);

            if (myEvent.Description.Length < 15) // truncate the description we display in our demo if it is too long
            {
                eventString += string.Format("Desc: {0}..., ", myEvent.Description);
            }
            else
            {
                eventString += string.Format("Desc: {0}..., ", myEvent.Description.Substring(0, 15));
            }

            eventString += string.Format("Country Name: {0}, ", myEvent.CountryName);
            eventString += string.Format("Image URL: {0}, ", myEvent.ImageURL);
            eventString += string.Format("End Time: {0}, ", Convert.ToString(myEvent.EndTime));
            eventString += string.Format("Event ID: {0}, ", myEvent.ID);
            eventString += string.Format("Last Update: {0}, ", Convert.ToString(myEvent.LastUpdate));
            eventString += string.Format("Latitude: {0}, ", Convert.ToString(myEvent.Latitude));
            eventString += string.Format("Longitude: {0}, ", Convert.ToString(myEvent.Longitude));
            eventString += string.Format("MaxParticipants: {0}, ", Convert.ToString(myEvent.MaxParticipants));
            eventString += string.Format("Privacy: {0}, ", Convert.ToString(myEvent.Privacy));
            eventString += string.Format("Status: {0}, ", Convert.ToString(myEvent.Status));
            eventString += string.Format("Person ID: {0}, ", myEvent.PersonID);
            eventString += string.Format("Person name: {0}", myEvent.PersonName);

            return eventString;

        }
        private string LocationToString(Location record)
        {
            string msgString = string.Format("ID: {0}, ", record.ID);
            msgString += String.Format("Name: {0}, ", record.Name);
            msgString += string.Format("Lat: {0},", record.Latitude);
            msgString += string.Format("Lon: {0}, ", record.Longitude);
            msgString += string.Format("Addr1: {0}, ", record.Address1);
            msgString += string.Format("City: {0},", record.City);
            msgString += string.Format("State: {0},", record.State);
            msgString += string.Format("Zip: {0}, ", record.Zip);
            msgString += string.Format("LocType: {0}, ", record.LocationType);
            msgString += string.Format("PersonID: {0}, ", record.PersonID);
            msgString += string.Format("UTCoffset: {0} ", record.UTCOffset.ToString());
            return msgString;
        }
        private string PersonToString(Person record)
        {
            string msgString = string.Format("Full Name: {0}, ", record.FullName);
            msgString += string.Format("* display other fields later (like AppData Array!)");
            return msgString;
        }
        private string AppDataToString(AppData record)
        {
            string msgString = string.Format("ID: {0}, ", record.ID);
            msgString += string.Format("Key: {0}, ", record.Key);
            msgString += string.Format("Value: {0}, ", record.Value);
            msgString += string.Format("ObjectType: {0}", record.ObjectType);
            return msgString;
        }
        private string SocialNetworkInfoToString(SocialNetworkInfo record)
        {
            string msgString = string.Format("ID: {0}, ", record.ID);
            msgString += string.Format("Key: {0},", record.Key);
            msgString += string.Format("Person ID: {0},", record.PersonID);
            msgString += string.Format("Social Network: {0},", Convert.ToString(record.SocialNetwork));
            msgString += string.Format("Value: {0},", record.Value);
            return msgString;
        }
        private string GeoIPToString(GeoIP record)
        {
            string msgString = string.Format("Lat: {0}, ", record.Latitude);
            msgString += string.Format("Lon: {0}, ", record.Longitude);
            msgString += string.Format("IP: {0}", record.IP);
            return msgString;
        }
        private string GeoCodeToString(GeoCode record)
        {
            string msgString = string.Format("Addr: {0}, ", record.Address);
            msgString += string.Format("City: {0}, ", record.City);
            msgString += string.Format("Zip: {0}", record.Zip);
            return msgString;
        }

        #endregion

        private void Initialization_Complete(ItemCompletedEventArgs<Token> args)
        { //Callback no longer being used for signaling when Init has completed. We want the initialization to act synchronously so it should ot return until it is done.
            //Console.WriteLine("Hello, world!");
            Dispatcher.BeginInvoke(() => ConnectionStatus.Text = "Connected");
            Dispatcher.BeginInvoke(() => ProxomoMethodList.IsEnabled = true);
            Dispatcher.BeginInvoke(() => RunButton.IsEnabled = true);
            Dispatcher.BeginInvoke(() => Login.IsEnabled = true);
        }

        #region    1. Event Handlers

        #region AppData Callbacks

        private void AppDataAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error); 
            List<string> names = new List<string>();
            names.Add("AppData Record created. Record ID" + args.Result);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void AppDataDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void AppDataGet_Complete(ItemCompletedEventArgs<AppData> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(AppDataToString(args.Result));
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void AppDataGetAll_Complete(ItemCompletedEventArgs<List<AppData>> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", args.Result.Count));
            foreach (AppData element in args.Result)
            {
                names.Add(AppDataToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void AppDataUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void AppDataSearch_Complete(ItemCompletedEventArgs<List<AppData>> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", args.Result.Count));
            foreach (AppData element in args.Result)
            {
                names.Add(AppDataToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }

        #endregion

        #region CustomData Callbacks

        private void CustomDataAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("CustomData Record created. Record ID" + args.Result);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void CustomDataDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void CustomDataUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void CustomDataGet_Complete(ItemCompletedEventArgs<Custom_TrainingRec> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Get completed.Need to add code to display item contents.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        private void CustomDataSearch_Complete(ItemCompletedEventArgs<List<Custom_TrainingRec>> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Search completed. Need to add code to display items.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }

        #endregion

        #region    Event Callbacks

        private void EventAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            string myEventID = args.Result;
            List<string> names = new List<string>();
            names.Add("Record created. ID: " + myEventID);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventGet_Complete(ItemCompletedEventArgs<Event> args)
        {
            CheckErrorReturned(args.Error);
            Event myEvent = args.Result;

            List<string> names = new List<string>();
            names.Add("Event name: " + myEvent.EventName);
            names.Add("Description: " + myEvent.Description);
            names.Add("Country Name: {0} " + myEvent.CountryName);
            names.Add("Image URL: {0} "+ myEvent.ImageURL);
            names.Add("End Time: " + Convert.ToString(myEvent.EndTime));
            names.Add("Event ID:  " + myEvent.ID);
            names.Add("Last Update: {0}  : " + Convert.ToString(myEvent.LastUpdate));
            names.Add("Latitude:  " + Convert.ToString(myEvent.Latitude));
            names.Add("Longitude:  " + Convert.ToString(myEvent.Longitude));
            names.Add("MaxParticipants:  " + Convert.ToString(myEvent.MaxParticipants));
            names.Add("Privacy:  " + Convert.ToString(myEvent.Privacy));
            names.Add("Status:  " + Convert.ToString(myEvent.Status));
            names.Add("Person ID:  " + myEvent.PersonID);
            names.Add("Person name:" + myEvent.PersonName);

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventsSearchByDistance_Complete(ItemCompletedEventArgs<List<Event>> args)
        {
            CheckErrorReturned(args.Error);
            List<Event> listRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", listRecords.Count));
            foreach (Event element in listRecords)
            {
                names.Add(EventToString(element));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventsSearchByPersonID_Complete(ItemCompletedEventArgs<List<Event>> args)
        {
            CheckErrorReturned(args.Error);
            List<Event> listRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", listRecords.Count));
            foreach (Event element in listRecords)
            {
                names.Add(EventToString(element));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region EventComment Callbacks

        private void EventCommentAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            // Proxomo Unique identifier is returned for the added comment
            string myCommentID = args.Result;
            List<string> names = new List<string>();
            names.Add(string.Format("Comment ID returned: {0}", myCommentID));
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventCommentsGet_Complete(ItemCompletedEventArgs<List<EventComment>> args)
        {
            CheckErrorReturned(args.Error);
            List<EventComment> listRecords = (List<EventComment>)args.Result;

            List<string> names = new List<string>();

            names.Add(string.Format("Number of records: {0}", listRecords.Count));
            foreach (EventComment element in listRecords)
            {
                names.Add(string.Format("EventID: {0}, ID: {1}, LastUpd: {2}, Comment: {3}, By: {4}", element.EventID, element.ID, element.LastUpdate, element.Comment, element.PersonName));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventCommentUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventCommentDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region    Event Participant Callbacks

        private void EventParticipantsGet_Complete(ItemCompletedEventArgs<List<EventParticipant>> args)
        {
            CheckErrorReturned(args.Error);
            List<EventParticipant> listRecords = args.Result;

            List<string> names = new List<string>();

            names.Add(string.Format("Number of records is: {0}", listRecords.Count));
            foreach (EventParticipant element in listRecords)
            {
                string msgString = string.Format("EventID: {0}, ", element.EventID);
                msgString += string.Format("ID: {0}, ", element.ID);
                msgString += string.Format("ImageURL: {0}, ", element.ImageURL);
                msgString += string.Format("PersonID: {0}, ", element.PersonID);
                msgString += string.Format("Last Update: {0}, ", Convert.ToString(element.LastUpdate));
                msgString += string.Format("Status: {0} ", Convert.ToString(element.Status));

                names.Add(msgString);
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventParticipantInvite_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invite completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventParticipantsInvite_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invites completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventParticipantsDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invite deleted.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventRequestInvitation_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invitation request completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventRSVP_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("RSVP completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region Location AppData Callbacks

        private void EventAppDataAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Event AppData Record created. Record ID: " + args.Result);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventAppDataDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Event AppData Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventAppDataGet_Complete(ItemCompletedEventArgs<AppData> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(AppDataToString(args.Result));
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void EventAppDataGetAll_Complete(ItemCompletedEventArgs<List<AppData>> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", args.Result.Count));
            foreach (AppData element in args.Result)
            {
                names.Add(AppDataToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void EventAppDataUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Event AppData Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region Friend Callbacks

        private void FriendsGet_Complete(ItemCompletedEventArgs<List<Friend>> args)
        {
            CheckErrorReturned(args.Error);
            if (args.Result == null)
            {
                if (args.Error == null || string.IsNullOrEmpty(args.Error.Message))
                {
                    MessageBox.Show("Unknown error");
                    return;
                }
                MessageBox.Show(args.Error.Message);
                return;
            }

            List<Friend> listFriends = args.Result;

            List<string> names = new List<string>();

            foreach (Friend element in listFriends)
            {
                names.Add("FullName: " + element.FullName + ", PersonID: " + element.PersonID + ", Friend status: " + Convert.ToString(element.FriendStatus));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void FriendInvite_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invite completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void FriendBySocialNetworkInvite_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Invite completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void FriendRespond_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void FriendsSocialNetworkGet_Complete(ItemCompletedEventArgs<List<SocialNetworkFriend>> args)
        {
            CheckErrorReturned(args.Error);
            List<SocialNetworkFriend> listRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", listRecords.Count));
            foreach (SocialNetworkFriend element in listRecords)
            {
                names.Add(string.Format("Full Name: {0}, ID: {1}, Image URL: {2}, Link to profile: {3}", element.FullName, element.ID, element.ImageURL, element.Link));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void FriendsSocialNetworkAppGet_Complete(ItemCompletedEventArgs<List<SocialNetworkPFriend>> args)
        {
            CheckErrorReturned(args.Error);
            List<SocialNetworkPFriend> listRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", listRecords.Count));
            foreach (SocialNetworkPFriend element in listRecords)
            {
                names.Add(string.Format("Full Name: {0}, ID: {1}, Image URL: {2}, Link to profile: {3}, Person ID: {4}", element.FullName, element.ID, element.ImageURL, element.Link, element.PersonID));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region GeoCode Callbacks

        private void GeoCodebyAddress_Complete(ItemCompletedEventArgs<GeoCode> args)
        {
            CheckErrorReturned(args.Error);
            GeoCode record = args.Result;
            List<string> names = new List<string>();

            names.Add(GeoCodeToString(record));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void GeoCodeByIPAddress_Complete(ItemCompletedEventArgs<GeoIP> args)
        {
            CheckErrorReturned(args.Error);
            GeoIP record = args.Result;

            List<string> names = new List<string>();
            names.Add(GeoIPToString(record));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void ReverseGeoCode_Complete(ItemCompletedEventArgs<Location> args)
        {
            CheckErrorReturned(args.Error); 
            Location record = args.Result;

            List<string> names = new List<string>();
            names.Add(LocationToString(record));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        #endregion

        #region Location Callbacks

        private void LocationAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            string recordID = args.Result;
            List<string> names = new List<string>();
            names.Add("Record created. Record ID: " + recordID);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationGet_Complete(ItemCompletedEventArgs<Location> args)
        {
            CheckErrorReturned(args.Error);
            Location record = args.Result;

            List<string> names = new List<string>();
            names.Add(LocationToString(record));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationCategoriesGet_Complete(ItemCompletedEventArgs<List<Category>> args)
        {
            CheckErrorReturned(args.Error);
            List<Category> listOfRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records: {0}", listOfRecords.Count));
            foreach (Category element in listOfRecords)
            {
                names.Add(string.Format("Category: {0}, Type: {1}, Subcategory: {2}", element.category, element.type, element.subcategory));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationsSearchByAddress_Complete(ItemCompletedEventArgs<List<Location>> args)
        {
            CheckErrorReturned(args.Error);
            List<Location> listRecords = args.Result;

            List<string> names = new List<string>();

            names.Add(string.Format("Number of records: {0}", listRecords.Count));
            foreach (Location element in listRecords)
            {
                names.Add(LocationToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationsSearchByGPS_Complete(ItemCompletedEventArgs<List<Location>> args)
        {
            CheckErrorReturned(args.Error);
            List<Location> listRecords = args.Result;

            List<string> names = new List<string>();

            names.Add(string.Format("Number of records: {0}", listRecords.Count));
            foreach (Location element in listRecords)
            {
                names.Add(LocationToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationsSearchByIPAddress_Complete(ItemCompletedEventArgs<List<Location>> args)
        {
            CheckErrorReturned(args.Error);
            List<Location> listRecords = args.Result;

            List<string> names = new List<string>();

            names.Add(string.Format("Number of records: {0}", listRecords.Count));
            foreach (Location element in listRecords)
            {
                names.Add(LocationToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region Location AppData Callbacks

        private void LocationAppDataAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Location AppData Record created. Record ID: " + args.Result);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationAppDataDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Location AppData Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationAppDataGet_Complete(ItemCompletedEventArgs<AppData> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(AppDataToString(args.Result));
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void LocationAppDataGetAll_Complete(ItemCompletedEventArgs<List<AppData>> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add(string.Format("Number of records is: {0}", args.Result.Count));
            foreach (AppData element in args.Result)
            {
                names.Add(AppDataToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);

        }
        private void LocationAppDataUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Location AppData Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region Notification Callbacks

        private void NotificationSend_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Notification sent.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #region Person Callbacks

        private void PersonGet_Complete(ItemCompletedEventArgs<Person> args)
        {
            CheckErrorReturned(args.Error);
            Person record = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Fullname: {0}: ,", record.FullName));
            names.Add(string.Format("PersonID: {0}: , " , record.ID));
            //names.Add(string.Format("EmailAddress: {0}: ," + record.EmailAddress));
            //names.Add(string.Format("EmailVerificationStatus: {0}: ," + record.EmailVerificationStatus.ToString()));
            //names.Add(string.Format("EmailVerified: {0}: , " + record.EmailVerified.ToString()));
            //names.Add(string.Format("FacebookID: {0}: , " + record.FacebookID));
            //names.Add(string.Format("ImageURL: {0}: , " + record.ImageURL));
            //names.Add(string.Format("MobileAlerts: {0}: ," + record.MobileAlerts.ToString()));
            //names.Add(string.Format("MobileVerified: {0}: , " + record.MobileVerified.ToString()));
            //names.Add(string.Format("TwitterID: {0}: ," + record.TwitterID));
            //names.Add(string.Format("UTCOffset: {0}: " + record.UTCOffset.ToString()));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        private void PersonAppDataAdd_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            string recordID = args.Result;
            List<string> names = new List<string>();
            names.Add("Record created. Record ID" + recordID);
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonAppDataDelete_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Delete completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonAppDataGet_Complete(ItemCompletedEventArgs<AppData> args)
        {
            CheckErrorReturned(args.Error);
            AppData record = args.Result;

            List<string> names = new List<string>();
            names.Add(AppDataToString(record));

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonAppDataGetAll_Complete(ItemCompletedEventArgs<List<AppData>> args)
        {
            CheckErrorReturned(args.Error);
            List<AppData> listOfRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records: {0}", listOfRecords.Count));
            foreach (AppData element in listOfRecords)
            {
                names.Add(AppDataToString(element));
            }

            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonAppDataUpdate_Complete(ItemCompletedEventArgs<string> args)
        {
            CheckErrorReturned(args.Error);
            List<string> names = new List<string>();
            names.Add("Update completed.");
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        private void PersonLocationsGet_Complete(ItemCompletedEventArgs<List<Location>> args)
        {
            CheckErrorReturned(args.Error);
            List<Location> listOfRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records: {0}", listOfRecords.Count));
            foreach (Location element in listOfRecords)
            {
                names.Add(LocationToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }
        private void PersonSocialNetworkInfoGet_Complete(ItemCompletedEventArgs<List<SocialNetworkInfo>> args)
        {
            CheckErrorReturned(args.Error);
            List<SocialNetworkInfo> listOfRecords = args.Result;

            List<string> names = new List<string>();
            names.Add(string.Format("Number of records: {0}", listOfRecords.Count));
            foreach (SocialNetworkInfo element in listOfRecords)
            {
                names.Add(SocialNetworkInfoToString(element));
            }
            Dispatcher.BeginInvoke(() => OutputListBox.ItemsSource = names);
        }

        #endregion

        #endregion

        string applicationID = "nngAqYvGWMM9EwyO";
        string ProxomoAPIkey = "NnunozvrF6hg4Zfu77uqf8ssKwt1ueV+eZlCWk2k++I=";
        public Proxomo.ProxomoApi WP7SDKInstance;

        public void checkSDKInitialized()
        {
            if (WP7SDKInstance == null)
            {
                throw new Exception("WP7 object = null. Must instantiate it before methods can be called on it.");
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            initializeButtonState();
            initializeMethodListBox();
        }
        
        #region UI helpers (add new methods to the menu here!)
        private void initializeButtonState()
        {
            ProxomoMethodList.IsEnabled = false;
            RunButton.IsEnabled = false;
            Login.IsEnabled = false;
            ProxomoMethodList.SelectedItem = 0; // init to first item on list
            OutputSelectionInTextForm.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            OutputSelectionInTextForm.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

        }
        private void initializeMethodListBox()
        {
            #region Populate listbox with Proxomo Methods supported
            
            ProxomoMethodList.Items.Add("AppData Add");
            ProxomoMethodList.Items.Add("AppData Delete");
            ProxomoMethodList.Items.Add("AppData Get");
            ProxomoMethodList.Items.Add("AppData Get All");
            ProxomoMethodList.Items.Add("AppData Update");
            ProxomoMethodList.Items.Add("AppData Search");

            ProxomoMethodList.Items.Add("CustomData Add");
            ProxomoMethodList.Items.Add("CustomData Delete");
            ProxomoMethodList.Items.Add("CustomData Update");
            ProxomoMethodList.Items.Add("CustomData GetByID");
            ProxomoMethodList.Items.Add("CustomData Search");

            ProxomoMethodList.Items.Add("Event Add");
            ProxomoMethodList.Items.Add("Event Get");
            ProxomoMethodList.Items.Add("Event Update");
            ProxomoMethodList.Items.Add("Events Search By Distance");
            ProxomoMethodList.Items.Add("Events Search By PersonID");

            ProxomoMethodList.Items.Add("Event AppData Add");
            ProxomoMethodList.Items.Add("Event AppData Delete");
            ProxomoMethodList.Items.Add("Event AppData Get");
            ProxomoMethodList.Items.Add("Event AppData Get All");
            ProxomoMethodList.Items.Add("Event AppData Update");

            ProxomoMethodList.Items.Add("Event Comment Add");
            ProxomoMethodList.Items.Add("Event Comment Delete");
            ProxomoMethodList.Items.Add("Event Comments Get");
            ProxomoMethodList.Items.Add("Event Comment Update");
            
            ProxomoMethodList.Items.Add("Event Participants Get");
            ProxomoMethodList.Items.Add("Event Participant Invite");
            ProxomoMethodList.Items.Add("Event Participants Invite");
            ProxomoMethodList.Items.Add("Event Participants Delete");
            ProxomoMethodList.Items.Add("Event Request Invitation");
            ProxomoMethodList.Items.Add("Event RSVP");

            ProxomoMethodList.Items.Add("Friends Get");
            ProxomoMethodList.Items.Add("Friend Invite");
            ProxomoMethodList.Items.Add("Friend Invite By SocialNetwork");
            ProxomoMethodList.Items.Add("Friend Respond");
            ProxomoMethodList.Items.Add("Friends SocialNetwork Get");
            ProxomoMethodList.Items.Add("Friends SocialNetwork App Get");
            ProxomoMethodList.Items.Add("GeoCode By Address");
            ProxomoMethodList.Items.Add("Reverse GeoCode");
            ProxomoMethodList.Items.Add("GeoCode By IP Address");

            ProxomoMethodList.Items.Add("Location Add");
            ProxomoMethodList.Items.Add("Location Delete");
            ProxomoMethodList.Items.Add("Location Get");
            ProxomoMethodList.Items.Add("Location Update");
            ProxomoMethodList.Items.Add("Location Categories Get");
            ProxomoMethodList.Items.Add("Locations Search By Address");
            ProxomoMethodList.Items.Add("Locations Search By GPS");
            ProxomoMethodList.Items.Add("Locations Search By IP Address");

            ProxomoMethodList.Items.Add("Location AppData Add");
            ProxomoMethodList.Items.Add("Location AppData Delete");
            ProxomoMethodList.Items.Add("Location AppData Get");
            ProxomoMethodList.Items.Add("Location AppData Get All");
            ProxomoMethodList.Items.Add("Location AppData Update");

            ProxomoMethodList.Items.Add("Notification Send");
            
            ProxomoMethodList.Items.Add("Person Get");
            ProxomoMethodList.Items.Add("Person Update");
            ProxomoMethodList.Items.Add("Person AppData Add");
            ProxomoMethodList.Items.Add("Person AppData Delete");
            ProxomoMethodList.Items.Add("Person AppData Get");
            ProxomoMethodList.Items.Add("Person AppData Get All");
            ProxomoMethodList.Items.Add("Person AppData Update");
            ProxomoMethodList.Items.Add("Person Locations Get");
            ProxomoMethodList.Items.Add("Person SocialNetworkInfo Get");
            #endregion
        }
        private void clearResultsScreen(string msg)
        {
            List<string> empty = new List<string>();
            empty.Add(msg);
            OutputListBox.ItemsSource = empty;
        }
        #endregion

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            initializeButtonState();
            
            // This next call will be make to behave synchronously so it will nto return until Initialization is completed...
            WP7SDKInstance = new ProxomoApi(applicationID, ProxomoAPIkey, "V09", CommunicationType.JSON, true);
            
            ConnectionStatus.Text = "Connecting....";

            // Enable the front panel options so user can now run methods....
            Dispatcher.BeginInvoke(() => ConnectionStatus.Text = "Connected");
            Dispatcher.BeginInvoke(() => ProxomoMethodList.IsEnabled = true);
            Dispatcher.BeginInvoke(() => RunButton.IsEnabled = true);
            Dispatcher.BeginInvoke(() => Login.IsEnabled = true);
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            string methodToCall = ProxomoMethodList.SelectedItem.ToString();
            clearResultsScreen("Processing " + methodToCall + "...Please Wait");
            OutputSelectionInTextForm.Text = "";
            checkSDKInitialized();



            switch (methodToCall)
            {
                #region Appdata calls
                case "AppData Add":
                    AppData newRecord = new AppData();
                    newRecord.Key = "Some AppData Name on " + DateTime.Now.ToString();
                    newRecord.Value = "Some AppData Value here on " + DateTime.Now.ToString();
                    newRecord.ObjectType = "Fish Type" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> AppDataAdd_Callback = new ProxomoUserCallbackDelegate<string>(AppDataAdd_Complete);
                    WP7SDKInstance.AppDataAdd(newRecord, AppDataAdd_Callback);
                    break;
                case "AppData Delete":          
                    ProxomoUserCallbackDelegate<string> AppDataDelete_Callback = new ProxomoUserCallbackDelegate<string>(AppDataDelete_Complete);
                    WP7SDKInstance.AppDataDelete("WHViEPrUguKGq8W4", AppDataDelete_Callback); // the specific ID passed in can only be used once for testing!
                    break;
                case "AppData Get":
                    ProxomoUserCallbackDelegate<AppData> AppDataGet_Callback = new ProxomoUserCallbackDelegate<AppData>(AppDataGet_Complete);
                    WP7SDKInstance.AppDataGet("KHlimBfgeMBmE7pG", AppDataGet_Callback); // Also can use 8weFqMhjYPCvgLfs
                    break;
                case "AppData Get All":
                    ProxomoUserCallbackDelegate<List<AppData>> AppDataGetAll_Callback = new ProxomoUserCallbackDelegate<List<AppData>>(AppDataGetAll_Complete);
                    WP7SDKInstance.AppDataGetAll(AppDataGetAll_Callback);
                    break;
                case "AppData Update":
                    AppData updRecord = new AppData();
                    updRecord.ID = "8weFqMhjYPCvgLfs";
                    updRecord.Key = "Some AppData update value on " + DateTime.Now.ToString();
                    updRecord.Value = "Some updated Appdata Value on " + DateTime.Now.ToString();
                    updRecord.ObjectType = "Updated on" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> AppDataUpdate_Callback = new ProxomoUserCallbackDelegate<string>(AppDataUpdate_Complete);
                    WP7SDKInstance.AppDataAdd(updRecord, AppDataUpdate_Callback);
                    break;
                case "AppData Search":
                    ProxomoUserCallbackDelegate<List<AppData>> AppDataSearch_Callback = new ProxomoUserCallbackDelegate<List<AppData>>(AppDataSearch_Complete);
                    WP7SDKInstance.AppDataSearch("Fish Type", AppDataSearch_Callback);
                    break;

                #endregion

                #region CustomData calls
                case "CustomData Add":
                    Custom_TrainingRec financialRec = new Custom_TrainingRec("BabyBoomerTable", "", "02-21-11", "Bank Of St.David", 555.11, "Overtime", false, true, 3);
                    
                    ProxomoUserCallbackDelegate<string> CustomDataAdd_Callback = new ProxomoUserCallbackDelegate<string>(CustomDataAdd_Complete);
                    WP7SDKInstance.CustomDataAdd(financialRec, CustomDataAdd_Callback);
                    break;
                case "CustomData Delete":
                    ProxomoUserCallbackDelegate<string> CustomDataDelete_Callback = new ProxomoUserCallbackDelegate<string>(CustomDataDelete_Complete);
                    WP7SDKInstance.CustomDataDelete("BabyBoomerTable", "WHViEPrUguKGq8W4", CustomDataDelete_Callback); // the specific ID passed in can only be used once for testing!
                    break;
                case "CustomData Update":
                    Custom_TrainingRec financialRecUpd = new Custom_TrainingRec("BabyBoomerTable", "", "02-21-08", "Bank Of David", 123.45, "Holiday bonus", false, true, 3);

                    ProxomoUserCallbackDelegate<string> CustomDataUpdate_Callback = new ProxomoUserCallbackDelegate<string>(CustomDataUpdate_Complete);
                    WP7SDKInstance.CustomDataUpdate(financialRecUpd, CustomDataUpdate_Callback);
                    break;
                case "CustomData GetByID":
                    ProxomoUserCallbackDelegate<Custom_TrainingRec> CustomDataGet_Callback = new ProxomoUserCallbackDelegate<Custom_TrainingRec>(CustomDataGet_Complete);
                    WP7SDKInstance.CustomDataGet("BabyBoomerTable", "9TXZrVuyAW4x8GaX", CustomDataGet_Callback);
                    break;
                case "CustomData Search":
                    ProxomoUserCallbackDelegate<List<Custom_TrainingRec>> CustomDataSearch_Callback = new ProxomoUserCallbackDelegate<List<Custom_TrainingRec>>(CustomDataSearch_Complete);
                    ContinuationTokens ct = new ContinuationTokens("", "");
                    WP7SDKInstance.CustomDataSearch("BabyBoomerTable", "amount gt 1.0", 50, CustomDataSearch_Callback, ref ct);
                    break;

                #endregion

                #region Event calls

                case "Event Add":
                    // Debug from Fiddler using: POST
                    // https://127.0.0.1:444/V09/xml/event
                    // content:
                    // <Event><Description>The Statesman Capitol 10,000 is the largest 10K in Texas and the fifth largest in the nation. In its 35th year, the Capitol 10,000 attracts the silly to the serious and has become an annual rite of spring in Austin, Texas.</Description><EndTime>2012-03-25T00:00:00</EndTime><LastUpdate>0001-01-01T00:00:00</LastUpdate><MaxParticipants>10000</MaxParticipants><MinParticipants>200</MinParticipants><EventName>Statesman Capitol 10,000 35th Anniversary</EventName><PersonID>pLJVJbcgUeMum6RR</PersonID><PersonName>Hjalmar Perez</PersonName><Privacy>2</Privacy><StartTime>2012-03-25T00:00:00</StartTime><Status>0</Status><Latitude>30.272859</Latitude><Longitude>-97.741024</Longitude></Event>

                    #region Create and init new event
                    Event new_event = new Event();
                    new_event.Address1 = "Some address";
                    new_event.ImageURL = "some image here";
                    new_event.CountryName = "Not Panama";
                    new_event.Description = "The Panama Ironman is very humid!";
                    new_event.EndTime = Convert.ToDateTime("3/25/2012");
                    new_event.EventName = "Statesman Capitol 10,000 35th Anniversary";
                    new_event.Latitude = 30.272859;
                    new_event.Longitude = -97.741024;
                    new_event.MaxParticipants = 10000;
                    new_event.MinParticipants = 200;
                    new_event.PersonID = "pLJVJbcgUeMum6RR";
                    new_event.PersonName = "Hjalmar Perez";
                    new_event.Privacy = EventPrivacy.Open;
                    new_event.StartTime = Convert.ToDateTime("3/25/2012");
                    new_event.Status = EventStatus.Upcoming;
                    #endregion                    
                                        
                    ProxomoUserCallbackDelegate<string> EventAdd_Callback = new ProxomoUserCallbackDelegate<string>(EventAdd_Complete);
                    WP7SDKInstance.EventAdd(new_event, EventAdd_Callback);
                    break;
                case "Event Get":
                    ProxomoUserCallbackDelegate<Event> EventGet_Callback = new ProxomoUserCallbackDelegate<Event>(EventGet_Complete);
                    WP7SDKInstance.EventGet("TDfSLC5Ge1E2Xdov",EventGet_Callback); // 3Znr7CnkEo8OCqk7 = Bun Run 2011
                    break;
                case "Event Update":

                    #region Create and init new event
                    Event updEvent = new Event();
                    updEvent.ImageURL = "http://bunrun.com/";
                    updEvent.ID = "3Znr7CnkEo8OCqk7"; // Bun Run is event ID: "3Znr7CnkEo8OCqk7"

                    // MUST send in all required fields even ones that have not changed
                    updEvent.Description = "Updated " + DateTime.Now.ToString() + ". Please join Schlotzsky's and the YMBL for a fun day for everyone. ";
                    updEvent.EndTime = Convert.ToDateTime("05/09/2012");
                    updEvent.EventName = "Schlotzsky's Bun Run 2011";
                    updEvent.Latitude = 30.2669;
                    updEvent.Longitude = -97.7427;
                    updEvent.MaxParticipants = 900;
                    updEvent.MinParticipants = 2;
                    updEvent.PersonID = "pLJVJbcgUeMum6RR";
                    updEvent.PersonName = "Hjalmar Perez";
                    updEvent.Privacy = EventPrivacy.Open;
                    updEvent.StartTime = Convert.ToDateTime("10/12/2012");
                    updEvent.Status = EventStatus.Upcoming;
                    #endregion

                    ProxomoUserCallbackDelegate<string> EventUpdate_Callback = new ProxomoUserCallbackDelegate<string>(EventUpdate_Complete);
                    WP7SDKInstance.EventUpdate(updEvent, EventUpdate_Callback);
                    break;
                case "Events Search By Distance":
                    DateTime startTime = new DateTime(2010, 1, 1);
                    DateTime endTime = new DateTime(2012, 10, 10);

                    ProxomoUserCallbackDelegate<List<Event>> EventsSearchByDistance_Callback = new ProxomoUserCallbackDelegate<List<Event>>(EventsSearchByDistance_Complete);
                    WP7SDKInstance.EventsSearchByDistance(30, -97, 500, startTime, endTime, EventsSearchByDistance_Callback);
                    break;
                case "Events Search By PersonID":
                    DateTime startTime2 = new DateTime(2010, 1, 1);
                    DateTime endTime2 = new DateTime(2012, 10, 10);

                    ProxomoUserCallbackDelegate<List<Event>> EventsSearchByPersonID_Callback = new ProxomoUserCallbackDelegate<List<Event>>(EventsSearchByPersonID_Complete);
                    WP7SDKInstance.EventsSearchByPersonID("pLJVJbcgUeMum6RR", startTime2, endTime2, EventsSearchByPersonID_Callback);
                    break;

                #endregion

                #region EventComment calls

                case "Event Comment Add":
                    EventComment new_comment = new EventComment();
                    new_comment.Comment = "Test comment: " + DateTime.Now.ToString();
                    new_comment.EventID = "3Znr7CnkEo8OCqk7";
                    new_comment.PersonID = "pLJVJbcgUeMum6RR";
                    new_comment.PersonName = "Hjalmar Perez";

                    ProxomoUserCallbackDelegate<string> EventCommentAdd_Callback = new ProxomoUserCallbackDelegate<string>(EventCommentAdd_Complete);
                    WP7SDKInstance.EventCommentAdd(new_comment.EventID, new_comment, EventCommentAdd_Callback);
                    break;
                case "Event Comment Delete":
                    string eventID = "uvzewOelA69yo5Rg";
                    string commentID_toDelete = "Hxof9ind2v6ajBb1"; // Comment ID to delete... cannot run this method twice without changing

                    ProxomoUserCallbackDelegate<string> EventCommentDelete_Callback = new ProxomoUserCallbackDelegate<string>(EventCommentDelete_Complete);
                    WP7SDKInstance.EventCommentDelete(eventID, commentID_toDelete, EventCommentDelete_Callback);
                    break;
                case "Event Comments Get":
                    ProxomoUserCallbackDelegate<List<EventComment>> EventCommentsGet_Callback = new ProxomoUserCallbackDelegate<List<EventComment>>(EventCommentsGet_Complete);
                    WP7SDKInstance.EventCommentsGet("uvzewOelA69yo5Rg", EventCommentsGet_Callback);
                    break;
                case "Event Comment Update":
                    EventComment updated_EventComment = new EventComment();
                    updated_EventComment.ID = "YmhtdGlAgd6SaFvg";  // need to include this one for Proxomo to know which one to update
                    updated_EventComment.Comment = "Updated Test comment: " + DateTime.Now.ToString();
                    updated_EventComment.EventID = "3Znr7CnkEo8OCqk7";
                    updated_EventComment.PersonID = "pLJVJbcgUeMum6RR";
                    updated_EventComment.PersonName = "Hjalmar Perez";

                   ProxomoUserCallbackDelegate<string> EventCommentUpdate_Callback = new ProxomoUserCallbackDelegate<string>(EventCommentUpdate_Complete);
                    WP7SDKInstance.EventCommentUpdate(updated_EventComment.EventID, updated_EventComment, EventCommentUpdate_Callback);
                    break;

                #endregion

                #region Event Participant calls

                case "Event Participants Get":
                    ProxomoUserCallbackDelegate<List<EventParticipant>> EventParticipantsGet_Callback = new ProxomoUserCallbackDelegate<List<EventParticipant>>(EventParticipantsGet_Complete);
                    WP7SDKInstance.EventParticipantsGet("3Znr7CnkEo8OCqk7", EventParticipantsGet_Callback);
                    break;
                case "Event Participant Invite":
                    string invEventID = "3Znr7CnkEo8OCqk7"; // Bun Run
                    string invPersonID = "sxcTDuxAfMAblAVV";
                    // Daniel PersonID: sxcTDuxAfMAblAVV
                    // Doug PersonID: xcHXAvkWY9FtPM7S

                    ProxomoUserCallbackDelegate<string> EventParticipantInvite_Callback = new ProxomoUserCallbackDelegate<string>(EventParticipantInvite_Complete);
                    WP7SDKInstance.EventParticipantInvite(invEventID, invPersonID, EventParticipantInvite_Callback);
                    break;
                case "Event Participants Invite":
                    string[] personIDArray = { "sxcTDuxAfMAblAVV", "xcHXAvkWY9FtPM7S" }; // inviting Daniel and Doug
                    ProxomoUserCallbackDelegate<string> EventParticipantsInvite_Callback = new ProxomoUserCallbackDelegate<string>(EventParticipantsInvite_Complete);
                    WP7SDKInstance.EventParticipantsInvite("3Znr7CnkEo8OCqk7", personIDArray,EventParticipantsInvite_Callback);
                    break;
                case "Event Participants Delete":
                    // Debug from Fiddler using: DELETE
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/participant/{participantID}
                    string participantIDRecord = "enter valid value";
                    ProxomoUserCallbackDelegate<string> EventParticipantDelete_Callback = new ProxomoUserCallbackDelegate<string>(EventParticipantsDelete_Complete);
                    WP7SDKInstance.EventParticipantDelete("", participantIDRecord,EventParticipantDelete_Callback); // eventID not really used so can pass in empty string. Need to update the Participant record to delete
                    break;
                case "Event Request Invitation":
                    // Debug from Fiddler using: PUT
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/requestinvite/personid/9lKbnvIoT4Os9fyc
                    ProxomoUserCallbackDelegate<string> EventRequestInvitation_Callback = new ProxomoUserCallbackDelegate<string>(EventRequestInvitation_Complete);
                    WP7SDKInstance.EventRequestInvitation("3Znr7CnkEo8OCqk7", "9lKbnvIoT4Os9fyc",EventRequestInvitation_Callback);
                    break;
                case "Event RSVP":
                    ProxomoUserCallbackDelegate<string> EventRSVP_Callback = new ProxomoUserCallbackDelegate<string>(EventRSVP_Complete);
                    WP7SDKInstance.EventRSVP("3Znr7CnkEo8OCqk7", EventParticipantStatus.Attending, "sxcTDuxAfMAblAVV",EventRSVP_Callback); // Daniel RSVP
                    break;

                #endregion

                #region Event AppData calls

                case "Event AppData Add":
                    // Debug from Fiddler using: POST
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/appdata
                    // content:
                    // <AppData><Key>EventAppDataName:5/24/2011 2:41:05 PM</Key><Value>EventAppDataValue:5/24/2011 2:41:05 PM</Value><ObjectType>EventAppDataObjType:5/24/2011 2:41:05 PM</ObjectType></AppData>
                    AppData newaRecord = new AppData();
                    newaRecord.Key = "EventAppDataName:" + DateTime.Now.ToString();
                    newaRecord.Value = "EventAppDataValue:" + DateTime.Now.ToString();
                    newaRecord.ObjectType = "EventAppDataObjType:" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> EventAppDataAdd_Callback = new ProxomoUserCallbackDelegate<string>(EventAppDataAdd_Complete);
                    WP7SDKInstance.EventAppDataAdd("M3VLa4SBO7oWNvJL", newaRecord,EventAppDataAdd_Callback);
                    break;
                case "Event AppData Delete":
                    // Debug from Fiddler using:
                    // 
                    ProxomoUserCallbackDelegate<string> EventAppDataDelete_Callback = new ProxomoUserCallbackDelegate<string>(EventAppDataDelete_Complete);
                    WP7SDKInstance.EventAppDataDelete("", "",EventAppDataDelete_Callback); // will need to at least update AppData record ID during debug
                    break;
                case "Event AppData Get":
                    // Debug from Fiddler using: GET
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/appdata/9CpPWxZtKCvYlKtr
                    ProxomoUserCallbackDelegate<AppData> EventAppDataGet_Callback = new ProxomoUserCallbackDelegate<AppData>(EventAppDataGet_Complete);
                    WP7SDKInstance.EventAppDataGet("M3VLa4SBO7oWNvJL", "lw8p79z2fuWGlXtd",EventAppDataGet_Callback);
                    break;
                case "Event AppData Get All":
                    // Debug from Fiddler using: GET
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/appdata
                    ProxomoUserCallbackDelegate<List<AppData>> EventAppDataGetAll_Callback = new ProxomoUserCallbackDelegate<List<AppData>>(EventAppDataGetAll_Complete);
                    WP7SDKInstance.EventAppDataGetAll("M3VLa4SBO7oWNvJL",EventAppDataGetAll_Callback);
                    break;
                case "Event AppData Update":
                    // Debug from Fiddler using: PUT
                    // https://127.0.0.1:444/V09/xml/event/3Znr7CnkEo8OCqk7/appdata
                    // content:
                    // <AppData><ID>nCvRzaPIM4hsIAjA</ID><Key>UPDEventAppDataKey:5/24/2011 3:01:16 PM</Key><Value>UPDEventAppDataValue:5/24/2011 3:01:16 PM</Value><ObjectType>UPDEventAppDataObjType:5/24/2011 3:01:16 PM</ObjectType></AppData>
                    AppData updaRecord = new AppData();
                    updaRecord.ID = "lw8p79z2fuWGlXtd"; // May need to change during debugging!
                    updaRecord.Key = "UPDEventAppDataKey:" + DateTime.Now.ToString();
                    updaRecord.Value = "UPDEventAppDataValue:" + DateTime.Now.ToString();
                    updaRecord.ObjectType = "UPDEventAppDataObjType:" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> EventAppDataUpdate_Callback = new ProxomoUserCallbackDelegate<string>(EventAppDataUpdate_Complete);
                    WP7SDKInstance.EventAppDataUpdate("M3VLa4SBO7oWNvJL", updaRecord,EventAppDataUpdate_Callback);
                    break;

                #endregion

                #region Friend calls

                case "Friends Get":
                    ProxomoUserCallbackDelegate<List<Friend>> FriendsGet_Callback = new ProxomoUserCallbackDelegate<List<Friend>>(FriendsGet_Complete);
                    WP7SDKInstance.FriendsGet("pLJVJbcgUeMum6RR",FriendsGet_Callback);
                    break;
                case "Friend Invite":
                    ProxomoUserCallbackDelegate<string> FriendInvite_Callback = new ProxomoUserCallbackDelegate<string>(FriendInvite_Complete);
                    WP7SDKInstance.FriendInvite("pLJVJbcgUeMum6RR", "xcHXAvkWY9FtPM7S",FriendInvite_Callback);
                    break;
                case "Friend Invite By SocialNetwork":
                    ProxomoUserCallbackDelegate<string> FriendBySocialNetworkInvite_Callback = new ProxomoUserCallbackDelegate<string>(FriendBySocialNetworkInvite_Complete);
                    WP7SDKInstance.FriendBySocialNetworkInvite(SocialNetwork.Facebook, "pLJVJbcgUeMum6RR", "xcHXAvkWY9FtPM7S",FriendBySocialNetworkInvite_Callback);
                    break;
                case "Friend Respond":
                    ProxomoUserCallbackDelegate<string> FriendRespond_Callback = new ProxomoUserCallbackDelegate<string>(FriendRespond_Complete);
                    WP7SDKInstance.FriendRespond(FriendResponse.Accept, "sxcTDuxAfMAblAVV", "pLJVJbcgUeMum6RR",FriendRespond_Callback);
                    break;
                case "Friends SocialNetwork Get":
                    ProxomoUserCallbackDelegate<List<SocialNetworkFriend>> FriendsSocialNetworkGet_Callback = new ProxomoUserCallbackDelegate<List<SocialNetworkFriend>>(FriendsSocialNetworkGet_Complete);
                    WP7SDKInstance.FriendsSocialNetworkGet(SocialNetwork.Facebook, "pLJVJbcgUeMum6RR",FriendsSocialNetworkGet_Callback);
                    break;
                case "Friends SocialNetwork App Get":
                    ProxomoUserCallbackDelegate<List<SocialNetworkPFriend>> FriendsSocialNetworkAppGet_Callback = new ProxomoUserCallbackDelegate<List<SocialNetworkPFriend>>(FriendsSocialNetworkAppGet_Complete);
                    WP7SDKInstance.FriendsSocialNetworkAppGet(SocialNetwork.Facebook, "pLJVJbcgUeMum6RR",FriendsSocialNetworkAppGet_Callback);
                    break;

                #endregion

                #region GeoCode calls

                case "GeoCode By Address":
                    // Debug from Fiddler using any of:
                    // https://127.0.0.1:446/V09/xml/geo/lookup/address/78729
                    // https://127.0.0.1:446/V09/xml/geo/lookup/address/13220%20Marrero%20Dr.%20Austin%20TX%2078729
                    ProxomoUserCallbackDelegate<GeoCode> GeoCodebyAddress_Callback = new ProxomoUserCallbackDelegate<GeoCode>(GeoCodebyAddress_Complete);
                    WP7SDKInstance.GeoCodebyAddress("13218 Marrero Dr. Austin TX 78729",GeoCodebyAddress_Callback);
                    break;
                case "Reverse GeoCode":
                    // Debug from Fiddler using:
                    // https://127.0.0.1:452/V09/xml/geo/lookup/latitude/30.457146/longitude/-97.745941
                    ProxomoUserCallbackDelegate<Location> ReverseGeoCode_Callback = new ProxomoUserCallbackDelegate<Location>(ReverseGeoCode_Complete);
                    WP7SDKInstance.ReverseGeoCode("30.457146", "-97.745941",ReverseGeoCode_Callback); // coords for 13218 Marrero Dr. Austin TX 78729
                    break;
                case "GeoCode By IP Address":
                    // Debug from Fiddler using:
                    // https://127.0.0.1:446/V09/xml/geo/lookup/ip/76.183.57.69
                    ProxomoUserCallbackDelegate<GeoIP> GeoCodeByIPAddress_Callback = new ProxomoUserCallbackDelegate<GeoIP>(GeoCodeByIPAddress_Complete);
                    WP7SDKInstance.GeoCodeByIPAddress("76.183.57.69",GeoCodeByIPAddress_Callback);
                    break;

                #endregion

                #region Location calls

                case "Location Add":
                    Location newLocation = new Location();
                    newLocation.Name = "New North Central Fitness facility";
                    newLocation.Latitude = 30.123;
                    newLocation.Longitude = -97.4321;
                    newLocation.Address1 = "1234 Some Impostor Rd";
                    newLocation.City = "Austin";
                    newLocation.State = "TX";
                    newLocation.Zip = "78729";
                    newLocation.LocationSecurity = LocationSecurity.Open;
                    newLocation.PersonID = "pLJVJbcgUeMum6RR";

                    ProxomoUserCallbackDelegate<string> LocationAdd_Callback = new ProxomoUserCallbackDelegate<string>(LocationAdd_Complete);
                    WP7SDKInstance.LocationAdd(newLocation,LocationAdd_Callback);
                    break;
                case "Location Delete":
                    ProxomoUserCallbackDelegate<string> LocationDelete_Callback = new ProxomoUserCallbackDelegate<string>(LocationDelete_Complete);
                    WP7SDKInstance.LocationDelete("GqA5Yl6JRpMileob",LocationDelete_Callback);
                    break;
                case "Location Get":
                    ProxomoUserCallbackDelegate<Location> LocationGet_Callback = new ProxomoUserCallbackDelegate<Location>(LocationGet_Complete);
                    WP7SDKInstance.LocationGet("Gq465TKVuqeQw8fb",LocationGet_Callback);
                    break;
                case "Location Update":
                    Location updLocRecord = new Location();
                    updLocRecord.ID = "Gq465TKVuqeQw8fb";
                    updLocRecord.Name = DateTime.Now.ToString() + " - Milwood Test Loc 1";
                    updLocRecord.Latitude = 50.4525;
                    updLocRecord.Longitude = -90.756071;
                    updLocRecord.Address1 = "13220 Marrero Dr.";
                    updLocRecord.City = "Austin";
                    updLocRecord.State = "TX";
                    updLocRecord.Zip = "78729";
                    updLocRecord.LocationSecurity = LocationSecurity.Open;
                    updLocRecord.PersonID = "pLJVJbcgUeMum6RR";

                    ProxomoUserCallbackDelegate<string> LocationUpdate_Callback = new ProxomoUserCallbackDelegate<string>(LocationUpdate_Complete);
                    WP7SDKInstance.LocationUpdate(updLocRecord,LocationUpdate_Callback);
                    break;
                case "Location Categories Get":
                    ProxomoUserCallbackDelegate<List<Category>> LocationCategoriesGet_Callback = new ProxomoUserCallbackDelegate<List<Category>>(LocationCategoriesGet_Complete);
                    WP7SDKInstance.LocationCategoriesGet(LocationCategoriesGet_Callback);
                    break;

            #endregion

                #region Location search calls
                // Remember:
                // LocationSearchScope = Application Only >> searches only locations in the Proxomo Locations table
                // LocationSearchScope = Global  Only AND Person ID specified >> searches locations in Facebook Places
                // LocationSearchScope = Global  Only and NO Person ID specified >> searches locations in SimpleGeo *** this is what we are currently testing with the calls below!
                case "Locations Search By Address":
                    // Debug from Fiddler using: GET
                    // NO doubt this one worked from Fiddler on 5/25 at 1 pm
                    // https://127.0.0.1:452/V09/xml/locations/search?address=13220%20Marrero%20Dr.%20Austin%20TX%2078729&radius=100&maxresults=2
                    ProxomoUserCallbackDelegate<List<Location>> LocationsSearchByAddress_Callback = new ProxomoUserCallbackDelegate<List<Location>>(LocationsSearchByAddress_Complete);
                    WP7SDKInstance.LocationsSearchByAddress("13218 Marrero Dr. Austin TX 78729", LocationsSearchByAddress_Callback,"", "", 50, LocationSearchScope.GlobalOnly, 10, ""); // no personID specified
                    break;
                case "Locations Search By GPS":
                    // Debug from Fiddler using:
                    // https://127.0.0.1:453/V09/xml/locations/search/latitude/30.457146/longitude/-97.745941?radius=10&maxresults=2
                    ProxomoUserCallbackDelegate<List<Location>> LocationsSearchByGPS_Callback = new ProxomoUserCallbackDelegate<List<Location>>(LocationsSearchByGPS_Complete);
                    WP7SDKInstance.LocationsSearchByGPS("30", "-97",LocationsSearchByGPS_Callback, "", "", 10, LocationSearchScope.GlobalOnly, 2, "");  // no personID specified
                    break;
                case "Locations Search By IP Address":
                    // Debug from Fiddler using:
                    // https://127.0.0.1:452/V09/xml/locations/search/ip/76.183.57.69?radius=100&maxresults=10
                    ProxomoUserCallbackDelegate<List<Location>> LocationsSearchByIPAddress_Callback = new ProxomoUserCallbackDelegate<List<Location>>(LocationsSearchByIPAddress_Complete);
                    WP7SDKInstance.LocationsSearchByIPAddress("76.183.57.69", LocationsSearchByIPAddress_Callback,"", "", 100, LocationSearchScope.GlobalOnly, 10);
                    break;

                #endregion

                #region Location Appdata calls

                case "Location AppData Add":
                    // Debug from Fiddler using: POST
                    // https://127.0.0.1:444/V09/xml/location/yHU2CEiXgBteVltB/appdata
                    // content:
                    // <AppData><Key>LocAppDataName:5/24/2011 2:41:05 PM</Key><Value>LocAppDataValue:5/24/2011 2:41:05 PM</Value><ObjectType>LocAppDataObjType:5/24/2011 2:41:05 PM</ObjectType></AppData>
                    AppData newLocAppRecord = new AppData();
                    newLocAppRecord.Key = "LocAppDataName:" + DateTime.Now.ToString();
                    newLocAppRecord.Value = "LocAppDataValue:" + DateTime.Now.ToString();
                    newLocAppRecord.ObjectType = "LocAppDataObjType:" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> LocationAppDataAdd_Callback = new ProxomoUserCallbackDelegate<string>(LocationAppDataAdd_Complete);
                    WP7SDKInstance.LocationAppDataAdd("yHU2CEiXgBteVltB", newLocAppRecord, LocationAppDataAdd_Callback);
                    break;
                case "Location AppData Delete":
                    // Debug from Fiddler using:
                    // 
                    ProxomoUserCallbackDelegate<string> LocationAppDataDelete_Callback = new ProxomoUserCallbackDelegate<string>(LocationAppDataDelete_Complete);
                    WP7SDKInstance.LocationAppDataDelete("USPgv6umFZ76p4Pk", "",LocationAppDataDelete_Callback); // will need to at least update AppData record ID during debug
                    break;
                case "Location AppData Get":
                    // Debug from Fiddler using: GET
                    // https://127.0.0.1:444/V09/xml/location/yHU2CEiXgBteVltB/appdata/9CpPWxZtKCvYlKtr
                    ProxomoUserCallbackDelegate<AppData> LocationAppDataGet_Callback = new ProxomoUserCallbackDelegate<AppData>(LocationAppDataGet_Complete);
                    WP7SDKInstance.LocationAppDataGet("yHU2CEiXgBteVltB", "nCvRzaPIM4hsIAjA", LocationAppDataGet_Callback); 
                    break;
                case "Location AppData Get All":
                    // Debug from Fiddler using: GET
                    // https://127.0.0.1:444/V09/xml/location/yHU2CEiXgBteVltB/appdata
                    ProxomoUserCallbackDelegate<List<AppData>> LocationAppDataGetAll_Callback = new ProxomoUserCallbackDelegate<List<AppData>>(LocationAppDataGetAll_Complete);
                    WP7SDKInstance.LocationAppDataGetAll("yHU2CEiXgBteVltB",LocationAppDataGetAll_Callback);
                    break;
                case "Location AppData Update":
                    // Debug from Fiddler using: PUT
                    // https://127.0.0.1:444/V09/xml/location/yHU2CEiXgBteVltB/appdata
                    // content:
                    // <AppData><ID>nCvRzaPIM4hsIAjA</ID><Key>UPDLocAppDataKey:5/24/2011 3:01:16 PM</Key><Value>UPDLocAppDataValue:5/24/2011 3:01:16 PM</Value><ObjectType>UPDLocAppDataObjType:5/24/2011 3:01:16 PM</ObjectType></AppData>
                    AppData updLocAppRecord = new AppData();
                    updLocAppRecord.ID = "nCvRzaPIM4hsIAjA"; // May need to change during debugging!
                    updLocAppRecord.Key = "UPDLocAppDataKey:" + DateTime.Now.ToString();
                    updLocAppRecord.Value = "UPDLocAppDataValue:" + DateTime.Now.ToString();
                    updLocAppRecord.ObjectType = "UPDLocAppDataObjType:" + DateTime.Now.ToString();

                    ProxomoUserCallbackDelegate<string> LocationAppDataUpdate_Callback = new ProxomoUserCallbackDelegate<string>(LocationAppDataUpdate_Complete);
                    WP7SDKInstance.LocationAppDataUpdate("yHU2CEiXgBteVltB", updLocAppRecord, LocationAppDataUpdate_Callback);
                    break;

                #endregion

                #region Notification calls

                case "Notification Send":

                    Notification nnewRecord = new Notification();
                    nnewRecord.NotificationType = NotificationType.EventInvite;
                    nnewRecord.EMailSubject = "Hello Subject";
                    nnewRecord.EMailMessage = "Just a brief message to test this...";
                    nnewRecord.PersonID = "pLJVJbcgUeMum6RR";
                    nnewRecord.SendMethod = NotificationSendMethod.EMail;
                    nnewRecord.MobileMessage = ""; // until a bug is fixed in notification.vb line 26 which checks for the length of this string

  //                  WP7SDKInstance.NotificationSend(nnewRecord);

                    ProxomoUserCallbackDelegate<string> NotificationSend_Callback = new ProxomoUserCallbackDelegate<string>(NotificationSend_Complete);
                    WP7SDKInstance.NotificationSend(nnewRecord, NotificationSend_Callback);
                    break;

                #endregion

                #region Person calls

                case "Person Get":
                    ProxomoUserCallbackDelegate<Person> PersonGet_Callback = new ProxomoUserCallbackDelegate<Person>(PersonGet_Complete);
                    WP7SDKInstance.PersonGet("pLJVJbcgUeMum6RR", PersonGet_Callback);
                    break;
                case "Person Update":
                    Person new_record = new Person();
                    new_record.FullName = "Hjalmar I. Perez";
                    new_record.ID = "pLJVJbcgUeMum6RR";
                    new_record.EmailAddress = "hjalmar.perez@gmail.com";
                    new_record.EmailAlerts = true;
                    new_record.EmailVerificationStatus = VerificationStatus.Complete;

                    ProxomoUserCallbackDelegate<string> PersonUpdate_Callback = new ProxomoUserCallbackDelegate<string>(PersonUpdate_Complete);
                    WP7SDKInstance.PersonUpdate(new_record, PersonUpdate_Callback);
                    break;
                case "Person AppData Add":
                    AppData addRecord = new AppData();
                    addRecord.Key = "Favorite color";
                    addRecord.Value = "Superman Blue";
                    addRecord.ObjectType = "Preferences";

                    ProxomoUserCallbackDelegate<string> PersonAppDataAdd_Callback = new ProxomoUserCallbackDelegate<string>(PersonAppDataAdd_Complete);
                    WP7SDKInstance.PersonAppDataAdd("pLJVJbcgUeMum6RR", addRecord, PersonAppDataAdd_Callback);
                    break;
                case "Person AppData Delete":
                    ProxomoUserCallbackDelegate<string> PersonAppDataDelete_Callback = new ProxomoUserCallbackDelegate<string>(PersonAppDataDelete_Complete);
                    WP7SDKInstance.PersonAppDataDelete("pLJVJbcgUeMum6RR", "dFn2VhqvzBwqluo1", PersonAppDataDelete_Callback);
                    break;
                case "Person AppData Get":
                    ProxomoUserCallbackDelegate<AppData> PersonAppDataGet_Callback = new ProxomoUserCallbackDelegate<AppData>(PersonAppDataGet_Complete);
                    WP7SDKInstance.PersonAppDataGet("pLJVJbcgUeMum6RR", "ABP6H1JFpMD9gRuc", PersonAppDataGet_Callback);
                    break;
                case "Person AppData Get All":
                    ProxomoUserCallbackDelegate<List<AppData>> PersonAppDataGetAll_Callback = new ProxomoUserCallbackDelegate<List<AppData>>(PersonAppDataGetAll_Complete);
                    WP7SDKInstance.PersonAppDataGetAll("pLJVJbcgUeMum6RR", PersonAppDataGetAll_Callback);
                    break;
                case "Person AppData Update":

                    AppData updAppDataRecord = new AppData();
                    updAppDataRecord.ID = "ABP6H1JFpMD9gRuc";
                    updAppDataRecord.Key = "Favorite color";
                    updAppDataRecord.Value = "Pitch Black";
                    updAppDataRecord.ObjectType = "Preferences";

                    ProxomoUserCallbackDelegate<string> PersonAppDataUpdate_Callback = new ProxomoUserCallbackDelegate<string>(PersonAppDataUpdate_Complete);
                    WP7SDKInstance.PersonAppDataUpdate("pLJVJbcgUeMum6RR", updAppDataRecord, PersonAppDataUpdate_Callback);
                    break;
                case "Person Locations Get":
                    ProxomoUserCallbackDelegate<List<Location>> PersonLocationsGet_Callback = new ProxomoUserCallbackDelegate<List<Location>>(PersonLocationsGet_Complete);
                    WP7SDKInstance.PersonLocationsGet("pLJVJbcgUeMum6RR", PersonLocationsGet_Callback);
                    break;
                case "Person SocialNetworkInfo Get":
                    ProxomoUserCallbackDelegate<List<SocialNetworkInfo>> PersonSocialNetworkInfoGet_Callback = new ProxomoUserCallbackDelegate<List<SocialNetworkInfo>>(PersonSocialNetworkInfoGet_Complete);
                    WP7SDKInstance.PersonSocialNetworkInfoGet("pLJVJbcgUeMum6RR", SocialNetwork.Facebook, PersonSocialNetworkInfoGet_Callback);
                    break;

                #endregion

            }
        }


        private void OutputListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutputListBox.SelectedItem != null)
            {
                string tempstring = OutputListBox.SelectedItem.ToString();
                OutputSelectionInTextForm.Text = tempstring.Replace(", ", "\n");
            }
        } // see selected listbox row results in larger textbox



        private void OutputSelectionInTextForm_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            string queryString = string.Format("/Views/ProxomoLogin.xaml?applicationID={0}&authtoken={1}", applicationID, WP7SDKInstance.AuthToken.AccessToken);
            NavigationService.Navigate(new Uri(queryString, UriKind.Relative));
            }

    }
}