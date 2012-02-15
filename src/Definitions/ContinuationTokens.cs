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

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class ContinuationTokens
    {
        [DataMember(Name = "NextPartitionKey")]
        public string NextPartitionKey { get; set; }

        [DataMember(Name = "NextRowKey")]
        public string NextRowKey { get; set; }

        public ContinuationTokens(string NextPKey, string NextRKey)
        {
            NextPartitionKey = NextPKey;
            NextRowKey = NextRKey;
        }
    }

}
