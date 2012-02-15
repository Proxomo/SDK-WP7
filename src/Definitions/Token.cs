using System;
using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Token
    {
        [DataMember(Name = "AccessToken")]
        public string AccessToken { get; set; }

        [DataMember(Name = "Expires")]
        public double Expires { get; set; }

        [DataMember(Name = "ExpiresDate")]
        public DateTime ExpiresDate { get; set; }
    }
}