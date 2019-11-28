using System;

namespace TixFactory.Firebase
{
	/// <summary>
	/// Sends messages through firebase.
	/// </summary>
	public interface IFirebaseMessageSender
	{
		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <param name="topic">The topic to send to.</param>
		/// <param name="data">The message data.</param>
		/// <exception cref="ArgumentException"><paramref name="topic"/> is null or whitespace.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="data"/></exception>
		void Send(string topic, object data);
	}
}
