namespace SIS.WebServer.Results
{
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    using System.Text;

    public class UnauthorizedResult : HttpResponse
    {
        private const string DefaultErrorHandling = "<h1>You have no permission to access this functionality.</h1>";

        public UnauthorizedResult(string content) : base(HttpResponseStatusCode.Unauthorized)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
