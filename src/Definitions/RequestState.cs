using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Proxomo
{
    internal class RequestStateItem<t>
    {
        internal ProxomoUserCallbackDelegate<t> UserCallback { get; set; }
        internal object UserData { get; set; } 

        internal ItemCompletedEventArgs<t> Result { get; set; }
        internal string Url { get; set; }
        internal string Method { get; set; }
        internal string ContentType { get; set; }
        internal string Content { get; set; }
        internal HttpWebRequest Request { get; set; }
        internal HttpWebResponse Response { get; set; }
    }
}
