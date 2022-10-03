using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Operations;

namespace TixFactory.Http.Server
{
	/// <inheritdoc cref="IOperationExecuter"/>
	public class OperationExecuter : IOperationExecuter
	{
		/// <inheritdoc cref="IOperationExecuter.Execute{TInput}(IAction{TInput}, TInput)"/>
		public IActionResult Execute<TInput>(IAction<TInput> action, TInput input)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			var error = action.Execute(input);
			if (error != null)
			{
				return BuildErrorResult(error);
			}

			return new NoContentResult();
		}

		/// <inheritdoc cref="IOperationExecuter.Execute{TOutput}(IOperation{TOutput})"/>
		public IActionResult Execute<TOutput>(IOperation<TOutput> operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
			}

			var (data, operationError) = operation.Execute();
			return BuildPayloadResult(data, operationError);
		}

		/// <inheritdoc cref="IOperationExecuter.Execute{TInput,TOutput}(IOperation{TInput,TOutput}, TInput)"/>
		public IActionResult Execute<TInput, TOutput>(IOperation<TInput, TOutput> operation, TInput input)
		{
			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
			}

			var (data, operationError) = operation.Execute(input);
			return BuildPayloadResult(data, operationError);
		}

		/// <inheritdoc cref="IOperationExecuter.ExecuteAsync{TInput}(IAsyncAction{TInput}, TInput, CancellationToken)"/>
		public async Task<IActionResult> ExecuteAsync<TInput>(IAsyncAction<TInput> action, TInput input, CancellationToken cancellationToken)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			var error = await action.Execute(input, cancellationToken).ConfigureAwait(false);
			if (error != null)
			{
				return BuildErrorResult(error);
			}

			return new NoContentResult();
		}

		/// <inheritdoc cref="IOperationExecuter.ExecuteAsync{TOutput}(IAsyncOperation{TOutput}, CancellationToken)"/>
		public async Task<IActionResult> ExecuteAsync<TOutput>(IAsyncOperation<TOutput> operation, CancellationToken cancellationToken)
		{
			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
			}

			var (data, operationError) = await operation.Execute(cancellationToken).ConfigureAwait(false);
			return BuildPayloadResult(data, operationError);
		}

		/// <inheritdoc cref="IOperationExecuter.ExecuteAsync{TInput,TOutput}(IAsyncOperation{TInput,TOutput}, TInput, CancellationToken)"/>
		public async Task<IActionResult> ExecuteAsync<TInput, TOutput>(IAsyncOperation<TInput, TOutput> operation, TInput input, CancellationToken cancellationToken)
		{
			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
			}

			var (data, operationError) = await operation.Execute(input, cancellationToken).ConfigureAwait(false);
			return BuildPayloadResult(data, operationError);
		}

		private IActionResult BuildPayloadResult<TData>(TData data, OperationError operationError)
		{
			if (operationError != null)
			{
				return BuildErrorResult(operationError);
			}

			var payload = new Payload<TData>(data, null);
			return new JsonResult(payload);
		}

		private IActionResult BuildErrorResult(OperationError operationError)
		{
			var payload = new Payload<object>(null, operationError);

			return new JsonResult(payload)
			{
				StatusCode = (int)HttpStatusCode.BadRequest
			};
		}
	}
}
