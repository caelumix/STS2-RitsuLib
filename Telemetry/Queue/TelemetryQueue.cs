using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Telemetry
{
    internal static class TelemetryQueue
    {
        private const int MaxEventsPerApplicant = 2000;
        private static readonly Lock Sync = new();

        internal static void Enqueue(TelemetryEnvelope envelope)
        {
            lock (Sync)
            {
                var doc = ReadQueue(envelope.ApplicantId);
                doc.Events.Add(envelope);
                if (doc.Events.Count > MaxEventsPerApplicant)
                    doc.Events.RemoveRange(0, doc.Events.Count - MaxEventsPerApplicant);
                WriteQueue(envelope.ApplicantId, doc);
                RitsuLibFramework.Logger.Info(
                    $"[Telemetry] Queued event '{envelope.EventName}' for applicant '{envelope.ApplicantId}'. Queue size: {doc.Events.Count}.");
            }
        }

        public static async Task FlushApplicantAsync(string applicantId, CancellationToken cancellationToken = default)
        {
            TelemetryApplicant applicant;
            TelemetryEnvelope[] batch;

            lock (Sync)
            {
                if (!TelemetryRegistry.TryGetApplicant(applicantId, out applicant))
                {
                    RitsuLibFramework.Logger.Warn($"[Telemetry] Flush skipped: unknown applicant '{applicantId}'.");
                    return;
                }

                var doc = ReadQueue(applicantId);
                if (doc.Events.Count == 0)
                {
                    RitsuLibFramework.Logger.Info($"[Telemetry] Flush skipped for '{applicantId}': queue is empty.");
                    return;
                }

                batch = [.. doc.Events];
            }

            RitsuLibFramework.Logger.Info(
                $"[Telemetry] Sending {batch.Length} queued event(s) for applicant '{applicantId}' via {applicant.Adapter.AdapterId}.");

            TelemetrySendResult result;
            try
            {
                result = await applicant.Adapter.SendAsync(applicant, batch, cancellationToken);
            }
            catch (Exception ex)
            {
                result = TelemetrySendResult.Fail(ex.Message);
            }

            lock (Sync)
            {
                var state = ReadState(applicantId);
                state.LastSendUtc = DateTimeOffset.UtcNow;

                if (result.Success)
                {
                    WriteQueue(applicantId, new());
                    state.LastError = null;
                    state.FailureCount = 0;
                    RitsuLibFramework.Logger.Info(
                        $"[Telemetry] Sent {batch.Length} event(s) for applicant '{applicantId}'.");
                }
                else
                {
                    state.LastError = result.ErrorMessage;
                    state.FailureCount++;
                    RitsuLibFramework.Logger.Warn(
                        $"[Telemetry] Send failed for applicant '{applicantId}': {result.ErrorMessage}");
                }

                WriteState(applicantId, state);
            }
        }

        public static async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            foreach (var applicant in TelemetryRegistry.GetApplicants())
                await FlushApplicantAsync(applicant.ApplicantId, cancellationToken);
        }

        public static void ClearApplicant(string applicantId)
        {
            lock (Sync)
            {
                WriteQueue(applicantId, new());
                RitsuLibFramework.Logger.Info($"[Telemetry] Cleared queue for applicant '{applicantId}'.");
            }
        }

        public static int GetQueuedEventCount(string applicantId)
        {
            lock (Sync)
            {
                return ReadQueue(applicantId).Events.Count;
            }
        }

        private static TelemetryQueueDocument ReadQueue(string applicantId)
        {
            var result = FileOperations.ReadJson<TelemetryQueueDocument>(
                TelemetryPaths.QueuePath(applicantId),
                TelemetryJson.Options,
                "TelemetryQueue");
            return result is { Success: true, Data: not null } ? result.Data : new();
        }

        private static void WriteQueue(string applicantId, TelemetryQueueDocument doc)
        {
            FileOperations.WriteJson(TelemetryPaths.QueuePath(applicantId), doc, TelemetryJson.Options,
                "TelemetryQueue");
        }

        private static TelemetryQueueState ReadState(string applicantId)
        {
            var result = FileOperations.ReadJson<TelemetryQueueState>(
                TelemetryPaths.StatePath(applicantId),
                TelemetryJson.Options,
                "TelemetryQueueState");
            return result is { Success: true, Data: not null } ? result.Data : new();
        }

        private static void WriteState(string applicantId, TelemetryQueueState state)
        {
            FileOperations.WriteJson(TelemetryPaths.StatePath(applicantId), state, TelemetryJson.Options,
                "TelemetryQueueState");
        }
    }
}
