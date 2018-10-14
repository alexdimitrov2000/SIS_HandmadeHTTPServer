namespace SIS.Framework.Views
{
    using SIS.Framework.ActionResults.Contracts;
    using System.IO;

    public class View : IRenderable
    {
        private readonly string fullyQualifiedTemplateName;

        public View(string fullyQualifiedTemplateName)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
        }

        private string ReadFile(string fullyQualifiedTemplateName)
        {
            if (!File.Exists(fullyQualifiedTemplateName))
                throw new FileNotFoundException();

            return File.ReadAllText(fullyQualifiedTemplateName);
        }

        public string Render()
            =>  this.ReadFile(this.fullyQualifiedTemplateName);
    }
}
