using System.Net;

namespace TixFactory.CookieJar
{
	/// <summary>
	/// An <see cref="ICookieJar"/> with no backing.
	/// </summary>
	/// <inheritdoc cref="ICookieJar"/>
	public class BaselessCookieJar : ICookieJar
	{
		/// <inheritdoc cref="ICookieJar.CookieContainer"/>
		public CookieContainer CookieContainer { get; }

		/// <summary>
		/// Initializes a new <see cref="BaselessCookieJar"/>.
		/// </summary>
		/// <param name="cookieContainer">The <see cref="CookieContainer"/> (if <c>null</c> initializes a new one.)</param>
		public BaselessCookieJar(CookieContainer cookieContainer = null)
		{
			CookieContainer = cookieContainer ?? new CookieContainer();
		}

		/// <inheritdoc cref="ICookieJar.Save"/>
		public void Save()
		{
		}
	}
}
