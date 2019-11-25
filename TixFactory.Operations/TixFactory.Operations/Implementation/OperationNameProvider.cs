using System;
using System.ComponentModel;
using System.Reflection;

namespace TixFactory.Operations
{
	/// <inheritdoc cref="IOperationNameProvider"/>
	public class OperationNameProvider : IOperationNameProvider
	{
		private const string _OperationSuffix = "Operation";

		/// <inheritdoc cref="IOperationNameProvider.GetOperationName"/>
		public string GetOperationName(Type operationType)
		{
			if (operationType == null)
			{
				throw new ArgumentNullException(nameof(operationType));
			}

			var name = operationType.Name;

			var displayName = operationType.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
			if (!string.IsNullOrWhiteSpace(displayName?.DisplayName))
			{
				name = displayName.DisplayName;
			}
			else if (name.EndsWith(_OperationSuffix))
			{
				name = name.Substring(0, name.Length - _OperationSuffix.Length);
			}

			return name;
		}
	}
}
