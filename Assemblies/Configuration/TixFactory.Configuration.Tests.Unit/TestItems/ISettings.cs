using System.ComponentModel;

namespace TixFactory.Configuration.Tests.Unit
{
	public interface ISettings : Configuration.ISettings
	{
		[DefaultValue(SettingsInitializerTests.DefaultStrValue)]
		string Str { get; set; }

		string StrWithoutDefaultValue { get; set; }
	}
}
