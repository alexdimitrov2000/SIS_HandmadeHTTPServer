namespace SIS.WebServer.Results
{
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    using System.Text;

    public class TextResult : HttpResponse
    {
        public TextResult(string content, HttpResponseStatusCode statusCode) : base(statusCode)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/plain"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
