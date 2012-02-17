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

using System.Text.RegularExpressions;

namespace ProxomoWP7Demo.Views
{
    public partial class Page1 : PhoneApplicationPage
    {
       
        private void NewProxomoLogin_Complete(Proxomo.ItemCompletedEventArgs<Proxomo.LoginResults> args)
        {
            Proxomo.LoginResults outcome = (Proxomo.LoginResults)args.Result;

            string resultMessage = string.Format("Result:{0} \nError (if app.):{1} \nPersonID:{2} \nSocial Network:{3} \nSocial Network ID:{4} \nAccess Token:{5}", outcome.Result, outcome.Error, outcome.PersonID, outcome.Socialnetwork, outcome.Socialnetwork_id, outcome.Access_token);
            NavigationService.Navigate(new Uri("/Views/ProxomoLoginResult.xaml?message=" + resultMessage, UriKind.Relative));
        }

        
        public Page1()
        {
            InitializeComponent();
            try
            {
                ProxomoLoginBrowser.appID = "nngAqYvGWMM9EwyO";//this.NavigationContext.QueryString["applicationID"];  //"nngAqYvGWMM9EwyO";
                ProxomoLoginBrowser.authtoken = "nlbUw97E7xb%2b6Z5XU6fJPdyUwUzPRVlEVOpn198EMeM%3d";//"6OMcNtYu2GoJsRZtu9sxnRMkkg263QHwPK/dCzbcej4=";//this.NavigationContext.QueryString["authtoken"];  //"nlbUw97E7xb%2b6Z5XU6fJPdyUwUzPRVlEVOpn198EMeM%3d";
                ProxomoLoginBrowser.Login_Complete += new Proxomo.ProxomoLoginControl.LoginOutcomeEventHandler(NewProxomoLogin_Complete);
            }
            catch (Exception ex)
            {
            }


        }
    }
}