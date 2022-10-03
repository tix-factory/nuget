using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace TixFactory.CookieJar
{
    /// <remarks>
    /// A filed based <see cref="ICookieJar"/>.
    /// </remarks>
    /// <inheritdoc cref="ICookieJar"/>
    public class FileCookieJar : ICookieJar
    {
        private readonly string _FileName;
        private readonly BinaryFormatter _BinaryFormatter;
        private readonly SemaphoreSlim _SaveLock = new SemaphoreSlim(1, 1);

        /// <inheritdoc cref="ICookieJar.CookieContainer"/>
        public CookieContainer CookieContainer { get; }

        /// <summary>
        /// Initializes a new <see cref="FileCookieJar"/>.
        /// </summary>
        /// <param name="fileName">The file name to save/load the cookies to/from.</param>
        /// <param name="cookieContainer"></param>
        public FileCookieJar(string fileName, CookieContainer cookieContainer = null)
            : this(new BinaryFormatter(), fileName, cookieContainer)
        {
        }

        private FileCookieJar(BinaryFormatter binaryFormatter, string fileName, CookieContainer cookieContainer = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            }

            _FileName = fileName;
            _BinaryFormatter = binaryFormatter ?? throw new ArgumentNullException(nameof(binaryFormatter));
            CookieContainer = cookieContainer ?? CreateCookieContainer(fileName, binaryFormatter);
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
                using (var memoryStream = new MemoryStream())
                {
                    _BinaryFormatter.Serialize(memoryStream, CookieContainer);
                    File.WriteAllBytes(_FileName, memoryStream.ToArray());
                }
            }
            finally
            {
                _SaveLock.Release();
            }
        }

        private static CookieContainer CreateCookieContainer(string fileName, BinaryFormatter binaryFormatter)
        {
            if (File.Exists(fileName))
            {
                var cookieBytes = File.ReadAllBytes(fileName);
                using (var memoryStream = new MemoryStream(cookieBytes))
                {
                    return (CookieContainer)binaryFormatter.Deserialize(memoryStream);
                }
            }

            return new CookieContainer();
        }
    }
}
