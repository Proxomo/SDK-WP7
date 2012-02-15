using System;
using System.Runtime.Serialization;

namespace Proxomo
{
        [DataContract(Namespace = "")]
        public partial class LoginResults
        {
            [DataMember(Name = "Result")]
            public LoginResult Result { get; set; }

            [DataMember(Name = "Error")]
            public string Error { get; set; }

            [DataMember(Name = "PersonID")]
            public string PersonID { get; set; }

            [DataMember(Name = "Socialnetwork")]
            public SocialNetwork Socialnetwork { get; set; }

            [DataMember(Name = "Socialnetwork_id")]
            public string Socialnetwork_id { get; set; }

            [DataMember(Name = "Access_token")]
            public string Access_token { get; set; }

            [DataMember(Name = "Absolute_uri")]
            public string Absolute_uri { get; set; }

        }
}
