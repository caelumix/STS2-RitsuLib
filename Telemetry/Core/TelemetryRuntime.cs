using System.Text.Json.Nodes;
using STS2RitsuLib.Telemetry.RunHistory;

namespace STS2RitsuLib.Telemetry
{
    /// <summary>
    ///     Captures process-level telemetry facts once, then replays them to applicants after explicit consent.
    ///     只采集一次进程级 telemetry 事实，并在用户明确授权后回放给各申请方。
    /// </summary>
    internal static class TelemetryRuntime
    {
        private static readonly Lock Sync = new();
        private static readonly HashSet<string> DeliveredStartupKeys = new(StringComparer.OrdinalIgnoreCase);
        private static StartupTelemetrySnapshot? _startupSnapshot;

        /// <summary>
        ///     Captures the startup snapshot once and publishes a replayable lifecycle event for other tasks.
        ///     采集一次启动快照，并发布可重放生命周期事件供其他任务复用。
        /// </summary>
        internal static void CaptureStartupSnapshot()
        {
            lock (Sync)
            {
                if (_startupSnapshot != null)
                    return;

                _startupSnapshot = new(
                    DateTimeOffset.UtcNow,
                    RunHistoryTelemetryCollector.BuildLoadedModList());
            }

            RitsuLibFramework.Logger.Info("[Telemetry] Captured persistent startup telemetry snapshot.");
            RitsuLibFramework.PublishLifecycleEvent(
                new TelemetryStartupSnapshotReadyEvent(_startupSnapshot.CapturedAtUtc, DateTimeOffset.UtcNow),
                nameof(TelemetryStartupSnapshotReadyEvent));
        }

        /// <summary>
        ///     Replays cached startup events to every currently authorized applicant.
        ///     将已缓存的启动事件回放给当前已授权的所有申请方。
        /// </summary>
        internal static void ReplayStartupSnapshotToAuthorizedApplicants()
        {
            foreach (var applicant in TelemetryRegistry.GetApplicants())
                ReplayStartupSnapshotToApplicant(applicant.ApplicantId);
        }

        /// <summary>
        ///     Refreshes the cached loaded-mod list after the host has finished mod initialization.
        ///     在宿主完成 mod 初始化后刷新缓存的已加载 mod 列表。
        /// </summary>
        internal static void RefreshStartupModInventorySnapshot(string reason)
        {
            lock (Sync)
            {
                if (_startupSnapshot == null)
                    return;

                _startupSnapshot = _startupSnapshot with
                {
                    LoadedMods = RunHistoryTelemetryCollector.BuildLoadedModList(),
                };
            }

            RitsuLibFramework.Logger.Info($"[Telemetry] Refreshed startup mod inventory snapshot ({reason}).");
        }

        /// <summary>
        ///     Replays cached startup events to a single applicant, if the user authorized matching requests.
        ///     如果用户授权了对应申请，则将已缓存的启动事件回放给单个申请方。
        /// </summary>
        internal static void ReplayStartupSnapshotToApplicant(string applicantId)
        {
            StartupTelemetrySnapshot snapshot;

            lock (Sync)
            {
                if (_startupSnapshot == null)
                    return;

                snapshot = _startupSnapshot;
            }

            if (!TelemetryRegistry.TryGetApplicant(applicantId, out var applicant))
                return;

            TryReplayStartupEvent(applicant, "basic_usage", "session_start", snapshot.BuildSessionStartPayload());
            TryReplayStartupEvent(applicant, "mod_inventory", "mod_inventory", snapshot.BuildModInventoryPayload());
        }

        private static void TryReplayStartupEvent(
            TelemetryApplicant applicant,
            string requestId,
            string eventName,
            JsonNode payload)
        {
            if (!TelemetryRegistry.TryGetRequest(applicant, requestId, out var request))
                return;

            if (!TelemetryConsentStore.IsRequestGranted(applicant, request))
                return;

            var deliveryKey = $"{applicant.ApplicantId}\n{requestId}\n{eventName}";
            lock (Sync)
            {
                if (!DeliveredStartupKeys.Add(deliveryKey))
                    return;
            }

            RitsuLibFramework.Logger.Info(
                $"[Telemetry] Replaying startup event '{eventName}' to applicant '{applicant.ApplicantId}'.");
            new TelemetryClient(applicant.ApplicantId).CapturePayload(eventName, requestId, payload);
        }

        private sealed record StartupTelemetrySnapshot(DateTimeOffset CapturedAtUtc, JsonArray LoadedMods)
        {
            /// <summary>
            ///     Builds the lightweight session-start payload. Common envelope properties carry version/platform ids.
            ///     构建轻量 session-start 载荷。版本、平台等通用字段由 envelope properties 携带。
            /// </summary>
            public JsonObject BuildSessionStartPayload()
            {
                return new()
                {
                    ["captured_at_utc"] = CapturedAtUtc.ToString("O"),
                };
            }

            /// <summary>
            ///     Builds the mod-inventory trigger payload; the envelope factory attaches the actual loaded-mod list.
            ///     构建 mod-inventory 触发载荷；实际已加载 mod 清单由 envelope factory 附加。
            /// </summary>
            public JsonObject BuildModInventoryPayload()
            {
                var basePayload = new JsonObject
                {
                    ["loaded_mods"] = LoadedMods.DeepClone(),
                };

                return new()
                {
                    ["captured_at_utc"] = CapturedAtUtc.ToString("O"),
                    [TelemetryEnvelopeFactory.BasePayloadOverrideKey] = basePayload,
                };
            }
        }
    }
}
