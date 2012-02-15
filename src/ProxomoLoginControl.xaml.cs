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

using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;

namespace Proxomo
{
    public partial class ProxomoLoginControl : UserControl
    {
        public delegate void LoginOutcomeEventHandler(ItemCompletedEventArgs<LoginResults> args);

        // Define Login Outcome event that caller can register for
        public event LoginOutcomeEventHandler Login_Complete;

        public ProxomoLoginControl()
        {
            InitializeComponent();
        }

        private string m_appID = "";
        private string m_authtoken = "";

        public string appID
        {
            set { m_appID = value; }
        }

        public string authtoken
        {
            set { m_authtoken = value; }
        }

        void LoginBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                
                if (e.Uri.AbsoluteUri.Contains("https://service.proxomo.com/LoginComplete.aspx?result") == true)
                {
                    if (this.Login_Complete != null)
                    {
                        this.Login_Complete(new ItemCompletedEventArgs<LoginResults> { Error = null, Result = ResultFromQueryString(e.Uri.AbsoluteUri) });
                    }
                }

            }

            catch (Exception ex)
            {
                this.Login_Complete(new ItemCompletedEventArgs<LoginResults> { Error = ex, Result = new LoginResults { Result = LoginResult.Error, Error = ex.Message } });
            }
        }

        private LoginResults ResultFromQueryString(string url)
        {
            LoginResults lresult = new LoginResults();

            Dictionary<string,string> qparams = ParseQueryString(url);

            if (qparams.Keys.Contains("access_token")) { lresult.Access_token = qparams["access_token"];};
            if (qparams.Keys.Contains("personID")) { lresult.PersonID = qparams["personID"]; };
            if (qparams.Keys.Contains("error")) { lresult.Error = qparams["error"]; };
            if (qparams.Keys.Contains("socialnetwork_id")) { lresult.Socialnetwork_id = qparams["socialnetwork_id"]; };

            if (qparams.Keys.Contains("socialnetwork")) 
            {
                switch(qparams["socialnetwork"])
                {
                    case "0":
                        lresult.Socialnetwork = SocialNetwork.Facebook;
                        break;
                    case "1":
                        lresult.Socialnetwork = SocialNetwork.Twitter;
                        break;
                }

            };
            
           if (qparams.Keys.Contains("result")) 
            {
                switch(qparams["result"])
                {
                    case "success":
                        lresult.Result = LoginResult.Success;
                        break;

                    case "error":
                        lresult.Result = LoginResult.Error;
                        break;

                    case "cancel":
                        lresult.Result = LoginResult.Cancel;
                        break;
                }
            };
            
            return lresult;
        }

        public static Dictionary<string,string> ParseQueryString(string s)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            foreach (string vp in Regex.Split( s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2)
                {
                    dict.Add(singlePair[0], singlePair[1]);
                }
                else
                {
                    // only one key with no value specified in query string
                    dict.Add(singlePair[0], string.Empty);
                }
            }

            return dict;
        }

        private void LoginBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string ProxomoLoginUri = Uri.EscapeUriString(string.Format("https://service.proxomo.com/login.aspx?application_id={0}&display_type=mobile&auth_token={1}", m_appID, m_authtoken));
                this.LoginBrowser.Navigate(new Uri(ProxomoLoginUri));
            }
            catch (Exception ex)
            {
                this.Login_Complete(new ItemCompletedEventArgs<LoginResults> { Error = ex, Result = new LoginResults { Result = LoginResult.Error, Error = ex.Message } });
            }

        }

    }
}
