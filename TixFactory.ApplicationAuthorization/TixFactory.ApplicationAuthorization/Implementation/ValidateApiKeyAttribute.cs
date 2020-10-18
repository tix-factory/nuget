using System;
using System.Linq;
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

		/// <inheritdoc cref="ActionFilterAttribute.OnActionExecuting"/>
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			if (!ShouldValidateApiKey(actionContext))
			{
				// Action does not require authorization.
				return;
			}

			if (!TryValidateApiKey(actionContext))
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

		private bool TryValidateApiKey(ActionExecutingContext actionContext)
		{
			if (_ApiKeyParser.TryParseApiKey(actionContext.HttpContext.Request, out var apiKey)
				&& actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				var authorizedOperationNames = _ApplicationAuthorizationsAccessor.GetAuthorizedOperationNames(apiKey);
				return authorizedOperationNames.Contains(controllerActionDescriptor.ActionName);
			}

			return false;
		}
	}
}
