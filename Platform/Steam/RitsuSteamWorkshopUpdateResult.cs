namespace STS2RitsuLib.Platform.Steam
{
    internal sealed record RitsuSteamWorkshopUpdateResult(
        bool Available,
        int InspectedCount,
        int NeedsUpdateCount,
        int TriggeredCount,
        int AlreadyQueuedCount,
        int FailedCount,
        string? ErrorMessage = null,
        IReadOnlyList<ulong>? TriggeredItemIds = null)
    {
        internal static RitsuSteamWorkshopUpdateResult Unavailable(string? errorMessage = null)
        {
            return new(false, 0, 0, 0, 0, 0, errorMessage);
        }
    }

    internal sealed record RitsuSteamWorkshopUpdateProgress(
        RitsuSteamWorkshopUpdateProgressStage Stage,
        int CompletedCount,
        int TotalCount,
        int NeedsUpdateCount = 0,
        int QueuedCount = 0,
        int AlreadyQueuedCount = 0,
        int FailedCount = 0);

    internal enum RitsuSteamWorkshopUpdateProgressStage
    {
        Starting,
        ReadingSubscriptions,
        RefreshingDetails,
        InspectingItems,
    }

    internal sealed record RitsuSteamWorkshopDownloadProgress(
        int CompletedCount,
        int TotalCount,
        ulong BytesDownloaded,
        ulong BytesTotal);
}
