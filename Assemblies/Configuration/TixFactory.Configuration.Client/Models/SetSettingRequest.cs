using System.Runtime.Serialization;

namespace TixFactory.Configuration.Client
{
	[DataContract]
	internal class SetSettingRequest
	{
		[DataMember(Name = "applicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "settingName")]
		public string SettingName { get; set; }

		[DataMember(Name = "settingValue")]
		public string SettingValue { get; set; }
	}
}
