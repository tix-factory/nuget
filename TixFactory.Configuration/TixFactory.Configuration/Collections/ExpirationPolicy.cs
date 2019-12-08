namespace TixFactory.Collections
{
	/// <summary>
	/// The expiration policy for an <see cref="IExpirableDictionary{TKey,TValue}"/>.
	/// </summary>
	public enum ExpirationPolicy
	{
		/// <summary>
		/// Only renew the expiration of an item when it is written into the <see cref="IExpirableDictionary{TKey,TValue}"/>.
		/// </summary>
		RenewOnWrite = 0,

		/// <summary>
		/// Renew the expiration of an item when it is read from the <see cref="IExpirableDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <remarks>
		/// Will also renew the expiration on writes.
		/// </remarks>
		RenewOnRead = 1,
	}
}
