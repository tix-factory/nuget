using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TixFactory.ApplicationAuthorization
{
    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> that validates an Api
    /// </summary>
    public class ValidateApiKeyAttribute : ActionFilterAttribute
    {
        private readonly IApiKeyParser _ApiKeyParser;
        private readonly IApplicationAuthorizationsAccessor _ApplicationAuthorizationsAccessor;
        private static readonly Type _AnonymousAttributeType = typeof(AllowAnonymousAttribute);

        /// <summary>
        /// Initializes a new <see cref="ValidateApiKeyAttribute"/>.
        /// </summary>
        /// <param name="apiKeyParser">An <see cref="IApiKeyParser"/>.</param>
        /// <param name="applicationAuthorizationsAccessor">An <see cref="IApplicationAuthorizationsAccessor"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="apiKeyParser"/>
        /// - <paramref name="applicationAuthorizationsAccessor"/>
        /// </exception>
        public ValidateApiKeyAttribute(IApiKeyParser apiKeyParser, IApplicationAuthorizationsAccessor applicationAuthorizationsAccessor)
        {
            _ApiKeyParser = apiKeyParser;
            _ApplicationAuthorizationsAccessor = applicationAuthorizationsAccessor;
        }

        /// <inheritdoc cref="ActionFilterAttribute.OnActionExecutionAsync"/>
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            await OnActionExecutingAsync(context).ConfigureAwait(false);
            if (context.Result == null)
            {
                OnActionExecuted(await next().ConfigureAwait(false));
            }
        }

        private async Task OnActionExecutingAsync(ActionExecutingContext actionContext)
        {
            if (!ShouldValidateApiKey(actionContext))
            {
                // Action does not require authorization.
                return;
            }

            var validated = await TryValidateApiKey(actionContext).ConfigureAwait(false);
            if (!validated)
            {
                actionContext.Result = new UnauthorizedResult();
            }
        }

        private bool ShouldValidateApiKey(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var allowAnonymousAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(
                    attributeType: _AnonymousAttributeType,
                    inherit: true).ToList();

                allowAnonymousAttributes.AddRange(controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(
                    attributeType: _AnonymousAttributeType,
                    inherit: true));

                return !allowAnonymousAttributes.Any();
            }

            return true;
        }

        private async Task<bool> TryValidateApiKey(ActionExecutingContext actionContext)
        {
            if (_ApiKeyParser.TryParseApiKey(actionContext.HttpContext.Request, out var apiKey)
                && actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var authorizedOperationNames = await _ApplicationAuthorizationsAccessor.GetAuthorizedOperationNames(apiKey, actionContext.HttpContext.RequestAborted).ConfigureAwait(false);
                return authorizedOperationNames.Contains(controllerActionDescriptor.ActionName);
            }

            return false;
        }
    }
}
