namespace SIS.HTTP.Sessions
{
    using Contracts;

    using System.Collections.Generic;

    public class HttpSession : IHttpSession
    {
        private readonly IDictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.Id = id;

            this.parameters = new Dictionary<string, object>();
        }

        public string Id { get; }

        public void AddParameter(string name, object parameter)
            => this.parameters[name] = parameter;

        public void ClearParameters()
            => this.parameters.Clear();

        public bool ContainsParameter(string name)
            => this.parameters.ContainsKey(name);

        public object GetParameter(string name)
            => this.ContainsParameter(name) ? this.parameters[name] : null;
    }
}
