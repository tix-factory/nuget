using System;
using System.Threading;
using TixFactory.ApplicationContext;
using TixFactory.Logging;

namespace TixFactory.Processors
{
    /// <summary>
    /// A base entry class for a background processor application.
    /// </summary>
    public abstract class Program
    {
        private readonly SemaphoreSlim _ApplicationRunLock;

        /// <summary>
        /// The application's <see cref="ILogger"/>.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// The <see cref="IApplicationContext"/>.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Initailizes a new <see cref="Program"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="logger"/>
        /// </exception>
        protected Program(ILogger logger)
            : this(logger, TixFactory.ApplicationContext.ApplicationContext.Singleton)
        {
        }

        /// <summary>
        /// Initailizes a new <see cref="Program"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <param name="applicationContext">An <see cref="IApplicationContext"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="logger"/>
        /// - <paramref name="applicationContext"/>
        /// </exception>
        protected Program(ILogger logger, IApplicationContext applicationContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));

            _ApplicationRunLock = new SemaphoreSlim(0, 1);
        }

        /// <summary>
        /// A method that executes when the application starts.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// A method that executes when the application stops.
        /// </summary>
        /// <remarks>
        /// This method is not intended to be called directly.
        /// </remarks>
        protected abstract void Stop();

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="args">The application command line arguments.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="args"/>
        /// </exception>
        protected void Run(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Logger.Verbose($"Starting {ApplicationContext.Name}...");

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            Console.CancelKeyPress += ConsoleExit;

            try
            {
                Start();
            }
            catch (Exception e)
            {
                Logger.Error($"Unhandled exception starting {ApplicationContext.Name}\n{e}");
                return;
            }

            _ApplicationRunLock.Wait();

            Logger.Verbose($"Stopping {ApplicationContext.Name}...");

            try
            {
                Stop();
            }
            catch (Exception e)
            {
                Logger.Error($"Unhandled exception stopping {ApplicationContext.Name}\n{e}");
            }

            // The logger writes background tasks to send the logs.
            // This is probably not the best way to make sure they've all finished.
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        protected void Exit()
        {
            _ApplicationRunLock.Release();
        }

        private void ProcessExit(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        private void ConsoleExit(object sender, ConsoleCancelEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
            ProcessExit(sender, eventArgs);
        }
    }
}
