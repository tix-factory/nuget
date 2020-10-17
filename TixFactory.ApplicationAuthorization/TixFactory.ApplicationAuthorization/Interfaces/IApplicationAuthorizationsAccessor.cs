using System;
using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationsAccessor
	{
		/// <summary>
		/// Gets the names of authorized operations for a given ApiKey.
		/// </summary>
		/// <param name="apiKey">The ApiKey (<see cref="Guid"/>).</param>
		/// <returns>The names of the operations the ApiKey is authorized to access.</returns>
		ISet<string> GetAuthorizedOperationNames(Guid apiKey);
	}
}
