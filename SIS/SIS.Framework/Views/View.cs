namespace SIS.Framework.Views
{
    using SIS.Framework.ActionResults.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class View : IRenderable
    {
        private readonly string fullyQualifiedTemplateName;

        private readonly IDictionary<string, object> viewData;

        public View(string fullyQualifiedTemplateName, IDictionary<string, object> viewData)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
            this.viewData = viewData;
        }

        public string Render()
        {
            string fullHtml = this.ReadFile();
            string renderedHtml = this.RenderHtml(fullHtml);

            string viewContent = File.ReadAllText($"Views/_Layout.html").Replace("@RenderBody", renderedHtml);

            return viewContent;
        }

        private string ReadFile()
        {
            if (!File.Exists(this.fullyQualifiedTemplateName))
                throw new FileNotFoundException();

            return File.ReadAllText(this.fullyQualifiedTemplateName);
        }

        private string RenderHtml(string fullHtml)
        {
            string renderedHtml = fullHtml;

            if (this.viewData.Any())
            {
                foreach (var parameter in this.viewData)
                {
                    renderedHtml = renderedHtml
                        .Replace(GlobalConstants.ModelParam + parameter.Key.Capitalize(), parameter.Value.ToString());
                }
            }

            return renderedHtml;
        }
    }
}
