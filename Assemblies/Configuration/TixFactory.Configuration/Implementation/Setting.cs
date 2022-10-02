using System;
using System.Threading;

namespace TixFactory.Configuration
{
	/// <inheritdoc cref="ISetting{T}"/>
	public class Setting<T> : ISetting<T>
	{
		private T _CurrentValue;

		/// <inheritdoc cref="IReadOnlySetting{T}.Changed"/>
		public event Action<T, T> Changed;

		/// <inheritdoc cref="ISetting{T}.Value"/>
		public T Value
		{
			get => _CurrentValue;
			set
			{
				var newValue = value;
				var originalValue = _CurrentValue;
				if (!newValue.Equals(originalValue))
				{
					_CurrentValue = newValue;

					var changedEventListener = Changed;
					if (changedEventListener != null)
					{
						ThreadPool.QueueUserWorkItem(state => changedEventListener.Invoke(newValue, originalValue));
					}
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
