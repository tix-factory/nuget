using System;
using Microsoft.AspNetCore.Mvc;
using TixFactory.ApplicationContext;

namespace TixFactory.Http.Service
{
	/// <summary>
	/// A controller to verify the application is running.
	/// </summary>
	[ApiController]
	public class ApplicationMetdataController : Controller
	{
		private readonly IApplicationContext _ApplicationContext;

		/// <summary>
		/// Initializes a new <see cref="ApplicationMetdataController"/>.
		/// </summary>
		/// <param name="applicationContext">An <see cref="IApplicationContext"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="applicationContext"/>
		/// </exception>
		public ApplicationMetdataController(IApplicationContext applicationContext)
		{
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
		}

		/// <summary>
		/// Gets the metadata for the running application.
		/// </summary>
		/// <returns></returns>
		[HttpGet("application-metadata")]
		public ApplicationMetadataResponse GetApplicationMetadata()
		{
			return new ApplicationMetadataResponse
			{
				Name = _ApplicationContext.Name
			};
		}
	}
}
