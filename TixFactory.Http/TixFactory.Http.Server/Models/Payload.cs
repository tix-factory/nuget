using System.Runtime.Serialization;
using TixFactory.Operations;

namespace TixFactory.Http.Server
{
	/// <summary>
	/// A model to wrap response data.
	/// </summary>
	/// <typeparam name="TData">The response data type.</typeparam>
	[DataContract]
	public class Payload<TData>
	{
		/// <summary>
		/// The response data.
		/// </summary>
		[DataMember(Name = "data")]
		public TData Data { get; set; }

		/// <summary>
		/// An error if there is one.
		/// </summary>
		[DataMember(Name = "error")]
		public OperationError Error { get; set; }

		/// <summary>
		/// Initializes a new <see cref="Payload{TData}"/>.
		/// </summary>
		/// <param name="data">The <see cref="Data"/>.</param>
		/// <param name="error">The <see cref="Error"/>.</param>
		public Payload(TData data, OperationError error)
		{
			if (error != null)
			{
				Error = error;
			}
			else
			{
				Data = data;
			}
		}
	}
}
