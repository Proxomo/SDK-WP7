using System;
using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class EventComment
    {
        [DataMember(Name = "EventID")]
        public string EventID { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "PersonName")]
        public string PersonName { get; set; }

        [DataMember(Name = "Comment")]
        public string Comment { get; set; }

        [DataMember(Name = "LastUpdate")]
        public DateTime LastUpdate { get; set; }
    }
}