using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.ApplicationAuthorization
{
	public interface IApplicationAuthorizationsAccessor
	{
		/// <summary>
		/// Gets the names of authorized operations for a given ApiKey.
		/// </summary>
		/// <param name="apiKey">The ApiKey (<see cref="Guid"/>).</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
		/// <returns>The names of the operations the ApiKey is authorized to access.</returns>
		Task<ISet<string>> GetAuthorizedOperationNames(Guid apiKey, CancellationToken cancellationToken);
	}
}
