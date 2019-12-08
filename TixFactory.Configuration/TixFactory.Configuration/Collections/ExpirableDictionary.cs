using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TixFactory.Configuration;

namespace TixFactory.Collections
{
	/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}"/>
	public class ExpirableDictionary<TKey, TValue> : IExpirableDictionary<TKey, TValue>
	{
		private readonly ConcurrentDictionary<TKey, TValue> _Dictionary;
		private readonly ConcurrentDictionary<TKey, DateTime> _KeyExpirations;

		/// <inheritdoc cref="ICollection{TValue}.Count"/>
		public int Count => _Dictionary.Count;

		/// <inheritdoc cref="ICollection{TValue}.IsReadOnly"/>
		public bool IsReadOnly => false;

		/// <inheritdoc cref="IDictionary{TKey,TValue}.Keys"/>
		public ICollection<TKey> Keys => _Dictionary.Keys;

		/// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Keys"/>
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

		/// <inheritdoc cref="IDictionary{TKey,TValue}.Values"/>
		public ICollection<TValue> Values => _Dictionary.Values;

		/// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Values"/>
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

		/// <inheritdoc cref="IDictionary{TKey,TValue}.this[TKey]"/>
		public TValue this[TKey key]
		{
			get => _Dictionary[key];
			set => _Dictionary[key] = value;
		}

		/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}.ValueExpiration"/>
		public ISetting<TimeSpan> ValueExpiration { get; }

		/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}.ExpirationPolicy"/>
		public ISetting<ExpirationPolicy> ExpirationPolicy { get; }

		/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}.VerificationInterval"/>
		public ISetting<TimeSpan> VerificationInterval { get; }

		/// <summary>
		/// Initializes a new <see cref="ExpirableDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <param name="valueExpiration">The <see cref="ValueExpiration"/>.</param>
		/// <param name="expirationPolicy">The <see cref="ExpirationPolicy"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="valueExpiration"/>
		/// - <paramref name="expirationPolicy"/>
		/// </exception>
		public ExpirableDictionary(TimeSpan valueExpiration, ExpirationPolicy expirationPolicy)
			: this(new Setting<TimeSpan>(valueExpiration), new Setting<ExpirationPolicy>(expirationPolicy))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ExpirableDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <param name="valueExpiration">The <see cref="ValueExpiration"/>.</param>
		/// <param name="expirationPolicy">The <see cref="ExpirationPolicy"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="valueExpiration"/>
		/// - <paramref name="expirationPolicy"/>
		/// </exception>
		public ExpirableDictionary(ISetting<TimeSpan> valueExpiration, ISetting<ExpirationPolicy> expirationPolicy)
			: this(new ConcurrentDictionary<TKey, TValue>(), valueExpiration, expirationPolicy)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ExpirableDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <param name="dictionary">The base <see cref="ConcurrentDictionary{TKey,TValue}"/>.</param>
		/// <param name="valueExpiration">The <see cref="ValueExpiration"/>.</param>
		/// <param name="expirationPolicy">The <see cref="ExpirationPolicy"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="dictionary"/>
		/// - <paramref name="valueExpiration"/>
		/// - <paramref name="expirationPolicy"/>
		/// </exception>
		public ExpirableDictionary(ConcurrentDictionary<TKey, TValue> dictionary, ISetting<TimeSpan> valueExpiration, ISetting<ExpirationPolicy> expirationPolicy)
		{
			_Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
			ValueExpiration = valueExpiration ?? throw new ArgumentNullException(nameof(valueExpiration));
			ExpirationPolicy = expirationPolicy ?? throw new ArgumentNullException(nameof(expirationPolicy));

			_KeyExpirations = new ConcurrentDictionary<TKey, DateTime>();
			VerificationInterval = new Setting<TimeSpan>(TimeSpan.FromSeconds(1));

			var verificationTimer = new Timer(
				state => VerifyValues(),
				state: null,
				dueTime: VerificationInterval.Value,
				period: VerificationInterval.Value);

			VerificationInterval.Changed += (newInterval, previousInterval) =>
			{
				verificationTimer.Change(newInterval, newInterval);
			};
		}

		/// <inheritdoc cref="ICollection{TValue}.Add"/>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		/// <inheritdoc cref="ICollection{TValue}.Clear"/>
		public void Clear()
		{
			_Dictionary.Clear();
			_KeyExpirations.Clear();
		}

		/// <inheritdoc cref="ICollection{TValue}.Contains"/>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (TryGetValue(item.Key, isRead: false, value: out var value))
			{
				return item.Value?.Equals(value) == true;
			}

			return false;
		}

		/// <inheritdoc cref="ICollection{TValue}.CopyTo"/>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			var keyValuePairs = _Dictionary.AsEnumerable().ToArray();
			keyValuePairs.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc cref="ICollection{TValue}.Remove"/>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (Contains(item))
			{
				return Remove(item.Key);
			}

			return false;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _Dictionary.GetEnumerator();
		}

		/// <inheritdoc cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc cref="IDictionary{TKey,TValue}.Add(TKey,TValue)"/>
		public void Add(TKey key, TValue value)
		{
			if (_Dictionary.TryAdd(key, value))
			{
				RenewKeyExpiration(key);
			}
			else
			{ 
				throw new ArgumentException($"A value with the key '{key}' already exists in the dictionary.", nameof(key));
			}
		}

		/// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey"/>
		public bool ContainsKey(TKey key)
		{
			return TryGetValue(key, isRead: false, value: out _);
		}

		/// <inheritdoc cref="ICollection{TValue}.Remove"/>
		public bool Remove(TKey key)
		{
			var dictionaryContainedValue = _Dictionary.TryRemove(key, out _);
			var expirationContainedValue = _KeyExpirations.TryRemove(key, out _);
			return expirationContainedValue && dictionaryContainedValue;
		}

		/// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue"/>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return TryGetValue(key, isRead: true, value: out value);
		}

		/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}.VerifyValues"/>
		public void VerifyValues()
		{
			try
			{
				foreach (var entry in _KeyExpirations.ToArray())
				{
					if (entry.Value < DateTime.UtcNow)
					{
						Remove(entry.Key);
					}
				}
			}
			catch (Exception)
			{
				// What could go wrong... :troll:
			}
		}

		/// <inheritdoc cref="IExpirableDictionary{TKey,TValue}.RenewExpiration"/>
		public bool RenewExpiration(TKey key)
		{
			if (_KeyExpirations.ContainsKey(key))
			{
				RenewKeyExpiration(key);
				return true;
			}

			return false;
		}

		private ExpirationPolicy GetExpirationPolicy()
		{
			var expirationPolicy = ExpirationPolicy.Value;
			if (!Enum.IsDefined(typeof(ExpirationPolicy), expirationPolicy))
			{
				return Collections.ExpirationPolicy.RenewOnWrite;
			}

			return expirationPolicy;
		}

		private bool TryGetValue(TKey key, bool isRead, out TValue value)
		{
			if (_Dictionary.TryGetValue(key, out value)
			    && _KeyExpirations.TryGetValue(key, out var expiration))
			{
				if (expiration > DateTime.UtcNow)
				{
					if (isRead)
					{
						switch (GetExpirationPolicy())
						{
							case Collections.ExpirationPolicy.RenewOnRead:
								RenewKeyExpiration(key);
								break;
						}
					}

					return true;
				}

				Remove(key);
			}

			value = default(TValue);
			return false;
		}

		private void RenewKeyExpiration(TKey key)
		{
			_KeyExpirations[key] = DateTime.UtcNow + ValueExpiration.Value;
		}
	}
}
