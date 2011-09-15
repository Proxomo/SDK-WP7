using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class Event
    {
        [DataMember(Name = "AppData")]
        public List<AppData> AppData { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "EndTime")]
        public DateTime EndTime { get; set; }

        [DataMember(Name = "EventType")]
        public string EventType { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "ImageURL")]
        public string ImageURL { get; set; }

        [DataMember(Name = "LastUpdate")]
        public DateTime LastUpdate { get; set; }

        [DataMember(Name = "MaxParticipants")]
        public int MaxParticipants { get; set; }

        [DataMember(Name = "MinParticipants")]
        public int MinParticipants { get; set; }

        [DataMember(Name = "EventName")]
        public string EventName { get; set; }

        [DataMember(Name = "Notes")]
        public string Notes { get; set; }

        [DataMember(Name = "PersonID")]
        public string PersonID { get; set; }

        [DataMember(Name = "PersonName")]
        public string PersonName { get; set; }

        [DataMember(Name = "Privacy")]
        public EventPrivacy Privacy { get; set; }

        [DataMember(Name = "StartTime")]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "Status")]
        public EventStatus Status { get; set; }

        [DataMember(Name = "LocationID")]
        public string LocationID { get; set; }

        [DataMember(Name = "Latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "Address1")]
        public string Address1 { get; set; }

        [DataMember(Name = "Address2")]
        public string Address2 { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "State")]
        public string State { get; set; }

        [DataMember(Name = "Zip")]
        public string Zip { get; set; }

        [DataMember(Name = "CountryName")]
        public string CountryName { get; set; }

        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }
    }
}