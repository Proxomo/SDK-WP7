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
    public class ItemCompletedEventArgs<T>
    {
      
        public bool IsError { get; set; }  
        public int HttpRespCode {get; set;}
        public string HttpRespMessage { get; set;}
        public Exception Error { get; set;}
        public object UserData { get; set;} // to allow user to pass any data through to the delegate
        
        public T Result { get; set; }
        public ContinuationTokens cTokens { get; set; }
    }
}
