namespace STS2RitsuLib
{
    /// <summary>
    ///     Base type for framework lifecycle notifications published through
    ///     Base type 用于 framework lifecycle notifications published through
    ///     <see cref="RitsuLibFramework.SubscribeLifecycle" />.
    /// </summary>
    public interface IFrameworkLifecycleEvent
    {
        /// <summary>
        ///     UTC timestamp when the event was raised.
        ///     UTC timestamp 当 the 事件 was raised.
        /// </summary>
        DateTimeOffset OccurredAtUtc { get; }
    }

    /// <summary>
    ///     Marker for events that are replayed to new subscribers when <c>replayCurrentState</c> is true.
    ///     Marker 用于 事件s that are replayed to new subscribers 当 <c>replayCurrentState</c> is true.
    /// </summary>
    public interface IReplayableFrameworkLifecycleEvent : IFrameworkLifecycleEvent;

    /// <summary>
    ///     Fired while the RitsuLib framework is initializing (before mods complete setup).
    ///     Fired while the RitsuLib framework is initializing (之前 mods complete 设置up).
    /// </summary>
    /// <param name="FrameworkModId">
    ///     Manifest id of the framework mod.
    ///     中文说明：Manifest id of the framework mod.
    /// </param>
    /// <param name="FrameworkVersion">
    ///     Framework assembly or package version string.
    ///     Framework assembly 或 package version string.
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     When the event was raised.
    ///     当 the 事件 was raised.
    /// </param>
    public readonly record struct FrameworkInitializingEvent(
        string FrameworkModId,
        string FrameworkVersion,
        DateTimeOffset OccurredAtUtc
    ) : IFrameworkLifecycleEvent;

    /// <summary>
    ///     Fired after framework initialization finished.
    ///     Fired 之后 framework initialization finished.
    /// </summary>
    /// <param name="FrameworkModId">
    ///     Manifest id of the framework mod.
    ///     中文说明：Manifest id of the framework mod.
    /// </param>
    /// <param name="IsActive">
    ///     Whether the framework considers itself active for this session.
    ///     表示是否 the framework considers itself active for this session。
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     When the event was raised.
    ///     当 the 事件 was raised.
    /// </param>
    public readonly record struct FrameworkInitializedEvent(
        string FrameworkModId,
        bool IsActive,
        DateTimeOffset OccurredAtUtc
    ) : IReplayableFrameworkLifecycleEvent;

    /// <summary>
    ///     Fired before profile-scoped services are initialized.
    ///     Fired 之前 档案-scoped services are initialized.
    /// </summary>
    /// <param name="OccurredAtUtc">
    ///     When the event was raised.
    ///     当 the 事件 was raised.
    /// </param>
    public readonly record struct ProfileServicesInitializingEvent(
        DateTimeOffset OccurredAtUtc
    ) : IFrameworkLifecycleEvent;

    /// <summary>
    ///     Fired after profile-scoped services are ready.
    ///     Fired 之后 档案-scoped services are ready.
    /// </summary>
    /// <param name="ProfileId">
    ///     Active profile identifier.
    ///     active 档案 identifier.
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     When the event was raised.
    ///     当 the 事件 was raised.
    /// </param>
    public readonly record struct ProfileServicesInitializedEvent(
        int ProfileId,
        DateTimeOffset OccurredAtUtc
    ) : IReplayableFrameworkLifecycleEvent;

    /// <summary>
    ///     Receives strongly typed lifecycle events from <see cref="RitsuLibFramework.SubscribeLifecycle" />.
    ///     Receives strongly typed lifecycle 事件s 从 <c>RitsuLibFramework.SubscribeLifecycle</c>.
    /// </summary>
    public interface ILifecycleObserver
    {
        /// <summary>
        ///     Called for each lifecycle event; implementors typically switch on concrete event types.
        ///     Called 用于 each lifecycle 事件; implementors typically switch on concrete 事件 types.
        /// </summary>
        /// <param name="evt">
        ///     The event instance.
        ///     该 event instance。
        /// </param>
        void OnEvent(IFrameworkLifecycleEvent evt);
    }

    internal sealed class DelegateLifecycleObserver<TEvent>(Action<TEvent> handler) : ILifecycleObserver
        where TEvent : IFrameworkLifecycleEvent
    {
        public void OnEvent(IFrameworkLifecycleEvent evt)
        {
            if (evt is TEvent typedEvent)
                handler(typedEvent);
        }
    }

    internal sealed class LifecycleSubscriptionHolder
    {
        public IDisposable Subscription { get; set; } = null!;
    }

    internal sealed class DelegateLifecycleObserverWithSubscription<TEvent>(
        Action<TEvent, IDisposable> handler,
        LifecycleSubscriptionHolder holder
    ) : ILifecycleObserver
        where TEvent : IFrameworkLifecycleEvent
    {
        public void OnEvent(IFrameworkLifecycleEvent evt)
        {
            if (evt is TEvent typedEvent)
                handler(typedEvent, holder.Subscription);
        }
    }

    internal sealed class FrameworkLifecycleSubscription(Action unsubscribe) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            unsubscribe();
        }
    }
}
