using System;

namespace TixFactory.Configuration
{
	/// <inheritdoc cref="ILazyWithRetry{T}"/>
	public class LazyWithRetry<T> : ILazyWithRetry<T>
	{
		private readonly Func<T> _ValueFactory;
		private readonly ISetting<TimeSpan> _RetryTimeout;
		private Lazy<T> _Lazy;
		private DateTime? _ValueRetryExpiration;

		/// <inheritdoc cref="ILazyWithRetry{T}.Value"/>
		public T Value => LoadValue();

		/// <inheritdoc cref="ILazyWithRetry{T}.IsValueCreated"/>
		public bool IsValueCreated => _Lazy.IsValueCreated;

		/// <summary>
		/// Initializes a new <see cref="LazyWithRetry{T}"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="Value"/> retry timeout will be hard-coded to one second.
		/// </remarks>
		/// <param name="valueFactory">The <see cref="Func{TResult}"/> to load the <see cref="Value"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="valueFactory"/>
		/// </exception>
		public LazyWithRetry(Func<T> valueFactory)
			: this(valueFactory, TimeSpan.FromSeconds(1))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="LazyWithRetry{T}"/>.
		/// </summary>
		/// <param name="valueFactory">The <see cref="Func{TResult}"/> to load the <see cref="Value"/>.</param>
		/// <param name="retryTimeout">A <see cref="TimeSpan"/> for how long to wait before retrying the setting fetch.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="valueFactory"/>
		/// </exception>
		public LazyWithRetry(Func<T> valueFactory, TimeSpan retryTimeout)
			: this(valueFactory, new Setting<TimeSpan>(retryTimeout))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="LazyWithRetry{T}"/>.
		/// </summary>
		/// <param name="valueFactory">The <see cref="Func{TResult}"/> to load the <see cref="Value"/>.</param>
		/// <param name="retryTimeout">A <see cref="TimeSpan"/> <see cref="ISetting{T}"/> for how long to wait before retrying the setting fetch.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="valueFactory"/>
		/// - <paramref name="retryTimeout"/>
		/// </exception>
		public LazyWithRetry(Func<T> valueFactory, ISetting<TimeSpan> retryTimeout)
		{
			_ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
			_RetryTimeout = retryTimeout ?? throw new ArgumentNullException(nameof(retryTimeout));

			Refresh();
		}

		/// <inheritdoc cref="ILazyWithRetry{T}.Refresh"/>
		public void Refresh()
		{
			_Lazy = new Lazy<T>(_ValueFactory);
			_ValueRetryExpiration = null;
		}

		private T LoadValue()
		{
			if (_ValueRetryExpiration < DateTime.UtcNow)
			{
				Refresh();
			}

			try
			{
				return _Lazy.Value;
			}
			catch (Exception)
			{
				if (!_ValueRetryExpiration.HasValue)
				{
					_ValueRetryExpiration = DateTime.UtcNow + _RetryTimeout.Value;
				}

				throw;
			}
		}
	}
}
