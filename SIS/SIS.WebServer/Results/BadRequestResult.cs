namespace SIS.WebServer.Results
{
    using HTTP.Headers;
    using HTTP.Responses;
    using System;
    using System.Text;

    public class BadRequestResult : HttpResponse
    {
        public BadRequestResult(string content) : base(HTTP.Enums.HttpResponseStatusCode.BadRequest)
        {
            this.Headers.Add(new HttpHeader("Content-Type", "text/html"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
