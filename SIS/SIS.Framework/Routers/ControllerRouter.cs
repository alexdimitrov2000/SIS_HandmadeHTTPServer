namespace SIS.Framework.Routers
{
    using Controllers;
    using WebServer.Api;
    using Attributes.Methods;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.WebServer.Results;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Common;
    using System.IO;

    public class ControllerRouter : IHttpHandler
    {
        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (!string.IsNullOrEmpty(controllerName))
            {
                string controllerTypeName = string.Format(
                    "{0}.{1}.{2}, {0}",
                    MvcContext.Get.AssemblyName,
                    MvcContext.Get.ControllersFolder,
                    controllerName);

                var controllerType = Type.GetType(controllerTypeName);
                var controller = (Controller)Activator.CreateInstance(controllerType);

                if (controller != null)
                {
                    controller.Request = request;
                }

                return controller;
            }

            return null;
        } 

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            MethodInfo method = null;

            foreach (var methodInfo in GetSuitableMethods(controller, actionName))
            {
                var attributes = methodInfo.GetCustomAttributes()
                                           .Where(a => a is HttpMethodAttribute)
                                           .Cast<HttpMethodAttribute>();

                if (!attributes.Any() && requestMethod.ToUpper() == "GET")
                    return methodInfo;

                foreach (var attribute in attributes)
                {
                    if (attribute.IsValid(requestMethod))
                    {
                        return methodInfo;
                    }
                }
            }

            return method;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
                return new MethodInfo[0];

            return controller
                .GetType()
                .GetMethods()
                .Where(methodInfo => methodInfo.Name.ToLower() == actionName.ToLower());
        }

        private IHttpResponse PrepareResponse(Controller controller, MethodInfo action)
        {
            IActionResult actionResult = (IActionResult)action.Invoke(controller, null);
            string invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
                return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);
            else if (actionResult is IRedirectable)
                return new RedirectResult(invocationResult);
            else
                throw new InvalidOperationException("The view result is not supported.");
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            if (IsResourceRequest(request.Path))
                return ResourceFile(request.Path);

            var requestMethod = request.RequestMethod;

            string requestPath = request.Path;

            string controllerName = string.Empty;
            string actionName = string.Empty;

            if (requestPath == "/")
            {
                controllerName = "Home" + MvcContext.Get.ControllersSuffix;
                actionName = "Index";
            }
            else
            {
                string[] pathTokens = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                controllerName = pathTokens[0] + MvcContext.Get.ControllersSuffix;
                actionName = pathTokens[1];
            }

            Controller controller = this.GetController(controllerName, request);

            MethodInfo action = this.GetMethod(requestMethod.ToString(), controller, actionName);

            return this.PrepareResponse(controller, action);
        }

        private IHttpResponse ResourceFile(string requestPath)
        {
            string resourcePath = requestPath.Substring(1);

            if (!File.Exists(resourcePath))
            {
                string notFoundContent = File.ReadAllText(GlobalConstants.NotFoundFilePath);
                return new BadRequestResult(notFoundContent);
            }

            var resourceContent = File.ReadAllBytes(resourcePath);

            return new InlineResourceResult(resourceContent, HttpResponseStatusCode.Ok);
        }

        private bool IsResourceRequest(string path)
        {
            if (path.Contains(GlobalConstants.Dot))
            {
                var lastIndexOfDot = path.LastIndexOf(GlobalConstants.Dot);

                var resourceExtension = path.Substring(lastIndexOfDot);

                return GlobalConstants.ResourceExtensions.Contains(resourceExtension);
            }

            return false;
        }
    }
}
