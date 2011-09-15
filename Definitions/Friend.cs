using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract()]
    public partial class Friend
    {
        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        public string LastName { get; set; }

        [DataMember(Name = "FullName")]
        public string FullName { get; set; }

        [DataMember(Name = "FriendStatus")]
        public FriendStatus FriendStatus { get; set; }

        [DataMember(Name = "TwitterID")]
        public string TwitterID { get; set; }

        [DataMember(Name = "FacebookID")]
        public string FacebookID { get; set; }

        [DataMember(Name = "ImageURL")]
        public string ImageURL { get; set; }
    }
}