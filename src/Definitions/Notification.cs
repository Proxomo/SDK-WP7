using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Notification
    {
        [DataMember(Name = "EMailMessage")]
        public string EMailMessage { get; set; }

        [DataMember(Name = "EMailSubject")]
        public string EMailSubject { get; set; }

        [DataMember(Name = "MobileMessage")]
        public string MobileMessage { get; set; }

        [DataMember(Name = "NotificationType")]
        public NotificationType NotificationType { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "SendMethod")]
        public NotificationSendMethod SendMethod { get; set; }
    }
}