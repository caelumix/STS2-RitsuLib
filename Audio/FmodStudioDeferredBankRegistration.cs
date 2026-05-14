namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Queues FMOD Studio bank and GUID mapping paths until <see cref="DeferredInitializationCompletedEvent" />,
    ///     Queues FMOD Studio bank 和 GUID mapping 路径 until <c>DeferredInitializationCompleted事件</c>,
    ///     then loads them in one batch with a single <see cref="FmodStudioServer.TryWaitForAllLoads" />.
    ///     then 加载 them in one batch 带有 a single <c>FmodStudioServer.TryWaitForAll加载</c>.
    /// </summary>
    public static class FmodStudioDeferredBankRegistration
    {
        private static readonly Lock Gate = new();
        private static readonly HashSet<string> PendingBanks = new(StringComparer.Ordinal);
        private static readonly HashSet<string> PendingGuidFiles = new(StringComparer.Ordinal);
        private static bool _flushHookRegistered;

        /// <summary>
        ///     Queues a bank path to load after deferred initialization (deduplicated).
        ///     Queues a bank 路径 to 加载 之后 deferred initialization (deduplicated).
        /// </summary>
        public static void RegisterBank(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
                return;

            lock (Gate)
            {
                PendingBanks.Add(resourcePath.Trim());
                EnsureFlushHookRegisteredLocked();
            }
        }

        /// <summary>
        ///     Queues a GUID mapping file for <see cref="FmodStudioServer.TryLoadStudioGuidMappings" /> after deferred
        ///     Queues a GUID mapping file 用于 <c>FmodStudioServer.TryLoadStudioGuidMappings</c> 之后 deferred
        ///     initialization (deduplicated).
        ///     中文说明：initialization (deduplicated).
        /// </summary>
        public static void RegisterStudioGuidMappings(string guidMapResourcePath)
        {
            if (string.IsNullOrWhiteSpace(guidMapResourcePath))
                return;

            lock (Gate)
            {
                PendingGuidFiles.Add(guidMapResourcePath.Trim());
                EnsureFlushHookRegisteredLocked();
            }
        }

        private static void EnsureFlushHookRegisteredLocked()
        {
            if (_flushHookRegistered)
                return;

            _flushHookRegistered = true;
            RitsuLibFramework.SubscribeLifecycle<DeferredInitializationCompletedEvent>(_ => FlushPending());
        }

        private static void FlushPending()
        {
            if (FmodStudioServer.TryGet() is null)
            {
                RitsuLibFramework.Logger.Warn(
                    "[Audio] deferred FMOD: FmodServer singleton missing; pending banks/GUID files kept for a later flush."
                );
                return;
            }

            List<string> banks;
            List<string> guids;

            lock (Gate)
            {
                banks = [.. PendingBanks];
                guids = [.. PendingGuidFiles];
                PendingBanks.Clear();
                PendingGuidFiles.Clear();
            }

            if (banks.Count == 0 && guids.Count == 0)
                return;

            foreach (var path in banks)
                FmodStudioServer.TryLoadBank(path);

            foreach (var path in guids)
                FmodStudioServer.TryLoadStudioGuidMappings(path);

            FmodStudioServer.TryWaitForAllLoads();

            RitsuLibFramework.Logger.Info(
                $"[Audio] deferred FMOD flush complete (banks={banks.Count}, guid files={guids.Count})."
            );
        }
    }
}
