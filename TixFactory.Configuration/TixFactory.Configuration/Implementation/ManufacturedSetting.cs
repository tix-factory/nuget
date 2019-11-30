using System;

namespace TixFactory.Configuration
{
	/// <inheritdoc cref="IManufacturedSetting{T}"/>
	public class ManufacturedSetting<T> : IManufacturedSetting<T>
	{
		private readonly bool _RefreshOnRead;
		private readonly Func<T> _ValueFactory;
		private bool _ValueLoaded = false;
		private T _CurrentValue;

		/// <inheritdoc cref="IReadOnlySetting{T}.Changed"/>
		public event Action<T, T> Changed;

		/// <inheritdoc cref="ISetting{T}.Value"/>
		public T Value
		{
			get
			{
				if (_RefreshOnRead || !_ValueLoaded)
				{
					Refresh();
				}

				return _CurrentValue;
			}
		}
		
		/// <summary>
		/// Initializes a new <see cref="Setting{T}"/> with a default <see cref="Value"/>.
		/// </summary>
		public ManufacturedSetting(Func<T> valueFactory, bool refreshOnRead)
		{
			_ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
			_RefreshOnRead = refreshOnRead;
		}

		/// <inheritdoc cref="IManufacturedSetting{T}.Refresh"/>
		public virtual void Refresh()
		{
			var originalValue = _CurrentValue;
			var newValue = _ValueFactory();

			if (!newValue.Equals(originalValue))
			{
				_CurrentValue = newValue;
				_ValueLoaded = true;
				Changed?.Invoke(newValue, originalValue);
			}
		}
	}
}
