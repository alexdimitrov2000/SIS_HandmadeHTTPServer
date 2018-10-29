namespace SIS.Framework.Routers
{
    using HTTP.Enums;
    using HTTP.Common;
    using Controllers;
    using WebServer.Api;
    using HTTP.Extensions;
    using WebServer.Results;
    using Attributes.Methods;
    using Services.Contracts;
    using HTTP.Requests.Contracts;
    using ActionResults.Contracts;
    using HTTP.Responses.Contracts;

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using SIS.Framework.Attributes.Action;

    public class ControllerRouter : IHttpHandler
    {
        private readonly IDependencyContainer dependencyContainer;

        public ControllerRouter(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

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
                var controller = (Controller)this.dependencyContainer.CreateInstance(controllerType);

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

            var suitableMethods = GetSuitableMethods(controller, actionName);

            foreach (var methodInfo in suitableMethods)
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

            if (actionResult is IError)
                return new BadRequestResult(invocationResult);

            throw new InvalidOperationException("The view result is not supported.");
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
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

            object[] actionParameters = this.MapActionParameters(action, controller, request);

            return this.Authorize(controller, action) ?? 
                   this.PrepareResponse(this.InvokeAction(controller, action, actionParameters));
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
            => (IActionResult)action.Invoke(controller, actionParameters);

        private object[] MapActionParameters(MethodInfo action, Controller controller, IHttpRequest request)
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
                {
                    object bindingModel = ProcessBindingModelParameters(currentActionParameterInfo, request);
                    controller.ModelState.IsValid = this.IsValid(bindingModel, currentActionParameterInfo.ParameterType);
                    mappedActionParameters[i] = bindingModel;
                }
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
                    object value = this.GetParameterFromRequestData(request, prop.Name.LowerizeFirstLetter());
                    prop.SetValue(bindingModelInstance, Convert.ChangeType(value, prop.PropertyType));
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

        private IHttpResponse Authorize(Controller controller, MethodInfo action)
        {
            if (action
                .GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .Any(a => !a.IsAuthorized(controller.Identity)))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.AuthorizationErrorViewPath);
                errorViewContent = errorViewContent
                    .Replace(GlobalConstants.ErrorModel, 
                                        "You have no permission to access this functionality. Please log in first and try again.");
                return new UnauthorizedResult(errorViewContent);
            }

            return null;
        }

        private bool? IsValid(object bindingModel, Type bindingModelType)
        {
            PropertyInfo[] properties = bindingModelType.GetProperties();

            foreach (var property in properties)
            {
                var validationAttributes = property
                    .GetCustomAttributes()
                    .Where(a => a is ValidationAttribute)
                    .Cast<ValidationAttribute>()
                    .ToList();

                foreach (var attribute in validationAttributes)
                {
                    var propertyValue = property.GetValue(bindingModel);

                    if (!attribute.IsValid(propertyValue))
                        return false;
                }
            }

            return true;
        }
    }
}
