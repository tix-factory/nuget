using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TixFactory.Operations;

namespace TixFactory.Http.Service;

/// <inheritdoc cref="IOperationExecutor"/>
public class OperationExecutor : IOperationExecutor
{
    /// <inheritdoc cref="IOperationExecutor.Execute(IAction)"/>
    public IActionResult Execute(IAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var error = action.Execute();
        if (error != null)
        {
            return BuildErrorResult(error);
        }

        return new NoContentResult();
    }

    /// <inheritdoc cref="IOperationExecutor.Execute{TInput}(IAction{TInput}, TInput)"/>
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

    /// <inheritdoc cref="IOperationExecutor.Execute{TOutput}(IOperation{TOutput})"/>
    public IActionResult Execute<TOutput>(IOperation<TOutput> operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var (data, operationError) = operation.Execute();
        return BuildPayloadResult(data, operationError);
    }

    /// <inheritdoc cref="IOperationExecutor.Execute{TInput,TOutput}(IOperation{TInput,TOutput}, TInput)"/>
    public IActionResult Execute<TInput, TOutput>(IOperation<TInput, TOutput> operation, TInput input)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var (data, operationError) = operation.Execute(input);
        return BuildPayloadResult(data, operationError);
    }

    /// <inheritdoc cref="IOperationExecutor.ExecuteAsync(IAsyncAction, CancellationToken)"/>
    public async Task<IActionResult> ExecuteAsync(IAsyncAction action, CancellationToken cancellationToken)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var error = await action.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        if (error != null)
        {
            return BuildErrorResult(error);
        }

        return new NoContentResult();
    }

    /// <inheritdoc cref="IOperationExecutor.ExecuteAsync{TInput}(IAsyncAction{TInput}, TInput, CancellationToken)"/>
    public async Task<IActionResult> ExecuteAsync<TInput>(IAsyncAction<TInput> action, TInput input, CancellationToken cancellationToken)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var error = await action.ExecuteAsync(input, cancellationToken).ConfigureAwait(false);
        if (error != null)
        {
            return BuildErrorResult(error);
        }

        return new NoContentResult();
    }

    /// <inheritdoc cref="IOperationExecutor.ExecuteAsync{TOutput}(IAsyncOperation{TOutput}, CancellationToken)"/>
    public async Task<IActionResult> ExecuteAsync<TOutput>(IAsyncOperation<TOutput> operation, CancellationToken cancellationToken)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var (data, operationError) = await operation.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        return BuildPayloadResult(data, operationError);
    }

    /// <inheritdoc cref="IOperationExecutor.ExecuteAsync{TInput,TOutput}(IAsyncOperation{TInput,TOutput}, TInput, CancellationToken)"/>
    public async Task<IActionResult> ExecuteAsync<TInput, TOutput>(IAsyncOperation<TInput, TOutput> operation, TInput input, CancellationToken cancellationToken)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var (data, operationError) = await operation.ExecuteAsync(input, cancellationToken).ConfigureAwait(false);
        return BuildPayloadResult(data, operationError);
    }

    private static IActionResult BuildPayloadResult<TData>(TData data, OperationError operationError)
    {
        if (operationError != null)
        {
            return BuildErrorResult(operationError);
        }

        var payload = new Payload<TData>(data, null);
        return new JsonResult(payload);
    }

    private static IActionResult BuildErrorResult(OperationError operationError)
    {
        var payload = new Payload<object>(null, operationError);

        return new JsonResult(payload)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}
