using System;
using System.Runtime.Serialization;

namespace TixFactory.Operations
{
	/// <summary>
	/// The operation error model.
	/// </summary>
	[DataContract]
	public class OperationError
	{
		/// <summary>
		/// The error code.
		/// </summary>
		[DataMember(Name = "code")]
		public string Code { get; set; }

		/// <summary>
		/// Initializes a new <see cref="OperationError"/>.
		/// </summary>
		/// <param name="codeEnum">The <see cref="Code"/> as an enum.</param>
		public OperationError(Enum codeEnum)
			: this(codeEnum.ToString())
		{
		}

		/// <summary>
		/// Initializes a new <see cref="OperationError"/>.
		/// </summary>
		/// <param name="code">The <see cref="Code"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="code"/> is null or whitespace.</exception>
		public OperationError(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));
			}

			Code = code;
		}

		/// <summary>
		/// Constructor exists to be able to be used with Newtonsoft
		/// </summary>
		internal OperationError()
		{
		}
	}
}
