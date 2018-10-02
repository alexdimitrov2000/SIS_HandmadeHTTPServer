namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        private const string DEFAULT_MESSAGE = "The Request was malformed or contains unsupported elements.";

        public BadRequestException() : base(DEFAULT_MESSAGE)
        {}

        public BadRequestException(string message) : base(message)
        { }
    }
}
