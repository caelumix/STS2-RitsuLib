using System.Text.Json.Nodes;
using STS2RitsuLib.Telemetry.RunHistory;

namespace STS2RitsuLib.Telemetry
{
    internal static class TelemetryEnvelopeFactory
    {
        internal const string BasePayloadOverrideKey = "__ritsulib_base_payload";
        private static readonly string SessionId = Guid.NewGuid().ToString("N");

        internal static TelemetryEnvelope Create(
            TelemetryApplicant applicant,
            TelemetryRequest request,
            string eventName,
            JsonNode? applicantPayload,
            IReadOnlyDictionary<string, object?>? properties)
        {
            var mergedProperties = BuildCommonProperties(applicant);
            if (properties != null)
                foreach (var kvp in properties)
                    mergedProperties[kvp.Key] = kvp.Value;

            var payload = BuildPayload(applicant, request, eventName, applicantPayload);
            return new()
            {
                ApplicantId = applicant.ApplicantId,
                EventName = eventName,
                RequestId = request.RequestId,
                Category = request.Category,
                Properties = mergedProperties,
                Payload = payload,
            };
        }

        private static Dictionary<string, object?> BuildCommonProperties(TelemetryApplicant applicant)
        {
            return new(StringComparer.OrdinalIgnoreCase)
            {
                ["anonymous_install_id"] = TelemetryIdentityStore.AnonymousInstallId,
                ["session_id"] = SessionId,
                ["ritsulib_version"] = Const.Version,
                ["applicant_id"] = applicant.ApplicantId,
                ["owner_mod_id"] = applicant.OwnerModId,
                ["applicant_display_name"] = applicant.ResolveDisplayName(),
                ["platform"] = Environment.OSVersion.Platform.ToString(),
            };
        }

        private static JsonObject BuildPayload(
            TelemetryApplicant applicant,
            TelemetryRequest request,
            string eventName,
            JsonNode? applicantPayload)
        {
            var root = new JsonObject();
            var payloadForEnvelope = applicantPayload?.DeepClone();
            var basePayloadOverride = ExtractBasePayloadOverride(payloadForEnvelope);

            var basePayload = basePayloadOverride ?? BuildBasePayload(request);
            if (basePayload.Count > 0)
                root["base_payload"] = basePayload;

            var shared = BuildSharedContributions(applicant, request, eventName, basePayload);
            if (shared.Count > 0)
                root["shared_contributions"] = shared;

            if (payloadForEnvelope is JsonObject { Count: 0 })
                return root;

            if (payloadForEnvelope != null)
                root["applicant_payload"] = payloadForEnvelope;

            return root;
        }

        private static JsonObject? ExtractBasePayloadOverride(JsonNode? payload)
        {
            if (payload is not JsonObject obj)
                return null;

            if (obj[BasePayloadOverrideKey] is not JsonObject overridePayload)
                return null;

            obj.Remove(BasePayloadOverrideKey);
            return overridePayload.DeepClone().AsObject();
        }

        private static JsonObject BuildBasePayload(TelemetryRequest request)
        {
            return request.Category switch
            {
                TelemetryDataCategory.ModInventory => new()
                {
                    ["loaded_mods"] = RunHistoryTelemetryCollector.BuildLoadedModList(),
                },
                _ => new(),
            };
        }

        private static JsonObject BuildSharedContributions(
            TelemetryApplicant applicant,
            TelemetryRequest request,
            string eventName,
            JsonNode? basePayload)
        {
            var root = new JsonObject();
            var providers = TelemetryRegistry.ResolveSharedContributions(applicant, request);
            foreach (var provider in providers)
            {
                JsonNode? node;
                try
                {
                    node = provider.Build(new()
                    {
                        ApplicantId = applicant.ApplicantId,
                        RequestId = request.RequestId,
                        EventName = eventName,
                        BasePayload = basePayload,
                    });
                }
                catch (Exception ex)
                {
                    RitsuLibFramework.Logger.Warn(
                        $"[Telemetry] Shared contribution '{provider.ContributorModId}/{provider.ContributionId}' failed: {ex.Message}");
                    continue;
                }

                if (node == null)
                    continue;

                if (root[provider.ContributorModId] is not JsonObject byMod)
                {
                    byMod = new();
                    root[provider.ContributorModId] = byMod;
                }

                byMod[provider.ContributionId] = node.DeepClone();
            }

            return root;
        }
    }
}
