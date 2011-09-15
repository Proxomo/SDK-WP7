using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace = "")]
    public partial class GeoIP
    {
        [DataMember(Name = "IP")]
        public string IP { get; set; }

        [DataMember(Name = "Latitude")]
        public string Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public string Longitude { get; set; }
    }
}