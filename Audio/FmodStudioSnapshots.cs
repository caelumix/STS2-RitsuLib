using Godot;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Mixer snapshots (e.g. pause ducking) as Studio event instances.
    ///     Mixer snapshots (e.g. pause ducking) as Studio 事件 instances.
    /// </summary>
    public static class FmodStudioSnapshots
    {
        /// <summary>
        ///     Creates, starts, and wraps a snapshot instance in a typed handle.
        ///     创建, starts, 和 wraps a snapshot instance in a typed handle.
        /// </summary>
        public static AudioSnapshotHandle? TryStartHandle(string snapshotPath, AudioPlaybackOptions? options = null)
        {
            options ??= new();
            var instance = TryStart(snapshotPath);
            return instance is null
                ? null
                : new AudioSnapshotHandle(AudioSource.Snapshot(snapshotPath),
                    options.ScopeToken?.Scope ?? options.Scope, instance);
        }

        /// <summary>
        ///     Creates and starts a snapshot instance. Caller must <see cref="StopAndRelease" /> when done.
        ///     创建 and starts a snapshot instance. Caller must <c>StopAndRelease</c> when done。
        /// </summary>
        public static GodotObject? TryStart(string snapshotPath)
        {
            var instance = FmodStudioEventInstances.TryCreate(snapshotPath);
            if (instance is null)
                return null;

            return FmodStudioEventInstances.TryStart(instance) ? instance : null;
        }

        /// <summary>
        ///     Same as <see cref="TryStart" />, but uses a snapshot event GUID instead of a path.
        ///     Same as <c>TryStart</c>, but 使用 a snapshot 事件 GUID instead of a 路径.
        /// </summary>
        public static GodotObject? TryStartFromGuid(string snapshotEventGuid)
        {
            var instance = FmodStudioEventInstances.TryCreateFromGuid(snapshotEventGuid);
            if (instance is null)
                return null;

            return FmodStudioEventInstances.TryStart(instance) ? instance : null;
        }

        /// <summary>
        ///     Stops then releases <paramref name="snapshotInstance" />; no-op when null.
        ///     Stops then releases <c>snapshotInstance</c>; no-op 当 null.
        /// </summary>
        public static void StopAndRelease(GodotObject? snapshotInstance, bool allowFadeOut = true)
        {
            if (snapshotInstance is null)
                return;

            FmodStudioEventInstances.TryStop(snapshotInstance, allowFadeOut);
            FmodStudioEventInstances.TryRelease(snapshotInstance);
        }
    }
}
