namespace SIS.HTTP.Headers
{
    using Contracts;
    using System;
    using System.Collections.Generic;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
            => this.headers.Add(header.Key, header);

        public bool ContainsHeader(string key)
            => this.headers.ContainsKey(key);

        public HttpHeader GetHeader(string key)
            => this.headers.ContainsKey(key) ? this.headers[key] : null;

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.headers.Values);
        }
    }
}
