using System.Linq;
using NUnit.Framework;

namespace TixFactory.Configuration.Tests.Unit;

[TestFixture]
public class SettingsInitializerTests
{
    internal const string DefaultStrValue = "Hello, world!";

    private SettingsInitializer _SettingsInitializer;

    [SetUp]
    public void Setup()
    {
        var baselessSettingValueSource = new BaselessSettingValueSource();
        _SettingsInitializer = new SettingsInitializer(baselessSettingValueSource);
    }

    [Test]
    public void Test()
    {
        var settings = _SettingsInitializer.CreateFromInterface<ISettings>();
        Assert.That(settings.Str, Is.EqualTo(DefaultStrValue));

        var newStrValue = new string(settings.Str.Reverse().ToArray());
        settings.Str = newStrValue;
        Assert.That(settings.Str, Is.EqualTo(newStrValue));

        Assert.That(settings.StrWithoutDefaultValue, Is.EqualTo(default(string)));

        settings.StrWithoutDefaultValue = newStrValue;
        Assert.That(settings.StrWithoutDefaultValue, Is.EqualTo(newStrValue));
    }
}
