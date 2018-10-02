namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        private const string DEFAULT_MESSAGE = "The Server has encountered an error.";

        public InternalServerErrorException() : base(DEFAULT_MESSAGE)
        {}

        public InternalServerErrorException(string message) : base(message)
        { }
    }
}
