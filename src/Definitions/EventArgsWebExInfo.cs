//using System;
//using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.Runtime.Serialization;

// This class was created to support deserializing the Exception response when a WebException occurs during a call to the Proxomo service.
// Details: The purpose of this class is to provide an object to the WebException catch statement in the GetResponseItem_Callback function in
// order to perform the deserialization of the WebException response.
namespace Proxomo
{
    [DataContract(Namespace = "")]
    public class Error
    {
        [DataMember(Name = "Message")]
        public string Message { get; set; }

        [DataMember(Name = "Status")]
        public int Status { get; set; }
    }
}
