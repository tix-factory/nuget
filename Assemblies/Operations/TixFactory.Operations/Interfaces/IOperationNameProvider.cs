using System;
using System.ComponentModel;

namespace TixFactory.Operations
{
    /// <summary>
    /// A provider of names for operations.
    /// </summary>
    public interface IOperationNameProvider
    {
        /// <summary>
        /// The operation type.
        /// </summary>
        /// <remarks>
        /// The <see cref="DisplayNameAttribute"/> is looked up on the <paramref name="operationType"/>,
        /// and if it is found the value is returned.
        ///
        /// After that, if the class name is suffixed with "Action" or "Operation", it is stripped
        /// and the preceding string of the class name is returned.
        /// 
        /// The type is expected to be the class inheriting one of:
        /// - <see cref="IOperation{TOutput}"/>
        /// - <see cref="IOperation{TInput,TOutput}"/>
        /// - <see cref="IAsyncOperation{TOutput}"/>
        /// - <see cref="IAsyncOperation{TInput,TOutput}"/>
        /// - <see cref="IAction"/>
        /// - <see cref="IAction{TInput}"/>
        /// - <see cref="IAsyncAction"/>
        /// - <see cref="IAsyncAction{TInput}"/>
        /// </remarks>
        /// <param name="operationType">The operation type.</param>
        /// <returns>The name of the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="operationType"/>
        /// </exception>
        string GetOperationName(Type operationType);
    }
}
