using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace="")]
    public partial class AppData
    {
	    [DataMember(Name = "ID")]
	    public string ID { get; set; }

	    [DataMember(Name = "Key")]
	    public string Key { get; set; }

	    [DataMember(Name = "Value")]
	    public string Value { get; set; }

	    [DataMember(Name = "ObjectType")]
	    public string ObjectType { get; set; }
}
}
