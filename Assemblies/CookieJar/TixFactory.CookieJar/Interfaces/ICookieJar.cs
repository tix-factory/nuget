namespace TixFactory.CookieJar
{
    /// <summary>
    /// A wrapper for <see cref="CookieContainer"/> that allows saving the cookies inside.
    /// </summary>
    public interface ICookieJar
    {
        /// <summary>
        /// The <see cref="CookieContainer"/>.
        /// </summary>
        System.Net.CookieContainer CookieContainer { get; }

        /// <summary>
        /// Saves the cookies to disk with File Name used to construct the <see cref="ICookieJar"/>.
        /// </summary>
        void Save();
    }
}
