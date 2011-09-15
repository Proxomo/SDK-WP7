
namespace Proxomo
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "")]
    public partial class SocialNetworkFriend
    {
        [DataMember(Name = "FullName")]
        public string FullName { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "ImageURL")]
        public string ImageURL { get; set; }

        [DataMember(Name = "Link")]
        public string Link { get; set; }
    }
}