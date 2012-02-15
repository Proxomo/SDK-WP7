using System;
using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Task
    {
        [DataMember(Name = "Enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "Friday")]
        public bool Friday { get; set; }

        [DataMember(Name = "Hour")]
        public int Hour { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "LastRunUTC")]
        public DateTime LastRunUTC { get; set; }

        [DataMember(Name = "LastRunResult")]
        public TaskResult LastRunResult { get; set; }

        [DataMember(Name = "LastRunMessage")]
        public string LastRunMessage { get; set; }

        [DataMember(Name = "Method")]
        public TaskMethod Method { get; set; }

        [DataMember(Name = "MethodValue")]
        public string MethodValue { get; set; }

        [DataMember(Name = "Minute")]
        public int Minute { get; set; }

        [DataMember(Name = "Monday")]
        public bool Monday { get; set; }

        [DataMember(Name = "NextRunUTC")]
        public DateTime NextRunUTC { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "Saturday")]
        public bool Saturday { get; set; }

        [DataMember(Name = "Sunday")]
        public bool Sunday { get; set; }

        [DataMember(Name = "TaskType")]
        public TaskType TaskType { get; set; }

        [DataMember(Name = "Thursday")]
        public bool Thursday { get; set; }

        [DataMember(Name = "TimeZone")]
        public USTimeZone TimeZone { get; set; }

        [DataMember(Name = "Tuesday")]
        public bool Tuesday { get; set; }

        [DataMember(Name = "Wednesday")]
        public bool Wednesday { get; set; }
    }
}
