namespace SIS.HTTP.Responses
{
    using System.Text;

    using Enums;
    using Common;
    using Headers;
    using Cookies;
    using Contracts;
    using Extensions;
    using Headers.Contracts;
    using Cookies.Contracts;

    using System.Linq;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.Content = new byte[0];
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; private set; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
            => this.Headers.Add(header);

        public void AddCookie(HttpCookie cookie)
            => this.Cookies.Add(cookie);

        public byte[] GetBytes()
        {
            var byteResponse = Encoding.UTF8.GetBytes(this.ToString()).ToList();

            byteResponse.AddRange(this.Content);

            this.Content = byteResponse.ToArray();

            return this.Content;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}")
                .AppendLine(this.Headers.ToString());

            if (this.Cookies.HasCookies())
                result.AppendLine($"Set-Cookie: {this.Cookies}");

            result.AppendLine();

            return result.ToString();
        }
    }
}
