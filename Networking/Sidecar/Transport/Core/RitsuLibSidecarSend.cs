using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Runs;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Sends raw sidecar envelopes on the vanilla transport without using the game INetMessage serialization path.
    ///     Sends raw sidecar envelopes on the 原版 transport 带有out using the game INetMessage serialization 路径.
    /// </summary>
    public static class RitsuLibSidecarSend
    {
        /// <summary>
        ///     Maps <see cref="NetTransferMode" /> to a recommended ENet channel distinct from vanilla 0/1.
        ///     Maps <c>NetTransferMode</c> to a recommended ENet channel distinct 从 原版 0/1.
        /// </summary>
        /// <param name="mode">
        ///     Reliable or unreliable send mode.
        ///     Reliable 或 unreliable send mode.
        /// </param>
        public static int RecommendedChannel(NetTransferMode mode)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return mode switch
            {
                NetTransferMode.Reliable => RitsuLibSidecarWire.RecommendedReliableChannel,
                NetTransferMode.Unreliable => RitsuLibSidecarWire.RecommendedUnreliableChannel,
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };
        }

        /// <summary>
        ///     Client sends one envelope to the host.
        ///     中文说明：Client sends one envelope to the host.
        /// </summary>
        public static bool TrySendToHost(
            RunManager? runManager,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            return TrySendToHost(runManager?.NetService, envelope, mode, channel);
        }

        /// <summary>
        ///     Client sends one envelope to the host using an existing <see cref="INetGameService" /> (e.g. lobby
        ///     中文说明：Client sends one envelope to the host using an existing <c>INetGameService</c> (e.g. lobby
        ///     before <see cref="RunManager" /> has <see cref="RunManager.NetService" /> assigned).
        ///     之前 <c>跑局Manager</c> has <c>跑局Manager.NetService</c> assigned).
        /// </summary>
        public static bool TrySendToHost(
            INetGameService? netService,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            ArgumentNullException.ThrowIfNull(envelope);
            if (netService is not NetClientGameService { IsConnected: true } client ||
                client.NetClient == null)
                return false;
            if (!RitsuLibSidecarSessionManager.CanSendToPeer(client.HostNetId))
            {
                RitsuLibSidecarNetTrace.TraceSkippedSend(
                    RitsuLibSidecarTransportTracePaths.ClientToHost,
                    client.HostNetId,
                    RitsuLibSidecarSessionManager.TryGetReachability(client.HostNetId, out var r)
                        ? r
                        : RitsuLibSidecarPeerReachability.Unknown);
                return false;
            }

            try
            {
                client.NetClient.SendMessageToHost(envelope, envelope.Length, mode, channel);
            }
            catch (InvalidOperationException)
            {
                RitsuLibSidecarSessionManager.NoteTransportConnectionMissing(client.HostNetId);
                return false;
            }

            RitsuLibSidecarTrafficCounters.AddOutgoing(1, envelope.Length);
            RitsuLibSidecarNetTrace.TraceOutbound(RitsuLibSidecarTransportTracePaths.ClientToHost, envelope, mode,
                channel);
            return true;
        }

        /// <summary>
        ///     Host sends one envelope to a single peer.
        ///     中文说明：Host sends one envelope to a single peer.
        /// </summary>
        public static bool TrySendToPeer(
            RunManager? runManager,
            ulong peerNetId,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            return TrySendToPeer(runManager?.NetService, peerNetId, envelope, mode, channel);
        }

        /// <inheritdoc cref="TrySendToPeer(RunManager?, ulong, byte[], NetTransferMode, int)" />
        public static bool TrySendToPeer(
            INetGameService? netService,
            ulong peerNetId,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            ArgumentNullException.ThrowIfNull(envelope);
            if (netService is not NetHostGameService { IsConnected: true } host || host.NetHost == null)
                return false;
            if (!RitsuLibSidecarSessionManager.CanSendToPeer(peerNetId))
            {
                RitsuLibSidecarNetTrace.TraceSkippedSend(
                    RitsuLibSidecarTransportTracePaths.HostToPeer,
                    peerNetId,
                    RitsuLibSidecarSessionManager.TryGetReachability(peerNetId, out var r)
                        ? r
                        : RitsuLibSidecarPeerReachability.Unknown);
                return false;
            }

            try
            {
                host.NetHost.SendMessageToClient(peerNetId, envelope, envelope.Length, mode, channel);
            }
            catch (InvalidOperationException)
            {
                RitsuLibSidecarSessionManager.NoteTransportConnectionMissing(peerNetId);
                return false;
            }

            RitsuLibSidecarTrafficCounters.AddOutgoing(1, envelope.Length);
            RitsuLibSidecarNetTrace.TraceOutbound(
                RitsuLibSidecarTransportTracePaths.HostToPeer,
                envelope,
                mode,
                channel,
                peerNetId);
            return true;
        }

        /// <summary>
        ///     Host broadcasts to every peer that is ready for vanilla-style broadcast replication.
        ///     Host broadcasts to every peer that is ready 用于 原版-style broadcast replication.
        /// </summary>
        public static bool TryBroadcastToReadyPeers(
            RunManager? runManager,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            return TryBroadcastToReadyPeers(runManager?.NetService, envelope, mode, channel);
        }

        /// <inheritdoc cref="TryBroadcastToReadyPeers(RunManager?, byte[], NetTransferMode, int)" />
        public static bool TryBroadcastToReadyPeers(
            INetGameService? netService,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            ArgumentNullException.ThrowIfNull(envelope);
            if (netService is not NetHostGameService { IsConnected: true } host || host.NetHost == null)
                return false;

            var ops = 0;
            var bytes = 0L;
            foreach (var peer in host.ConnectedPeers)
            {
                if (!peer.readyForBroadcasting)
                    continue;
                if (!RitsuLibSidecarSessionManager.CanSendToPeer(peer.peerId))
                {
                    RitsuLibSidecarNetTrace.TraceSkippedSend(
                        RitsuLibSidecarTransportTracePaths.HostToBroadcastReady,
                        peer.peerId,
                        RitsuLibSidecarSessionManager.TryGetReachability(peer.peerId, out var r)
                            ? r
                            : RitsuLibSidecarPeerReachability.Unknown);
                    continue;
                }

                try
                {
                    host.NetHost.SendMessageToClient(peer.peerId, envelope, envelope.Length, mode, channel);
                }
                catch (InvalidOperationException)
                {
                    RitsuLibSidecarSessionManager.NoteTransportConnectionMissing(peer.peerId);
                    continue;
                }

                ops++;
                bytes += envelope.Length;
            }

            if (ops <= 0)
                return true;

            RitsuLibSidecarTrafficCounters.AddOutgoing(ops, bytes);
            RitsuLibSidecarNetTrace.TraceOutbound(
                RitsuLibSidecarTransportTracePaths.HostToBroadcastReady,
                envelope,
                mode,
                channel,
                broadcastPeerCount: ops);

            return true;
        }

        /// <summary>
        ///     Host sends the same raw envelope to every <see cref="NetHostGameService.ConnectedPeers" /> entry, without
        ///     Host sends the same raw envelope to every <c>NetHostGameService.ConnectedPeers</c> entry, 带有out
        ///     requiring <see cref="MegaCrit.Sts2.Core.Entities.Multiplayer.NetClientData.readyForBroadcasting" />. Use in lobby
        ///     requiring <c>MegaCrit.Sts2.Core.Entities.Multiplayer.NetClientData.readyForBroadcasting</c>. 使用 in lobby
        ///     (or any phase before vanilla marks peers ready) when ready-only broadcast would skip every peer.
        ///     (or any phase 之前 原版 marks peers ready) 当 ready-only broadcast would skip every peer.
        /// </summary>
        public static bool TryBroadcastToAllConnectedClients(
            INetGameService? netService,
            byte[] envelope,
            NetTransferMode mode,
            int channel)
        {
            ArgumentNullException.ThrowIfNull(envelope);
            if (netService is not NetHostGameService { IsConnected: true } host || host.NetHost == null)
                return false;

            var ops = 0;
            var bytes = 0L;
            foreach (var peer in host.ConnectedPeers)
            {
                if (!RitsuLibSidecarSessionManager.CanSendToPeer(peer.peerId))
                {
                    RitsuLibSidecarNetTrace.TraceSkippedSend(
                        RitsuLibSidecarTransportTracePaths.HostToAllConnected,
                        peer.peerId,
                        RitsuLibSidecarSessionManager.TryGetReachability(peer.peerId, out var r)
                            ? r
                            : RitsuLibSidecarPeerReachability.Unknown);
                    continue;
                }

                try
                {
                    host.NetHost.SendMessageToClient(peer.peerId, envelope, envelope.Length, mode, channel);
                }
                catch (InvalidOperationException)
                {
                    RitsuLibSidecarSessionManager.NoteTransportConnectionMissing(peer.peerId);
                    continue;
                }

                ops++;
                bytes += envelope.Length;
            }

            if (ops <= 0)
                return true;

            RitsuLibSidecarTrafficCounters.AddOutgoing(ops, bytes);
            RitsuLibSidecarNetTrace.TraceOutbound(
                RitsuLibSidecarTransportTracePaths.HostToAllConnected,
                envelope,
                mode,
                channel,
                broadcastPeerCount: ops);
            return true;
        }
    }
}
