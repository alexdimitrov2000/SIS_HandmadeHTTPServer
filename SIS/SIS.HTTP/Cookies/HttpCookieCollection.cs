namespace SIS.HTTP.Cookies
{
    using Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private readonly IDictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
            => this.cookies.Add(cookie.Key, cookie);

        public bool ContainsCookie(string key)
            => this.cookies.ContainsKey(key);

        public HttpCookie GetCookie(string key)
            => this.cookies.ContainsKey(key) ? this.cookies[key] : null;

        public bool HasCookies()
            => this.cookies.Any();

        public override string ToString()
        {
            return string.Join("; ", this.cookies.Values);
        }
    }
}
