using STS2RitsuLib.Telemetry.Integration;

namespace STS2RitsuLib.Telemetry
{
    /// <summary>
    ///     Process-wide registry for telemetry applicants and contribution providers.
    ///     进程级 telemetry 申请方和 contribution provider 注册表。
    /// </summary>
    public static class TelemetryRegistry
    {
        private static readonly Lock Sync = new();

        private static readonly Dictionary<string, TelemetryApplicant> Applicants =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, ITelemetryContributionProvider> ContributionProviders =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Registers or replaces a telemetry applicant.
        ///     注册或替换一个 telemetry 申请方。
        /// </summary>
        public static void RegisterApplicant(TelemetryApplicant applicant)
        {
            ArgumentNullException.ThrowIfNull(applicant);
            ArgumentException.ThrowIfNullOrWhiteSpace(applicant.ApplicantId);
            ArgumentException.ThrowIfNullOrWhiteSpace(applicant.OwnerModId);
            ArgumentException.ThrowIfNullOrWhiteSpace(applicant.DisplayName);
            ArgumentNullException.ThrowIfNull(applicant.Adapter);

            lock (Sync)
            {
                Applicants[applicant.ApplicantId] = applicant;
            }

            RitsuLibFramework.Logger.Info(
                $"[Telemetry] Registered applicant '{applicant.ApplicantId}' -> {applicant.Adapter.EndpointDescription} ({applicant.Requests.Count} request(s)).");
            TelemetrySettingsPages.RegisterApplicantPage(applicant);
        }

        /// <summary>
        ///     Registers or replaces a telemetry contribution provider.
        ///     注册或替换一个 telemetry contribution provider。
        /// </summary>
        public static void RegisterContributionProvider(ITelemetryContributionProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentException.ThrowIfNullOrWhiteSpace(provider.ContributorModId);
            ArgumentException.ThrowIfNullOrWhiteSpace(provider.ContributionId);
            lock (Sync)
            {
                ContributionProviders[BuildContributionKey(provider.ContributorModId, provider.ContributionId)] =
                    provider;
            }

            RitsuLibFramework.Logger.Info(
                $"[Telemetry] Registered contribution '{provider.ContributorModId}/{provider.ContributionId}' ({provider.Category}, {provider.Visibility}).");
        }

        /// <summary>
        ///     Returns a snapshot of registered telemetry applicants.
        ///     返回已注册 telemetry 申请方快照。
        /// </summary>
        public static IReadOnlyList<TelemetryApplicant> GetApplicants()
        {
            lock (Sync)
            {
                return Applicants.Values
                    .OrderBy(x => x.ResolveDisplayName(), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x.ApplicantId, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }

        /// <summary>
        ///     Returns a snapshot of registered contribution providers.
        ///     返回已注册 contribution provider 快照。
        /// </summary>
        public static IReadOnlyList<ITelemetryContributionProvider> GetContributionProviders()
        {
            lock (Sync)
            {
                return ContributionProviders.Values
                    .OrderBy(x => x.ContributorModId, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x.ContributionId, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }

        internal static bool TryGetApplicant(string applicantId, out TelemetryApplicant applicant)
        {
            lock (Sync)
            {
                return Applicants.TryGetValue(applicantId, out applicant!);
            }
        }

        internal static bool TryGetRequest(
            TelemetryApplicant applicant,
            string requestId,
            out TelemetryRequest request)
        {
            request = applicant.Requests.FirstOrDefault(r =>
                string.Equals(r.RequestId, requestId, StringComparison.OrdinalIgnoreCase))!;
            return request != null;
        }

        internal static IReadOnlyList<ITelemetryContributionProvider> ResolveSharedContributions(
            TelemetryApplicant applicant,
            TelemetryRequest request)
        {
            var subscriptions = request.ContributionSubscriptions;
            if (subscriptions.Count == 0)
                return [];

            lock (Sync)
            {
                return ContributionProviders.Values
                    .Where(provider =>
                        provider.Visibility == TelemetryContributionVisibility.SharedToAuthorizedSubscribers)
                    .Where(provider => provider.Category == request.Category)
                    .Where(provider =>
                        subscriptions.Contains(provider.ContributionId, StringComparer.OrdinalIgnoreCase))
                    .Where(provider => TelemetryConsentStore.IsSharedContributionGranted(
                        applicant.ApplicantId,
                        provider.ContributorModId,
                        provider.ContributionId))
                    .ToArray();
            }
        }

        internal static IReadOnlyList<ITelemetryContributionProvider> ResolvePrivateContributions(
            TelemetryApplicant applicant,
            TelemetryRequest request)
        {
            var subscriptions = request.ContributionSubscriptions;
            if (subscriptions.Count == 0)
                return [];

            lock (Sync)
            {
                return ContributionProviders.Values
                    .Where(provider => provider.Visibility == TelemetryContributionVisibility.PrivateToApplicant)
                    .Where(provider => provider.Category == request.Category)
                    .Where(provider =>
                        subscriptions.Contains(provider.ContributionId, StringComparer.OrdinalIgnoreCase))
                    .Where(provider => IsOwnedByApplicant(provider, applicant))
                    .ToArray();
            }
        }

        private static bool IsOwnedByApplicant(
            ITelemetryContributionProvider provider,
            TelemetryApplicant applicant)
        {
            return string.Equals(provider.ContributorModId, applicant.OwnerModId, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(provider.ContributorModId, applicant.ApplicantId, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildContributionKey(string contributorModId, string contributionId)
        {
            return $"{contributorModId}\n{contributionId}";
        }
    }
}
