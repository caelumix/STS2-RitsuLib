using STS2RitsuLib.Data;

namespace STS2RitsuLib.Utils.Persistence
{
    /// <summary>
    ///     Profile data lifecycle hub:
    ///     档案 data lifecycle hub:
    ///     - ProfileDataReady: profile data is safe to read/write
    ///     - 档案DataReady: 档案 data is safe to read/write
    ///     - ProfileDataChanged: profile switched after being ready
    ///     - 档案DataChanged: 档案 switched 之后 being ready
    ///     - ProfileDataInvalidated: current ready profile became invalid
    ///     - 档案DataInvalidated: current ready 档案 became invalid
    /// </summary>
    public static class DataReadyLifecycle
    {
        private static readonly Lock SyncRoot = new();

        private static ProfileDataReadyEvent? _lastReadyEvent;

        /// <summary>
        ///     True when profile path initialization completed and data is considered safe to use.
        ///     当 profile path initialization completed and data is considered safe to use 时为 true。
        /// </summary>
        public static bool IsReady { get; private set; }

        /// <summary>
        ///     Profile id associated with the last ready notification, or <c>-1</c> when not ready.
        ///     档案 id associated 带有 the last ready notification, 或 <c>-1</c> 当 not ready.
        /// </summary>
        public static int ReadyProfileId { get; private set; } = -1;

        /// <summary>
        ///     Derived lifecycle state from <see cref="IsReady" />.
        ///     Derived lifecycle state 从 <c>IsReady</c>.
        /// </summary>
        public static DataLifecycleState State =>
            IsReady ? DataLifecycleState.Ready : DataLifecycleState.WaitingForProfile;

        /// <summary>
        ///     Refreshes the current profile, ensures profile services, reloads data if paths changed, and raises
        ///     Refreshes the current 档案, ensures 档案 services, re加载 data 如果 路径 changed, 和 raises
        ///     lifecycle events when appropriate.
        ///     lifecycle 事件s 当 appropriate.
        /// </summary>
        /// <param name="source">
        ///     Diagnostic label for log and event payloads.
        ///     Diagnostic label 用于 log 和 事件 payload.
        /// </param>
        public static void NotifyPotentialReady(string source)
        {
            try
            {
                ProfileManager.Instance.RefreshCurrentProfile();

                RitsuLibFramework.EnsureProfileServicesInitialized();

                var dataReloaded = ModDataStore.ReloadAllIfPathChanged();

                var profileId = ProfileManager.Instance.CurrentProfileId;
                bool isInitialReady;
                int previousProfileId;
                bool isProfileSwitch;
                ProfileDataReadyEvent readyEvent;

                lock (SyncRoot)
                {
                    isInitialReady = !IsReady;
                    previousProfileId = ReadyProfileId;
                    isProfileSwitch = !isInitialReady && previousProfileId != profileId;

                    IsReady = true;
                    ReadyProfileId = profileId;

                    readyEvent = new(
                        profileId,
                        source,
                        isInitialReady,
                        isProfileSwitch,
                        dataReloaded,
                        DateTimeOffset.UtcNow
                    );

                    _lastReadyEvent = readyEvent;
                }

                if (!isInitialReady && !isProfileSwitch)
                    return;

                if (isProfileSwitch)
                    RitsuLibFramework.PublishLifecycleEvent(
                        new ProfileDataChangedEvent(
                            previousProfileId,
                            profileId,
                            source,
                            DateTimeOffset.UtcNow
                        ),
                        nameof(ProfileDataChangedEvent)
                    );

                if (ModDataStore.HasAnyProfileScopedEntries)
                    RitsuLibFramework.Logger.Info($"Data ready for profile {profileId} ({source})");

                RitsuLibFramework.PublishLifecycleEvent(readyEvent, nameof(ProfileDataReadyEvent));
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[Persistence] Failed to notify data ready lifecycle from '{source}': {ex.Message}");
            }
        }

        /// <summary>
        ///     Marks the given profile as invalid and raises
        ///     Marks the given 档案 as invalid 和 raises
        ///     <see cref="STS2RitsuLib.Utils.Persistence.ProfileDataInvalidatedEvent" /> when it was the active ready
        ///     profile.
        ///     档案.
        /// </summary>
        public static void NotifyProfileInvalidated(int profileId, string reason)
        {
            if (profileId < 0)
                return;

            var shouldRaise = false;

            lock (SyncRoot)
            {
                if (IsReady && ReadyProfileId == profileId)
                {
                    IsReady = false;
                    ReadyProfileId = -1;
                    _lastReadyEvent = null;
                    shouldRaise = true;
                }
            }

            if (!shouldRaise)
                return;

            RitsuLibFramework.PublishLifecycleEvent(
                new ProfileDataInvalidatedEvent(profileId, reason, DateTimeOffset.UtcNow),
                nameof(ProfileDataInvalidatedEvent)
            );
        }
    }
}
