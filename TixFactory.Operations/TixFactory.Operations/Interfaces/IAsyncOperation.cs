using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Operations
{
	/// <summary>
	/// An asynchronous operation interface.
	/// </summary>
	/// <typeparam name="TInput">The input type for the operation.</typeparam>
	/// <typeparam name="TOutput">The output type for the operation.</typeparam>
	public interface IAsyncOperation<in TInput, TOutput>
	{
		/// <summary>
		/// Executes the operation.
		/// </summary>
		/// <param name="input">The <typeparamref name="TInput"/>.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
		/// <returns>A task returning the <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
		/// <exception cref="TaskCanceledException"><paramref name="cancellationToken"/> is cancelled.</exception>
		Task<(TOutput output, OperationError error)> Execute(TInput input, CancellationToken cancellationToken);
	}

	/// <summary>
	/// An asynchronous operation interface.
	/// </summary>
	/// <typeparam name="TOutput">The output type for the operation.</typeparam>
	public interface IAsyncOperation<TOutput>
	{
		/// <summary>
		/// Executes the operation.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
		/// <returns>A task returning the <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
		/// <exception cref="TaskCanceledException"><paramref name="cancellationToken"/> is cancelled.</exception>
		Task<(TOutput output, OperationError error)> Execute(CancellationToken cancellationToken);
	}
}
