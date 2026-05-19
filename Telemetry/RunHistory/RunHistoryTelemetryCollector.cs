using System.Text.Json;
using System.Text.Json.Nodes;
using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib.Compat;

namespace STS2RitsuLib.Telemetry.RunHistory
{
    internal static class RunHistoryTelemetryCollector
    {
        internal static JsonArray BuildLoadedModList()
        {
            var mods = new JsonArray();
            foreach (var mod in Sts2ModManagerCompat.EnumerateLoadedModsWithAssembly()
                         .OrderBy(m => m.manifest?.id ?? m.assembly?.GetName().Name ?? "<unknown>",
                             StringComparer.OrdinalIgnoreCase))
            {
                var assemblyName = mod.assembly?.GetName();
                mods.Add(new JsonObject
                {
                    ["id"] = mod.manifest?.id ?? assemblyName?.Name ?? "<unknown>",
                    ["name"] = mod.manifest?.name ?? assemblyName?.Name ?? "<unknown>",
                    ["version"] = mod.manifest?.version,
                    ["state"] = mod.state.ToString(),
                    ["source"] = mod.modSource.ToString(),
                    ["assembly"] = assemblyName?.Name,
                    ["assembly_version"] = assemblyName?.Version?.ToString(),
                });
            }

            return mods;
        }

        public static void CaptureVanillaRunHistory(
            string applicantId,
            JsonNode runHistory,
            JsonNode? applicantPayload = null,
            IReadOnlyDictionary<string, object?>? properties = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicantId);
            ArgumentNullException.ThrowIfNull(runHistory);

            var payload = new JsonObject
            {
                ["run_history"] = runHistory.DeepClone(),
            };

            if (applicantPayload != null)
                payload["mod_payload"] = applicantPayload.DeepClone();

            var props = properties == null
                ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                : new(properties, StringComparer.OrdinalIgnoreCase);
            props["payload_kind"] = "vanilla_run_history";
            props["json_indented"] = false;

            new TelemetryClient(applicantId).CapturePayload(
                "run_history.completed",
                "run_history",
                payload,
                props);
        }

        internal static void CaptureEndedRun(RunEndedEvent evt)
        {
            JsonNode? runHistory;
            try
            {
                runHistory = JsonSerializer.SerializeToNode(
                    evt.Run,
                    JsonSerializationUtility.GetTypeInfo<SerializableRun>());
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn($"[Telemetry] Failed to serialize ended run history: {ex.Message}");
                return;
            }

            if (runHistory == null)
            {
                RitsuLibFramework.Logger.Warn("[Telemetry] Failed to serialize ended run history: result is null.");
                return;
            }

            var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["is_victory"] = evt.IsVictory,
                ["is_abandoned"] = evt.IsAbandoned,
                ["occurred_at_utc"] = evt.OccurredAtUtc.ToString("O"),
            };

            var capturedApplicants = new List<string>();
            foreach (var applicant in TelemetryRegistry.GetApplicants())
            {
                if (!TelemetryRegistry.TryGetRequest(applicant, "run_history", out var request) ||
                    !TelemetryConsentStore.IsRequestGranted(applicant, request))
                    continue;

                if (!ShouldCaptureForRequest(request, evt, applicant.ApplicantId))
                    continue;

                CaptureVanillaRunHistory(applicant.ApplicantId, runHistory, null, properties);
                capturedApplicants.Add(applicant.ApplicantId);
            }

            RitsuLibFramework.Logger.Info(
                $"[Telemetry] Captured ended run history for {capturedApplicants.Count} authorized applicant(s); abandoned={evt.IsAbandoned}, victory={evt.IsVictory}.");
            foreach (var applicantId in capturedApplicants)
                _ = TelemetryQueue.FlushApplicantAsync(applicantId);
        }

        private static bool ShouldCaptureForRequest(
            TelemetryRequest request,
            RunEndedEvent evt,
            string applicantId)
        {
            if (request.RunHistoryCaptureFilter == null)
                return true;

            try
            {
                return request.RunHistoryCaptureFilter(evt);
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[Telemetry] Run-history capture filter failed for applicant '{applicantId}': {ex.Message}");
                return false;
            }
        }
    }
}
