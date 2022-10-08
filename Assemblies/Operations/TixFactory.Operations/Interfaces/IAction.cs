namespace TixFactory.Operations
{
    /// <summary>
    /// A synchronous action interface.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="IOperation{TInput,TOutput}"/> but does not have an output.
    /// </remarks>
    /// <typeparam name="TInput">The input type for the action.</typeparam>
    public interface IAction<in TInput>
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="input">The <typeparamref name="TInput"/>.</param>
        /// <returns>The <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
        OperationError Execute(TInput input);
    }

    /// <summary>
    /// A synchronous action interface.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="IAction{TInput}"/> but does not have an input.
    /// </remarks>
    public interface IAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <returns>The <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
        OperationError Execute();
    }
}
