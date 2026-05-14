using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     One validation route in the unified sidecar reachability-discovery flow.
    ///     One 有效ation route in the unified sidecar reachability-discovery flow.
    /// </summary>
    public interface IRitsuLibSidecarCapabilityValidationRoute
    {
        /// <summary>
        ///     Route name for diagnostics.
        ///     Route name 用于 diagnostics.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Lower value executes earlier in the validation flow.
        ///     Lower value executes earlier in the 有效ation flow.
        /// </summary>
        int Order { get; }

        /// <summary>
        ///     Returns true when this route can operate for the current net service.
        ///     返回 true when this route can operate for the current net service。
        /// </summary>
        bool IsAvailable(INetGameService netService);

        /// <summary>
        ///     Publishes local out-of-band evidence, if required by this route.
        ///     Publishes local out-of-band evidence, 如果 required 通过 this route.
        /// </summary>
        void PublishLocalEvidence(INetGameService netService);

        /// <summary>
        ///     Resolves one peer reachability verdict; returns null when this route has no verdict.
        ///     解析 one peer reachability verdict; returns null when this route has no verdict。
        /// </summary>
        RitsuLibSidecarPeerReachability? TryResolve(INetGameService netService, ulong peerNetId);
    }
}
