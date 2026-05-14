using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace STS2RitsuLib.Settings.RunSidecar
{
    /// <summary>
    ///     Coordinates run sidecar cache epochs and disk cleanup aligned with vanilla run and save lifecycle. Sidecar
    ///     Coordinates 跑局 sidecar cache epochs 和 disk cleanup aligned 带有 原版 跑局 和 保存 lifecycle. Sidecar
    ///     files are client-local and never mutate vanilla <see cref="SerializableRun" /> network payloads.
    ///     files are client-local 和 never mutate 原版 <c>SerializableRun</c> network payload.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see cref="RunStartedEvent" /> / <see cref="RunLoadedEvent" />: bump <see cref="RunEpoch" />
    ///                 (same moments as <see cref="STS2RitsuLib.Lifecycle.Patches.RunLifecyclePatch" />).
    ///                 (same moments as <c>STS2RitsuLib.Lifecycle.Patches.跑局LifecyclePatch</c>).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="RunEndedEvent" />: delete the ended run’s sidecar folder, then bump epoch (covers
    ///                 <c>ShouldSave == false</c> where vanilla may keep the run save file).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="SaveManager.DeleteCurrentRun" /> / <see cref="SaveManager.DeleteCurrentMultiplayerRun" />:
    ///                 sidecar folder is removed in a Harmony prefix while the save file still exists (main-menu
    ///                 sidecar folder is removed in a Harmony 前置补丁 while the 保存 file still exists (main-menu
    ///                 abandon, normal run end after save deletion, etc.); see
    ///                 abandon, normal 跑局 end 之后 保存 deletion, etc.); see
    ///                 <see cref="STS2RitsuLib.Settings.RunSidecar.Patches.ModRunSidecarSaveDeletionPatches" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="ProfileSwitchedEvent" />: bump epoch. <see cref="ProfileDeletedEvent" />: delete the
    ///                 entire <c>run_sidecar</c> subtree for that profile.
    ///                 entire <c>跑局_sidecar</c> subtree 用于 that 档案.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static class ModRunSidecarSession
    {
        private static int _runEpoch;
        private static readonly Lock InitLock = new();
        private static bool _handlersAttached;

        /// <summary>
        ///     Incremented whenever the active profile or run instance changes; bindings use it to drop stale caches.
        ///     Incremented 当ever the active 档案 或 跑局 instance changes; bindings 使用 it to drop stale caches.
        /// </summary>
        public static int RunEpoch => Volatile.Read(ref _runEpoch);

        internal static void AttachLifecycleHandlers()
        {
            lock (InitLock)
            {
                if (_handlersAttached)
                    return;

                _handlersAttached = true;
                RitsuLibFramework.SubscribeLifecycle<RunStartedEvent>(_ => BumpRunEpoch());
                RitsuLibFramework.SubscribeLifecycle<RunLoadedEvent>(_ => BumpRunEpoch());
                RitsuLibFramework.SubscribeLifecycle<RunEndedEvent>(OnRunEnded);
                RitsuLibFramework.SubscribeLifecycle<ProfileSwitchedEvent>(_ => BumpRunEpoch());
                RitsuLibFramework.SubscribeLifecycle<ProfileDeletedEvent>(OnProfileDeleted);
            }
        }

        private static void BumpRunEpoch()
        {
            Interlocked.Increment(ref _runEpoch);
        }

        private static void OnRunEnded(RunEndedEvent evt)
        {
            try
            {
                var netId = RunManager.Instance?.NetService?.NetId ?? 0UL;
                var fingerprint = ModRunSidecarFingerprint.FromSerializableRun(evt.Run, netId);
                ModRunSidecarStore.TryDeleteRunDirectoryForFingerprint(fingerprint);
            }
            catch
            {
                // best-effort: run teardown must not throw
            }

            BumpRunEpoch();
        }

        private static void OnProfileDeleted(ProfileDeletedEvent evt)
        {
            BumpRunEpoch();
            ModRunSidecarStore.TryDeleteAllForProfile(evt.ProfileId);
        }
    }
}
