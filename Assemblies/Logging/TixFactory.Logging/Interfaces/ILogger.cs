using System;

namespace TixFactory.Logging
{
    /// <summary>
    /// An interface for a logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Calls <see cref="Write"/> with <see cref="LogLevel.Verbose"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        void Verbose(string message);

        /// <summary>
        /// Calls <see cref="Write"/> with <see cref="LogLevel.Information"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Calls <see cref="Write"/> with <see cref="LogLevel.Warning"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Calls <see cref="Write"/> with <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <param name="ex">The exception to be converted into the message.</param>
        void Error(Exception ex);

        /// <summary>
        /// Calls <see cref="Write"/> with <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
        /// <param name="message">The message.</param>
        void Write(LogLevel logLevel, string message);
    }
}
