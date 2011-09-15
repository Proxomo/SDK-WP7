using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Proxomo
{

	[DataContract()]
	public enum LocationSearchScope: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		All = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		GlobalOnly = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		ApplicationOnly = 2
	}

	[DataContract()]
	public enum LocationSecurity: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		Open = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Private = 1
	}

	[DataContract()]
	public enum NotificationType: int
	{

		[XmlEnum("0"), EnumMember(Value="0")]
		EventInvite = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		EventRequest = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		SystemMessage = 2,

		[XmlEnum("3"), EnumMember(Value="3")]
		ApplicationMessage = 3,

		[XmlEnum("4"), EnumMember(Value="4")]
		FriendInvitation = 4,

		[XmlEnum("5"), EnumMember(Value="5")]
		Other = 5,

		[XmlEnum("6"), EnumMember(Value="6")]
		Verification = 6

	}

	[DataContract()]
	public enum NotificationSendMethod: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		All = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		EMail = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		SMS = 2,

		[XmlEnum("3"), EnumMember(Value="3")]
		UserDefined = 3
	}

	[DataContract()]
	public enum EventStatus: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		Upcoming = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Complete = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Canceled = 2
	}

	[DataContract()]
	public enum EventPrivacy: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		Secret = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Closed = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Open = 2
	}

	[DataContract()]
	public enum EventParticipantStatus: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		NoReply = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Maybe = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Invited = 2,

		[XmlEnum("3"), EnumMember(Value="3")]
		Attending = 3,

		[XmlEnum("4"), EnumMember(Value="4")]
		Declined = 4,

		[XmlEnum("5"), EnumMember(Value="5")]
		RequestInvitation = 5,

		[XmlEnum("6"), EnumMember(Value="6")]
		RequestDeclined = 6

	}

	[DataContract()]
	public enum FriendStatus: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		None = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		InvitationSent = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Approved = 2,

		[XmlEnum("3"), EnumMember(Value="3")]
		Ignored = 3,

		[XmlEnum("4"), EnumMember(Value="4")]
		IncomingInvitation = 4,

		[XmlEnum("5"), EnumMember(Value="5")]
		InvitationIgnored = 5
	}

	[DataContract()]
	public enum FriendResponse: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		Ignore = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Accept = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Cancel = 2
	}

	[DataContract()]
	public enum SocialNetwork: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		Facebook = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Twitter = 1
	}

	[DataContract()]
	public enum CommunicationType: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		XML = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		JSON = 1
	}

	[DataContract()]
	public enum VerificationStatus: int
	{
		[XmlEnum("0"), EnumMember(Value="0")]
		None = 0,

		[XmlEnum("1"), EnumMember(Value="1")]
		Sent = 1,

		[XmlEnum("2"), EnumMember(Value="2")]
		Complete = 2,

		[XmlEnum("3"), EnumMember(Value="3")]
		Error = 3
	}

    [DataContract()]
    public enum LoginResult : int
    {
        [XmlEnum("0"), EnumMember(Value = "0")]
        Error = 0,

        [XmlEnum("1"), EnumMember(Value = "1")]
        Success = 1,

        [XmlEnum("2"), EnumMember(Value = "2")]
        Cancel = 2
    }


}
