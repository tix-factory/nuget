namespace TixFactory.Operations
{
	/// <summary>
	/// A synchronous operation interface.
	/// </summary>
	/// <typeparam name="TInput">The input type for the operation.</typeparam>
	/// <typeparam name="TOutput">The output type for the operation.</typeparam>
	public interface IOperation<in TInput, TOutput>
	{
		/// <summary>
		/// Executes the operation.
		/// </summary>
		/// <param name="input">The <typeparamref name="TInput"/>.</param>
		/// <returns>The <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
		(TOutput output, OperationError error) Execute(TInput input);
	}

	/// <summary>
	/// A synchronous operation interface without input.
	/// </summary>
	/// <typeparam name="TOutput">The output type for the operation.</typeparam>
	public interface IOperation<TOutput>
	{
		/// <summary>
		/// Executes the operation.
		/// </summary>
		/// <returns>The <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
		(TOutput output, OperationError error) Execute();
	}
}
