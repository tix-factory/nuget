using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging;
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
        /// <exception cref="ArgumentException">
        /// - <paramref name="fileName"/> is <c>null</c> or white space.
        /// </exception>
        public FileCookieJar(string fileName, CookieContainer cookieContainer = null)
            : this(fileName, null, cookieContainer)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="FileCookieJar"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="cookieContainer"/> is set, any cookies in the file are ignored, and replaced.
        /// </remarks>
        /// <param name="fileName">The file name to save/load the cookies to/from.</param>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}"/>.</param>
        /// <param name="cookieContainer">An initial <see cref="CookieContainer"/>.</param>
        /// <exception cref="ArgumentException">
        /// - <paramref name="fileName"/> is <c>null</c> or white space.
        /// </exception>
        public FileCookieJar(string fileName, ILogger<FileCookieJar> logger, CookieContainer cookieContainer = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            }

            _FileName = fileName;
            CookieContainer = cookieContainer ?? CreateCookieContainer(fileName, logger);
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

        private static CookieContainer CreateCookieContainer(string fileName, ILogger<FileCookieJar> logger)
        {
            var cookieContainer = new CookieContainer();

            try
            {
                var cookies = File.ReadAllText(fileName);
                var cookieCollection = JsonConvert.DeserializeObject<CookieCollection>(cookies);
                cookieContainer.Add(cookieCollection);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to read cookies from file.");
            }

            return cookieContainer;
        }
    }
}
