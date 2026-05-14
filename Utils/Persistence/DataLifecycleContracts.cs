namespace STS2RitsuLib.Utils.Persistence
{
    /// <summary>
    ///     High-level state of whether profile-scoped mod data may be accessed safely.
    ///     High-level state of whether 档案-scoped mod data may be accessed safely.
    /// </summary>
    public enum DataLifecycleState
    {
        /// <summary>
        ///     No active profile context is ready for mod persistence yet.
        ///     No active 档案 context is ready 用于 mod persistence yet.
        /// </summary>
        WaitingForProfile = 0,

        /// <summary>
        ///     Profile path is initialized and mod data operations are expected to be valid.
        ///     档案 路径 is initialized 和 mod data operations are expected to be 有效.
        /// </summary>
        Ready = 1,
    }

    /// <summary>
    ///     Published when profile-scoped mod data becomes readable/writable after initialization or reload.
    ///     Published 当 档案-scoped mod data becomes readable/writable 之后 initialization 或 re加载.
    /// </summary>
    /// <param name="ProfileId">
    ///     Active profile identifier.
    ///     active 档案 identifier.
    /// </param>
    /// <param name="Source">
    ///     Subsystem that triggered the notification.
    ///     中文说明：Subsystem that triggered the notification.
    /// </param>
    /// <param name="IsInitialReady">
    ///     True on the first transition to ready.
    ///     中文说明：True on the first transition to ready.
    /// </param>
    /// <param name="IsProfileSwitch">
    ///     True when the ready profile id changed from a previous ready state.
    ///     当 the ready profile id changed from a previous ready state 时为 true。
    /// </param>
    /// <param name="DataReloaded">
    ///     True when mod data was reloaded due to a path or profile change.
    ///     当 mod data was reloaded due to a path or profile change 时为 true。
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     Timestamp in UTC.
    ///     中文说明：Timestamp in UTC.
    /// </param>
    public readonly record struct ProfileDataReadyEvent(
        int ProfileId,
        string Source,
        bool IsInitialReady,
        bool IsProfileSwitch,
        bool DataReloaded,
        DateTimeOffset OccurredAtUtc
    ) : IReplayableFrameworkLifecycleEvent;

    /// <summary>
    ///     Published when the active profile id changes while the framework considered data ready.
    ///     Published 当 the active 档案 id changes while the framework considered data ready.
    /// </summary>
    /// <param name="OldProfileId">
    ///     Previous profile identifier.
    ///     Previous 档案 identifier.
    /// </param>
    /// <param name="NewProfileId">
    ///     New profile identifier.
    ///     New 档案 identifier.
    /// </param>
    /// <param name="Source">
    ///     Subsystem that triggered the notification.
    ///     中文说明：Subsystem that triggered the notification.
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     Timestamp in UTC.
    ///     中文说明：Timestamp in UTC.
    /// </param>
    public readonly record struct ProfileDataChangedEvent(
        int OldProfileId,
        int NewProfileId,
        string Source,
        DateTimeOffset OccurredAtUtc
    ) : IFrameworkLifecycleEvent;

    /// <summary>
    ///     Published when the current ready profile context is no longer valid (e.g. profile deleted).
    ///     Published 当 the current ready 档案 context is no longer 有效 (e.g. 档案 deleted).
    /// </summary>
    /// <param name="ProfileId">
    ///     Profile that was invalidated.
    ///     档案 that was invalidated.
    /// </param>
    /// <param name="Reason">
    ///     Short diagnostic label.
    ///     中文说明：Short diagnostic label.
    /// </param>
    /// <param name="OccurredAtUtc">
    ///     Timestamp in UTC.
    ///     中文说明：Timestamp in UTC.
    /// </param>
    public readonly record struct ProfileDataInvalidatedEvent(
        int ProfileId,
        string Reason,
        DateTimeOffset OccurredAtUtc
    ) : IFrameworkLifecycleEvent;
}
