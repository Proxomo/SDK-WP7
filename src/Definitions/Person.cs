using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Person
    {
        [DataMember(Name = "AppData")]
        public List<AppData> AppData { get; set; }

        [DataMember(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "EmailAlerts")]
        public bool EmailAlerts { get; set; }

        [DataMember(Name = "EmailVerificationCode")]
        public string EmailVerificationCode { get; set; }

        [DataMember(Name = "EmailVerificationStatus")]
        public VerificationStatus EmailVerificationStatus { get; set; }

        [DataMember(Name = "EmailVerified")]
        public bool EmailVerified { get; set; }

        [DataMember(Name = "FacebookID")]
        public string FacebookID { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "FullName")]
        public string FullName { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "ImageURL")]
        public string ImageURL { get; set; }

        [DataMember(Name = "LastLogin")]
        public DateTime LastLogin { get; set; }

        [DataMember(Name = "LastName")]
        public string LastName { get; set; }

        [DataMember(Name = "MobileAlerts")]
        public bool MobileAlerts { get; set; }

        [DataMember(Name = "MobileNumber")]
        public string MobileNumber { get; set; }

        [DataMember(Name = "MobileVerificationCode")]
        public string MobileVerificationCode { get; set; }

        [DataMember(Name = "MobileVerificationStatus")]
        public VerificationStatus MobileVerificationStatus { get; set; }

        [DataMember(Name = "MobileVerified")]
        public bool MobileVerified { get; set; }

        [DataMember(Name = "TwitterID")]
        public string TwitterID { get; set; }

        [DataMember(Name = "UserName")]
        public string UserName { get; set; }

        [DataMember(Name = "UTCOffset")]
        public double UTCOffset { get; set; }
    }
}