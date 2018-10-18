namespace SIS.Framework.Routers
{
    using Attributes.Methods;
    using Controllers;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using WebServer.Api;

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

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            string invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
                return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);

            if (actionResult is IRedirectable)
                return new RedirectResult(invocationResult);

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

            object[] actionParameters = this.MapActionParameters(action, request);

            IActionResult actionResult = this.InvokeAction(controller, action, actionParameters);

            return this.PrepareResponse(actionResult);
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
            => (IActionResult)action.Invoke(controller, actionParameters);

        private object[] MapActionParameters(MethodInfo action, IHttpRequest request)
        {
            ParameterInfo[] actionParametersInfo = action.GetParameters();
            object[] mappedActionParameters = new object[actionParametersInfo.Length];

            for (int i = 0; i < actionParametersInfo.Length; i++)
            {
                ParameterInfo currentActionParameterInfo = actionParametersInfo[i];

                if (currentActionParameterInfo.ParameterType.IsPrimitive ||
                        currentActionParameterInfo.ParameterType == typeof(string))
                    mappedActionParameters[i] = ProcessPrimitiveParameter(currentActionParameterInfo, request);
                else
                    mappedActionParameters[i] = ProcessBindingModelParameters(currentActionParameterInfo, request);
            }

            return mappedActionParameters;
        }

        private object ProcessBindingModelParameters(ParameterInfo param, IHttpRequest request)
        {
            Type bindingModelType = param.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var prop in bindingModelProperties)
            {
                try
                {
                    object value = this.GetParameterFromRequestData(request, prop.Name);
                    prop.SetValue(bindingModelInstance, Convert.ChangeType(value, prop.PropertyType
                        ));
                }
                catch
                {
                    Console.WriteLine($"The {prop.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(ParameterInfo param, IHttpRequest request)
        {
            object value = this.GetParameterFromRequestData(request, param.Name);

            return Convert.ChangeType(value, param.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string paramName)
        {
            if (request.FormData.ContainsKey(paramName))
                return request.FormData[paramName];
            if (request.QueryData.ContainsKey(paramName))
                return request.QueryData[paramName];

            return null;
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
