namespace SIS.HTTP.Extensions
{
    using Enums;
    using System.Collections.Generic;

    public static class HttpResponseStatusExtensions
    {
        private static Dictionary<int, string> status = new Dictionary<int, string>()
        {
            { 200, "200 OK" },
            { 201, "201 Created" },
            { 302, "302 Found" },
            { 303, "303 See Other" },
            { 400, "400 Bad Request" },
            { 401, "401 Unauthorized" },
            { 403, "403 Forbidden" },
            { 404, "404 Not Found" },
            { 500, "500 Internal Server Error" },
        };

        public static string GetResponseLine(this HttpResponseStatusCode responseStatusCode)
            => status[(int)responseStatusCode];
    }
}
