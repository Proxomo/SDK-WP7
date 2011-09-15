using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class SocialNetworkInfo
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "Key")]
        public string Key { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "SocialNetwork")]
        public SocialNetwork SocialNetwork { get; set; }

        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }
}