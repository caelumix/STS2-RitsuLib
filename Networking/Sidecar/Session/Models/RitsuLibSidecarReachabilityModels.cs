using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Reachability state for a remote peer from the sidecar sender's point of view.
    ///     Reachability state 用于 a remote peer 从 the sidecar sender's point of view.
    /// </summary>
    public enum RitsuLibSidecarPeerReachability
    {
        /// <summary>
        ///     No safe capability verdict yet.
        ///     中文说明：No safe capability verdict yet.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Peer is confirmed to support sidecar traffic.
        ///     中文说明：Peer is confirmed to support sidecar traffic.
        /// </summary>
        Supported = 1,

        /// <summary>
        ///     Peer is confirmed incompatible and must not receive sidecar packets.
        ///     Peer is confirmed incompatible 和 must not receive sidecar packets.
        /// </summary>
        Unsupported = 2,
    }

    /// <summary>
    ///     Raised when sidecar session binds to a multiplayer <see cref="INetGameService" />.
    ///     Raised 当 sidecar session binds to a multiplayer <c>INetGameService</c>.
    /// </summary>
    public readonly record struct SidecarSessionBoundEvent(INetGameService NetService, long Epoch);

    /// <summary>
    ///     Raised when sidecar session becomes unbound.
    ///     Raised 当 sidecar session becomes unbound.
    /// </summary>
    public readonly record struct SidecarSessionUnboundEvent(long Epoch);

    /// <summary>
    ///     Raised when a peer reachability state transitions.
    ///     Raised 当 a peer reachability state transitions.
    /// </summary>
    public readonly record struct SidecarPeerReachabilityChangedEvent(
        ulong PeerNetId,
        RitsuLibSidecarPeerReachability Previous,
        RitsuLibSidecarPeerReachability Current,
        string Reason,
        long Epoch);

    /// <summary>
    ///     Raised when handshake metadata for a peer is accepted.
    ///     Raised 当 handshake metadata 用于 a peer is accepted.
    /// </summary>
    public readonly record struct SidecarHandshakeCompletedEvent(
        ulong PeerNetId,
        RitsuLibSidecarPeerFeatures Features,
        long Epoch);
}
