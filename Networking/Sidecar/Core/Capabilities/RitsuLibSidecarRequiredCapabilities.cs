namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Required capability validation policy.
    ///     Required capability 有效ation policy.
    /// </summary>
    public enum RitsuLibSidecarRequiredCapabilityPolicy
    {
        /// <summary>
        ///     Emit warnings but allow begin-run flow to continue.
        ///     Emit warnings but allow begin-跑局 flow to continue.
        /// </summary>
        Warn = 0,

        /// <summary>
        ///     Block begin-run when validation fails.
        ///     Block begin-跑局 当 有效ation fails.
        /// </summary>
        Fail = 1,
    }

    /// <summary>
    ///     Event payload produced after required capability validation.
    ///     事件 payload produced 之后 required capability 有效ation.
    /// </summary>
    public readonly record struct SidecarRequiredCapabilityCheckCompletedEvent(
        bool Passed,
        RitsuLibSidecarRequiredCapabilityPolicy Policy,
        IReadOnlyList<SidecarRequiredCapabilityMiss> MissingByPeer);

    /// <summary>
    ///     Missing required capabilities for one peer.
    ///     Missing required capabilities 用于 one peer.
    /// </summary>
    public readonly record struct SidecarRequiredCapabilityMiss(
        ulong PeerNetId,
        IReadOnlyList<string> MissingCapabilities);

    /// <summary>
    ///     Registry and validator for required sidecar capabilities.
    ///     注册表 和 有效ator 用于 required sidecar capabilities.
    /// </summary>
    public static class RitsuLibSidecarRequiredCapabilities
    {
        private static readonly Lock Gate = new();
        private static readonly Dictionary<string, Func<ulong, bool>> CapabilityChecks = [];

        /// <summary>
        ///     Validation policy used during pre-run checks.
        ///     Validation policy used 期间 pre-跑局 checks.
        /// </summary>
        public static RitsuLibSidecarRequiredCapabilityPolicy Policy { get; set; } =
            RitsuLibSidecarRequiredCapabilityPolicy.Warn;

        /// <summary>
        ///     Raised after each validation run.
        ///     Raised 之后 each 有效ation 跑局.
        /// </summary>
        public static event Action<SidecarRequiredCapabilityCheckCompletedEvent>? CheckCompleted;

        /// <summary>
        ///     Registers one required capability evaluator.
        ///     注册 one required capability evaluator。
        /// </summary>
        public static void RegisterRequiredCapability(string capabilityKey, Func<ulong, bool> evaluator)
        {
            ArgumentException.ThrowIfNullOrEmpty(capabilityKey);
            ArgumentNullException.ThrowIfNull(evaluator);
            lock (Gate)
            {
                CapabilityChecks[capabilityKey] = evaluator;
            }
        }

        /// <summary>
        ///     Validates required capabilities for the specified peer set.
        ///     Validates required capabilities 用于 the specified peer 设置.
        /// </summary>
        public static bool ValidatePeers(IEnumerable<ulong> peerNetIds, out SidecarRequiredCapabilityMiss[] misses)
        {
            KeyValuePair<string, Func<ulong, bool>>[] checks;
            lock (Gate)
            {
                checks = [..CapabilityChecks];
            }

            var missList = new List<SidecarRequiredCapabilityMiss>();
            foreach (var peerId in peerNetIds.Distinct())
            {
                var missing = new List<string>();
                for (var i = 0; i < checks.Length; i++)
                    if (!checks[i].Value(peerId))
                        missing.Add(checks[i].Key);

                if (missing.Count > 0)
                    missList.Add(new(peerId, missing));
            }

            misses = [..missList];
            var passed = misses.Length == 0 || Policy == RitsuLibSidecarRequiredCapabilityPolicy.Warn;
            CheckCompleted?.Invoke(new(passed, Policy, misses));
            return passed;
        }
    }
}
