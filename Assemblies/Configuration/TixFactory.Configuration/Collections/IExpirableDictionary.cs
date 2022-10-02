using System;
using System.Collections.Generic;
using TixFactory.Configuration;

namespace TixFactory.Collections
{
	/// <summary>
	/// An <see cref="IDictionary{TKey,TValue}"/> where the values can expire.
	/// </summary>
	/// <typeparam name="TKey">The dictionary key.</typeparam>
	/// <typeparam name="TValue">The dictionary value.</typeparam>
	public interface IExpirableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
	{
		/// <summary>
		/// The expiration for values going into the dictionary.
		/// </summary>
		/// <remarks>
		/// Items are removed from the dictionary after this amount of time depending on the <see cref="ExpirationPolicy"/>.
		/// </remarks>
		ISetting<TimeSpan> ValueExpiration { get; }

		/// <summary>
		/// The <see cref="ExpirationPolicy"/> setting.
		/// </summary>
		ISetting<ExpirationPolicy> ExpirationPolicy { get; }

		/// <summary>
		/// How often to verify the entries in the dictionary.
		/// </summary>
		ISetting<TimeSpan> VerificationInterval { get; }

		/// <summary>
		/// Goes over all keys in the dictionary and removes expired entries.
		/// </summary>
		/// <remarks>
		/// Executes on an interval of <see cref="VerificationInterval"/>.
		/// </remarks>
		void VerifyValues();

		/// <summary>
		/// Renews the expiration for a <see cref="KeyValuePair{TKey,TValue}"/> in the dictionary.
		/// </summary>
		/// <returns><c>true</c> if the key exists in the dictionary and had its expiration extended.</returns>
		bool RenewExpiration(TKey key);
	}
}
