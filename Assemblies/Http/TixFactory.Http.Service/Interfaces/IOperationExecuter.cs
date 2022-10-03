using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TixFactory.Operations;

namespace TixFactory.Http.Service
{
    /// <summary>
    /// Executes operation interfaces and translates them to <see cref="IActionResult"/>s.
    /// </summary>
    public interface IOperationExecuter
    {
        /// <summary>
        /// Executes an <see cref="IAction{TInput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <typeparam name="TInput">The action input data type.</typeparam>
        /// <param name="action">The <see cref="IAction{TInput}"/>.</param>
        /// <param name="input">The action input.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="action"/>
        /// </exception>
        IActionResult Execute<TInput>(IAction<TInput> action, TInput input);

        /// <summary>
        /// Executes an <see cref="IOperation{TOutput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="operation">The <see cref="IOperation{TOutput}"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="operation"/>
        /// </exception>
        IActionResult Execute<TOutput>(IOperation<TOutput> operation);

        /// <summary>
        /// Executes an <see cref="IOperation{TOutput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <typeparam name="TInput">The operation input data type.</typeparam>
        /// <typeparam name="TOutput">The operation output data type.</typeparam>
        /// <param name="operation">The <see cref="IOperation{TOutput}"/>.</param>
        /// <param name="input">The operation input.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="operation"/>
        /// </exception>
        IActionResult Execute<TInput, TOutput>(IOperation<TInput, TOutput> operation, TInput input);

        /// <summary>
        /// Executes an <see cref="IAsyncAction{TInput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <typeparam name="TInput">The action input data type.</typeparam>
        /// <param name="action">The <see cref="IAsyncAction{TInput}"/>.</param>
        /// <param name="input">The action input.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="action"/>
        /// </exception>
        Task<IActionResult> ExecuteAsync<TInput>(IAsyncAction<TInput> action, TInput input, CancellationToken cancellationToken);

        /// <summary>
        /// Executes an <see cref="IAsyncOperation{TOutput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="operation">The <see cref="IAsyncOperation{TOutput}"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="operation"/>
        /// </exception>
        Task<IActionResult> ExecuteAsync<TOutput>(IAsyncOperation<TOutput> operation, CancellationToken cancellationToken);

        /// <summary>
        /// Executes an <see cref="IAsyncOperation{TOutput}"/> and converts the result to an <see cref="IActionResult"/>.
        /// </summary>
        /// <typeparam name="TInput">The operation input data type.</typeparam>
        /// <typeparam name="TOutput">The operation output data type.</typeparam>
        /// <param name="operation">The <see cref="IAsyncOperation{TOutput}"/>.</param>
        /// <param name="input">The operation input.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="operation"/>
        /// </exception>
        Task<IActionResult> ExecuteAsync<TInput, TOutput>(IAsyncOperation<TInput, TOutput> operation, TInput input, CancellationToken cancellationToken);
    }
}
