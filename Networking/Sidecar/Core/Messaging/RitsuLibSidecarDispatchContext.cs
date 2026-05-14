using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Arguments for a received sidecar envelope after magic detection, length checks, optional decompression, and
    ///     Arguments 用于 a received sidecar envelope 之后 magic detection, length checks, 可选 decompression, and
    ///     opcode dispatch. Payload and header extension memory may reference the transient receive buffer until the
    ///     opcode dispatch. Payload 和 header extension memory may reference the transient receive buffer until the
    ///     callback returns; use <see cref="WithOwnedEnvelopeMemory" /> before deferring work.
    ///     callback 返回; 使用 <c>WithOwnedEnvelopeMemory</c> 之前 deferring work.
    /// </summary>
    public readonly struct RitsuLibSidecarDispatchContext
    {
        /// <summary>
        ///     Creates a dispatch context for an opcode handler.
        ///     创建 a dispatch context for an opcode handler。
        /// </summary>
        /// <param name="senderNetId">
        ///     Vanilla sender peer id from the receive callback.
        ///     原版 sender peer id 从 the receive callback.
        /// </param>
        /// <param name="transferMode">
        ///     Reliable or unreliable as reported by the transport.
        ///     Reliable 或 unreliable as reported 通过 the transport.
        /// </param>
        /// <param name="channel">
        ///     ENet channel the packet arrived on.
        ///     中文说明：ENet channel the packet arrived on.
        /// </param>
        /// <param name="isHostIngest">
        ///     True when the host service received the packet.
        ///     当 the host service received the packet 时为 true。
        /// </param>
        /// <param name="envelope">
        ///     Parsed sidecar envelope for this packet.
        ///     Parsed sidecar envelope 用于 this packet.
        /// </param>
        public RitsuLibSidecarDispatchContext(
            ulong senderNetId,
            NetTransferMode transferMode,
            int channel,
            bool isHostIngest,
            RitsuLibSidecarEnvelope.ParsedEnvelope envelope)
        {
            SenderNetId = senderNetId;
            TransferMode = transferMode;
            Channel = channel;
            IsHostIngest = isHostIngest;
            Envelope = envelope;
        }

        /// <summary>
        ///     Sender id from the vanilla transport callback.
        ///     Sender id 从 the 原版 transport callback.
        /// </summary>
        public ulong SenderNetId { get; }

        /// <summary>
        ///     Reliable or unreliable delivery mode.
        ///     Reliable 或 unreliable delivery mode.
        /// </summary>
        public NetTransferMode TransferMode { get; }

        /// <summary>
        ///     ENet channel index.
        ///     中文说明：ENet channel index.
        /// </summary>
        public int Channel { get; }

        /// <summary>
        ///     True when this packet was ingested on <see cref="MegaCrit.Sts2.Core.Multiplayer.NetHostGameService" />.
        ///     当 this packet was ingested on <c>MegaCrit.Sts2.Core.Multiplayer.NetHostGameService</c> 时为 true。
        /// </summary>
        public bool IsHostIngest { get; }

        /// <summary>
        ///     Full parsed envelope.
        ///     中文说明：Full parsed envelope.
        /// </summary>
        public RitsuLibSidecarEnvelope.ParsedEnvelope Envelope { get; }

        /// <summary>
        ///     Convenience: <see cref="RitsuLibSidecarEnvelope.ParsedEnvelope.Opcode" />.
        ///     中文说明：Convenience: <c>RitsuLibSidecarEnvelope.ParsedEnvelope.Opcode</c>.
        /// </summary>
        public ulong Opcode => Envelope.Opcode;

        /// <summary>
        ///     Convenience: logical payload memory.
        ///     中文说明：Convenience: logical payload memory.
        ///     Convenience: logical payload memory.
        ///     中文说明：Convenience: logical payload memory.
        /// </summary>
        public ReadOnlyMemory<byte> Payload => Envelope.Payload;

        /// <summary>
        ///     Copies header extension and logical payload into new arrays so the context stays valid after the
        ///     Copies header extension 和 logical payload into new arrays so the context stays 有效 之后 the
        ///     multiplayer receive callback returns or when work is deferred to the Godot main loop.
        ///     multiplayer receive callback 返回 或 当 work is deferred to the Godot main loop.
        /// </summary>
        public RitsuLibSidecarDispatchContext WithOwnedEnvelopeMemory()
        {
            var ext = Envelope.HeaderExtension.Length == 0
                ? ReadOnlyMemory<byte>.Empty
                : Envelope.HeaderExtension.ToArray();
            var pay = Envelope.Payload.Length == 0
                ? ReadOnlyMemory<byte>.Empty
                : Envelope.Payload.ToArray();
            var owned = new RitsuLibSidecarEnvelope.ParsedEnvelope(
                Envelope.WireFormatVersion,
                Envelope.Flags,
                Envelope.Opcode,
                ext,
                pay);
            return new(SenderNetId, TransferMode, Channel, IsHostIngest, owned);
        }
    }
}
