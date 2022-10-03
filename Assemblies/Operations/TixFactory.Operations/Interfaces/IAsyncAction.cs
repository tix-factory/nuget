using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Operations
{
	/// <summary>
	/// An asynchronous action interface.
	/// </summary>
	/// <remarks>
	/// Similar to <see cref="IAsyncOperation{TInput,TOutput}"/> but does not have an output.
	/// </remarks>
	/// <typeparam name="TInput">The input type for the action.</typeparam>
	public interface IAsyncAction<in TInput>
	{
		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="input">The <typeparamref name="TInput"/>.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
		/// <returns>A task returning the <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
		Task<OperationError> Execute(TInput input, CancellationToken cancellationToken);
	}
}
