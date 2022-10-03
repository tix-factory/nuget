using System;
using System.IO;
using System.Net;
using System.Text;

namespace TixFactory.Http
{
    /// <inheritdoc cref="IHttpResponse"/>
    public class HttpResponse : IHttpResponse
    {
        private const int _MinimumSuccessfulStatusCode = 200;
        private const int _MaximumSuccessfulStatusCode = 299;

        private bool _StringBodySet = false;
        private string _StringBody;

        /// <inheritdoc cref="IHttpResponse.StatusCode"/>
        public HttpStatusCode StatusCode { get; set; }

        /// <inheritdoc cref="IHttpResponse.StatusText"/>
        public string StatusText { get; set; }

        /// <inheritdoc cref="IHttpResponse.IsSuccessful"/>
        public bool IsSuccessful
        {
            get
            {
                var statusCode = (int)StatusCode;
                return statusCode >= _MinimumSuccessfulStatusCode && statusCode <= _MaximumSuccessfulStatusCode;
            }
        }

        /// <inheritdoc cref="IHttpResponse.Url"/>
        public Uri Url { get; set; }

        /// <inheritdoc cref="IHttpResponse.Headers"/>
        public IHttpResponseHeaders Headers { get; set; }

        /// <inheritdoc cref="IHttpResponse.Body"/>
        public Stream Body { get; set; }

        /// <inheritdoc cref="IHttpResponse.GetStringBody"/>
        public string GetStringBody(Encoding encoding = null)
        {
            if (_StringBodySet)
            {
                return _StringBody;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string stringBody;

            Body.Seek(0, SeekOrigin.Begin);
            using (var streamReader = new StreamReader(Body, encoding))
            {
                stringBody = streamReader.ReadToEnd();
            }

            _StringBody = stringBody;
            _StringBodySet = true;

            return stringBody;
        }
    }
}
