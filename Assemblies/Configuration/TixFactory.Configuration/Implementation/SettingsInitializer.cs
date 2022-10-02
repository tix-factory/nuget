using Castle.Core.Internal;
using FakeItEasy;
using FakeItEasy.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TixFactory.Configuration
{
	/// <inheritdoc cref="ISettingsInitializer"/>
	public class SettingsInitializer : ISettingsInitializer
	{
		private const string _ValuePropertyName = "Value";

		private readonly ISettingValueSource _SettingValueSource;
		private readonly MethodInfo _TryGetSettingValueMethod;
		private readonly MethodInfo _WriteSettingValueMethod;

		/// <summary>
		/// Initializes a new <see cref="SettingsInitializer"/>.
		/// </summary>
		/// <param name="settingValueSource">An <see cref="ISettingValueSource"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="settingValueSource"/>
		/// </exception>
		public SettingsInitializer(ISettingValueSource settingValueSource)
		{
			_SettingValueSource = settingValueSource ?? throw new ArgumentNullException(nameof(settingValueSource));
			
			var settingValueSourceType = typeof(ISettingValueSource);
			_TryGetSettingValueMethod = settingValueSourceType.GetMethod(nameof(ISettingValueSource.TryGetSettingValue));
			_WriteSettingValueMethod = settingValueSourceType.GetMethod(nameof(ISettingValueSource.WriteSettingValue));
		}

		/// <inheritdoc cref="ISettingsInitializer.CreateFromInterface{TSettingsInterface}"/>
		public TSettingsInterface CreateFromInterface<TSettingsInterface>()
			where TSettingsInterface : ISettings
		{
			var settingsType = typeof(TSettingsInterface);
			if (!settingsType.IsInterface)
			{
				throw new ArgumentException("Interface type must be an interface.", nameof(TSettingsInterface));
			}

			var groupName = GetSettingsGroup(settingsType);
			var individualSettings = LoadIndividualSettings(settingsType, groupName);
			var fakedImplementation = Create.Fake(settingsType);

			A.CallTo(fakedImplementation).Where(call => call.Method.Name.StartsWith("get_")).WithNonVoidReturnType().ReturnsLazily(call =>
			{
				var property = GetProperty(settingsType, call.Method);
				var individualSetting = individualSettings[property.Name];
				var settingValueProperty = GetIndividualSettingValueProperty(property);
				return settingValueProperty.GetValue(individualSetting);
			});

			A.CallTo(fakedImplementation).Where(call => call.Method.Name.StartsWith("set_")).Invokes(call =>
			{
				var property = GetProperty(settingsType, call.Method);
				var writeSettingValue = _WriteSettingValueMethod.MakeGenericMethod(property.PropertyType);
				writeSettingValue.Invoke(_SettingValueSource, new[] { groupName, property.Name, call.Arguments[0] });
			});

			A.CallTo(fakedImplementation).Where(call => call.Method.Name.StartsWith(nameof(ISettings.ExtractSetting))).WithNonVoidReturnType().ReturnsLazily(call =>
			{
				var settingName = call.GetArgument<string>(0);
				var settingProperty = settingsType.GetProperty(settingName);
				if (settingProperty == null)
				{
					throw new ArgumentException($"{settingName} is not valid property on {settingsType.Name}", nameof(settingName));
				}

				var expectedPropertyType = call.Method.GetGenericArguments().FirstOrDefault();
				if (expectedPropertyType != settingProperty.PropertyType)
				{
					throw new ArgumentException("Expected generic argument to match setting property type.", "T");
				}

				return individualSettings[settingName];
			});

			_SettingValueSource.SettingValueChanged += (changedGroupName, changedSettingName) =>
			{
				if (changedGroupName != groupName)
				{
					return;
				}

				var settingProperty = settingsType.GetProperty(changedSettingName);
				if (settingProperty != null && individualSettings.TryGetValue(changedSettingName, out var individualSetting))
				{
					if (TryGetSettingValue(groupName, settingProperty, out var settingValue))
					{
						var settingValueProperty = GetIndividualSettingValueProperty(settingProperty);
						settingValueProperty.SetValue(individualSetting, settingValue);
					}
				}
			};

			return (TSettingsInterface)fakedImplementation;
		}

		private string GetSettingsGroup(Type settingsType)
		{
			var categoryAttribute = settingsType.GetAttribute<CategoryAttribute>();
			if (!string.IsNullOrWhiteSpace(categoryAttribute?.Category))
			{
				return categoryAttribute.Category;
			}

			return settingsType.Namespace;
		}

		private PropertyInfo GetProperty(Type interfaceType, MethodInfo methodInfo)
		{
			return interfaceType.GetProperty(methodInfo.Name.Substring(4));
		}

		private bool TryGetSettingValue(string groupName, PropertyInfo settingProperty, out object settingValue)
		{
			// https://stackoverflow.com/a/569267/1663648
			var tryGetSettingValue = _TryGetSettingValueMethod.MakeGenericMethod(settingProperty.PropertyType);
			var parameters = new object[] { groupName, settingProperty.Name, null };
			if (tryGetSettingValue.Invoke(_SettingValueSource, parameters) is bool success && success)
			{
				settingValue = parameters[2];
				return true;
			}

			settingValue = null;
			return false;
		}

		private object GetDefaultValue(PropertyInfo settingProperty, string groupName)
		{
			if (TryGetSettingValue(groupName, settingProperty, out var settingValue))
			{
				return settingValue;
			}

			var defaultValueAttribute = settingProperty.GetAttribute<DefaultValueAttribute>();
			if (defaultValueAttribute != null)
			{
				return Convert.ChangeType(defaultValueAttribute.Value, settingProperty.PropertyType);
			}

			if (settingProperty.PropertyType.GetTypeInfo().IsValueType)
			{
				return Activator.CreateInstance(settingProperty.PropertyType);
			}

			return null;
		}

		private IReadOnlyDictionary<string, object> LoadIndividualSettings(Type settingsType, string groupName)
		{
			var individualSettings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			foreach (var settingProperty in settingsType.GetProperties())
			{
				var individualSettingType = typeof(Setting<>).MakeGenericType(settingProperty.PropertyType);
				var individualSettingConstructor = individualSettingType.GetConstructors().First(c => c.GetParameters().Length == 1);
				var individualSetting = individualSettingConstructor.Invoke(new[] { GetDefaultValue(settingProperty, groupName) });

				individualSettings[settingProperty.Name] = individualSetting;
			}

			return individualSettings;
		}

		private PropertyInfo GetIndividualSettingValueProperty(PropertyInfo settingProperty)
		{
			var individualSettingInterfaceType = typeof(ISetting<>).MakeGenericType(settingProperty.PropertyType);
			return individualSettingInterfaceType.GetProperty(_ValuePropertyName);
		}
	}
}
