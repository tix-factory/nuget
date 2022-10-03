using System;
using System.Threading;

namespace TixFactory.Configuration
{
    /// <inheritdoc cref="IRefreshAheadSetting{T}"/>
    public class RefreshAheadSetting<T> : ManufacturedSetting<T>, IRefreshAheadSetting<T>
    {
        private readonly Timer _RefreshTimer;
        private readonly ISetting<DateTime?> _LastRefresh;
        private readonly SemaphoreSlim _RefreshLock;
        private readonly SemaphoreSlim _RefreshIntervalLock;

        /// <inheritdoc cref="IRefreshAheadSetting{T}.RefreshException"/>
        public event Action<Exception> RefreshException;

        /// <inheritdoc cref="IRefreshAheadSetting{T}.RefreshInterval"/>
        public ISetting<TimeSpan> RefreshInterval { get; }

        /// <inheritdoc cref="IRefreshAheadSetting{T}.LastRefresh"/>
        public IReadOnlySetting<DateTime?> LastRefresh => _LastRefresh;

        /// <summary>
        /// Initializes a new <see cref="RefreshAheadSetting{T}"/>.
        /// </summary>
        /// <param name="valueFactory">The value factory for the refreshed <see cref="ManufacturedSetting{T}.Value"/>.</param>
        /// <param name="refreshInterval">How often to refresh the <see cref="ManufacturedSetting{T}.Value"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="valueFactory"/>
        /// </exception>
        public RefreshAheadSetting(Func<T> valueFactory, TimeSpan refreshInterval)
            : this(valueFactory, new Setting<TimeSpan>(refreshInterval))
        {
        }

        /// <summary>
        /// Initializes a new <see cref="RefreshAheadSetting{T}"/>.
        /// </summary>
        /// <param name="valueFactory">The value factory for the refreshed <see cref="ManufacturedSetting{T}.Value"/>.</param>
        /// <param name="refreshIntervalSetting">How often to refresh the <see cref="ManufacturedSetting{T}.Value"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="valueFactory"/>
        /// - <paramref name="refreshIntervalSetting"/>
        /// </exception>
        public RefreshAheadSetting(Func<T> valueFactory, ISetting<TimeSpan> refreshIntervalSetting)
            : base(valueFactory, refreshOnRead: false)
        {
            RefreshInterval = refreshIntervalSetting ?? throw new ArgumentNullException(nameof(refreshIntervalSetting));
            _LastRefresh = new Setting<DateTime?>();
            _RefreshLock = new SemaphoreSlim(1, 1);
            _RefreshIntervalLock = new SemaphoreSlim(1, 1);
            _RefreshTimer = new Timer(
                callback: RefreshValue,
                state: null,
                dueTime: TimeSpan.Zero,
                period: refreshIntervalSetting.Value);

            refreshIntervalSetting.Changed += RefreshIntervalChanged;
        }

        /// <inheritdoc cref="IManufacturedSetting{T}.Refresh"/>
        public override void Refresh()
        {
            _RefreshLock.Wait();

            try
            {
                base.Refresh();
                _LastRefresh.Value = DateTime.UtcNow;
            }
            finally
            {
                _RefreshLock.Release();
            }
        }

        /// <inheritdoc cref="ManufacturedSetting{T}.ShouldRefresh"/>
        protected override bool ShouldRefresh()
        {
            if (base.ShouldRefresh())
            {
                // If we should refresh but we're already refreshing, wait for it.
                if (_RefreshLock.CurrentCount == 0)
                {
                    // https://docs.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim.availablewaithandle?view=netframework-4.8
                    // A successful wait on the AvailableWaitHandle does not imply a successful wait on the SemaphoreSlim itself, nor does it decrement the semaphore's count.
                    _RefreshLock.AvailableWaitHandle.WaitOne();
                    return false;
                }

                return true;
            }

            return false;
        }

        private void RefreshValue(object state)
        {
            if (_RefreshIntervalLock.CurrentCount == 0)
            {
                return;
            }

            _RefreshIntervalLock.Wait();

            try
            {
                Refresh();
            }
            catch (Exception e)
            {
                RefreshException?.Invoke(e);
            }
            finally
            {
                _RefreshIntervalLock.Release();
            }
        }

        private void RefreshIntervalChanged(TimeSpan newRefreshInterval, TimeSpan previousRefreshInterval)
        {
            var lastRefresh = _LastRefresh.Value;
            var dueTime = TimeSpan.Zero;

            if (lastRefresh.HasValue)
            {
                var nextRefreshTime = lastRefresh.Value + newRefreshInterval;
                dueTime = nextRefreshTime - DateTime.UtcNow;
            }

            _RefreshTimer.Change(dueTime, newRefreshInterval);
        }
    }
}
