using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class SocialNetworkPFriend : SocialNetworkFriend
    {
        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }
    }
}