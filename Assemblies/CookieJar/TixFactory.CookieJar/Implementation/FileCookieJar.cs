using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace TixFactory.CookieJar
{
    /// <remarks>
    /// A filed based <see cref="ICookieJar"/>.
    /// </remarks>
    /// <inheritdoc cref="ICookieJar"/>
    public class FileCookieJar : ICookieJar
    {
        private readonly string _FileName;
        private readonly SemaphoreSlim _SaveLock = new(1, 1);

        /// <inheritdoc cref="ICookieJar.CookieContainer"/>
        public CookieContainer CookieContainer { get; }

        /// <summary>
        /// Initializes a new <see cref="FileCookieJar"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="cookieContainer"/> is set, any cookies in the file are ignored, and replaced.
        /// </remarks>
        /// <param name="fileName">The file name to save/load the cookies to/from.</param>
        /// <param name="cookieContainer">An initial <see cref="CookieContainer"/>.</param>
        public FileCookieJar(string fileName, CookieContainer cookieContainer = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            }

            _FileName = fileName;
            CookieContainer = cookieContainer ?? CreateCookieContainer(fileName);
        }

        /// <inheritdoc cref="ICookieJar.Save"/>
        public void Save()
        {
            if (_SaveLock.CurrentCount <= 0)
            {
                return;
            }

            _SaveLock.Wait();

            try
            {
                var cookies = JsonConvert.SerializeObject(CookieContainer.GetAllCookies());
                File.WriteAllText(_FileName, cookies);
            }
            finally
            {
                _SaveLock.Release();
            }
        }

        private static CookieContainer CreateCookieContainer(string fileName)
        {
            var cookieContainer = new CookieContainer();

            try
            {
                var cookies = File.ReadAllText(fileName);
                var cookieCollection = JsonConvert.DeserializeObject<CookieCollection>(cookies);
                cookieContainer.Add(cookieCollection);
            }
            catch
            {
                // Probably shouldn't swallow this. ¯\_(ツ)_/¯
            }

            return cookieContainer;
        }
    }
}
