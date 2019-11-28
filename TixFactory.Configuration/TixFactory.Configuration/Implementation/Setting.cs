using System;

namespace TixFactory.Configuration
{
	/// <inheritdoc cref="ISetting{T}"/>
	public class Setting<T> : ISetting<T>
	{
		private T _CurrentValue;

		/// <inheritdoc cref="ISetting{T}.Changed"/>
		public event Action<T, T> Changed;

		/// <inheritdoc cref="ISetting{T}.Value"/>
		public T Value
		{
			get => _CurrentValue;
			set
			{
				var currentValue = _CurrentValue;
				if (!value.Equals(currentValue))
				{
					_CurrentValue = value;
					Changed?.Invoke(value, currentValue);
				}
			}
		}

		/// <summary>
		/// Initializes a new <see cref="Setting{T}"/>.
		/// </summary>
		public Setting()
			: this(default(T))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="Setting{T}"/> with a default <see cref="Value"/>.
		/// </summary>
		/// <param name="defaultValue">The default <see cref="Value"/>.</param>
		public Setting(T defaultValue)
		{
			_CurrentValue = defaultValue;
		}
	}
}
