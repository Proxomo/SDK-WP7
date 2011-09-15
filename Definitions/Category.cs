using System.Runtime.Serialization;

namespace Proxomo
{
    [DataContract(Namespace="")]
    public partial class Category
    {
	    [DataMember(Name="category")]
	    public string category {get; set;}

	    [DataMember(Name="type")]
	    public string type {get; set;}

	    [DataMember(Name="subcategory")]
	    public string subcategory {get; set;}
    }
}


