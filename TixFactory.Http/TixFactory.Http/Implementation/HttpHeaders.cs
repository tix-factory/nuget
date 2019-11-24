using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace TixFactory.Http
{
	/// <inheritdoc cref="IHttpHeaders"/>
	public abstract class HttpHeaders : IHttpHeaders
	{
		private readonly IDictionary<string, ICollection<string>> _Headers = new Dictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc cref="IHttpHeaders.Keys"/>
		public IReadOnlyList<string> Keys => _Headers.Keys.ToList();

		/// <summary>
		/// Initializes a new blank <see cref="HttpHeaders"/>
		/// </summary>
		public HttpHeaders()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HttpHeaders"/> with values from a <see cref="WebHeaderCollection"/>.
		/// </summary>
		/// <param name="webHeaders">The <see cref="WebHeaderCollection"/></param>
		/// <exception cref="ArgumentNullException">Any argument is null.</exception>
		public HttpHeaders(WebHeaderCollection webHeaders)
		{
			if (webHeaders == null)
			{
				throw new ArgumentNullException(nameof(webHeaders));
			}

			foreach (var headerName in webHeaders.AllKeys)
			{
				var values = webHeaders.GetValues(headerName);
				if (values != null)
				{
					foreach (var value in values)
					{
						Add(headerName, value);
					}
				}
			}
		}

		/// <summary>
		/// Initializes a new <see cref="HttpHeaders"/> with values from a <see cref="System.Net.Http.Headers.HttpHeaders"/> and <see cref="HttpContentHeaders"/>.
		/// </summary>
		/// <param name="httpHeaders">The <see cref="System.Net.Http.Headers.HttpHeaders"/>.</param>
		/// <param name="httpContentHeaders">The <see cref="HttpContentHeaders"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="httpHeaders"/></exception>
		public HttpHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
		{
			if (httpHeaders == null)
			{
				throw new ArgumentNullException(nameof(httpHeaders));
			}

			foreach (var header in httpHeaders)
			{
				foreach(var headerValue in header.Value)
				{
					Add(header.Key, headerValue);
				}
			}

			if(httpContentHeaders != null)
			{
				foreach (var header in httpContentHeaders)
				{
					foreach (var headerValue in header.Value)
					{
						Add(header.Key, headerValue);
					}
				}
			}
		}

		/// <inheritdoc cref="IHttpHeaders.Add"/>
		public void Add(string name, string value)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
			}

			if (_Headers.ContainsKey(name))
			{
				_Headers[name].Add(value);
			}
			else
			{
				_Headers.Add(name, new List<string> { value });
			}
		}

		/// <inheritdoc cref="IHttpHeaders.AddOrUpdate"/>
		public void AddOrUpdate(string name, string value)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
			}

			if (_Headers.ContainsKey(name))
			{
				_Headers[name] = new List<string> { value };
			}
			else
			{
				Add(name, value);
			}
		}

		/// <inheritdoc cref="IHttpHeaders.Get"/>
		public ICollection<string> Get(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
			}

			return _Headers.TryGetValue(name, out var values) ? values : new string[0];
		}

		/// <inheritdoc cref="IHttpHeaders.Remove"/>
		public bool Remove(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
			}

			return _Headers.Remove(name);
		}
	}
}
