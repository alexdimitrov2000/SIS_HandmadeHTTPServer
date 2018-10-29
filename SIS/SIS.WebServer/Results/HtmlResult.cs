namespace SIS.WebServer.Results
{
    using HTTP.Enums;
    using HTTP.Responses;
    using HTTP.Headers;

    using System.Text;

    public class HtmlResult : HttpResponse
    {
        public HtmlResult(string content, HttpResponseStatusCode statusCode) 
            : base(statusCode)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
