namespace SIS.WebServer.Results
{
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Headers;

    public class InlineResourceResult : HttpResponse
    {
        public InlineResourceResult(byte[] content, HttpResponseStatusCode statusCode)
            :base(statusCode)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Content = content;
        }
    }
}
