namespace SIS.HTTP.Requests
{
    using Common;
    using Contracts;
    using Cookies;
    using Cookies.Contracts;
    using Enums;
    using Exceptions;
    using Extensions;
    using Headers;
    using Headers.Contracts;
    using Sessions.Contracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpSession Session { get; set; }

        private void ParseCookies()
        {
            if (!this.Headers.ContainsHeader("Cookie"))
                return;

            HttpHeader cookieHeader = this.Headers.GetHeader("Cookie");
            
            string cookieHeaderValue = cookieHeader.Value;

            if (string.IsNullOrEmpty(cookieHeaderValue))
                return;

            string[] stringCookies = cookieHeaderValue.Split("; ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var cookieString in stringCookies)
            {
                string[] cookieTokens = cookieString.Split('=');

                if (cookieTokens.Length != 2)
                    continue;

                string cookieKey = cookieTokens[0];
                string cookieValue = cookieTokens[1];

                this.Cookies.Add(new HttpCookie(cookieKey, cookieValue, false));
            }
        }

        private bool IsValidRequestLine(string[] requestLine)
        {
            return (requestLine.Length == 3 && requestLine[2].ToUpper() == "HTTP/1.1");
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            return string.IsNullOrEmpty(queryString) && queryParameters.Length > 0;
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            string requestStringMethod = requestLine[0];

            HttpRequestMethod requestMethod;

            if (!Enum.TryParse(requestStringMethod.Capitalize(), out requestMethod))
                throw new BadRequestException();

            this.RequestMethod = requestMethod;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            string[] urlParams = this.Url.Split('?', '#', StringSplitOptions.RemoveEmptyEntries);

            this.Path = urlParams[0];
        }

        private void ParseHeaders(string[] requestLines)
        {
            int indexOfEmptyLine = Array.IndexOf(requestLines, string.Empty);

            for (int i = 0; i < indexOfEmptyLine; i++)
            {
                string headerString = requestLines[i];

                if (!headerString.Contains(": "))
                    continue;

                string[] headerTokens = headerString.Split(": ", StringSplitOptions.RemoveEmptyEntries);
                string headerKey = headerTokens[0];
                string headerValue = headerTokens[1];

                this.Headers.Add(new HttpHeader(headerKey, headerValue));
            }

            if (!this.Headers.ContainsHeader(GlobalConstants.HostHeaderKey))
                throw new BadRequestException();
        }

        private void ParseQueryParameters()
        {
            if (!this.Url.Contains('?'))
                return;

            string queryString = this.Url.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries)[1];

            if (!queryString.Contains('='))
                return;

            var queryPairs = queryString.Split('&');

            if (!IsValidRequestQueryString(queryString, queryPairs))
                throw new BadRequestException();

            foreach (var pair in queryPairs)
            {
                string[] kvp = pair.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (kvp.Length != 2)
                    return;

                string key = WebUtility.UrlDecode(kvp[0]);
                string value = WebUtility.UrlDecode(kvp[1]);

                this.QueryData.Add(key, value);
            }
        }

        private void ParseFormDataParameters(string formDataString)
        {
            string[] formDataPairs = formDataString.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in formDataPairs)
            {
                string[] pairTokens = pair.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (pairTokens.Length != 2)
                    continue;

                string key = WebUtility.UrlDecode(pairTokens[0]);
                string value = WebUtility.UrlDecode(pairTokens[1]);

                this.FormData.Add(key, value);
            }
        }

        private void ParseRequestParameters(string requestLine)
        {
            if (this.RequestMethod == HttpRequestMethod.Post)
            {
                this.ParseFormDataParameters(requestLine);
            }

            this.ParseQueryParameters();
        }

        private void ParseRequest(string requestString)
        {
            string[] requestLines = requestString.Split(Environment.NewLine);

            string[] requestLineTokens = requestLines[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLineTokens))
                throw new BadRequestException();

            this.ParseRequestMethod(requestLineTokens);
            this.ParseRequestUrl(requestLineTokens);
            this.ParseRequestPath();

            this.ParseHeaders(requestLines.Skip(1).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(requestLines.Last());
        }
    }
}
