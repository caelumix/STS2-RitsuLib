namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     How a sidecar payload should be sent and (by convention) interpreted. The first byte of
    ///     How a sidecar payload should be sent 和 (by convention) interpreted. The first byte of
    ///     <see cref="RitsuLibSidecarEnvelope.ParsedEnvelope.HeaderExtension" />, when present, records this; see
    ///     <see cref="RitsuLibSidecarHeaderExtension.GetDeliveryOrUnspecified" />.
    /// </summary>
    public enum RitsuLibSidecarDeliverySemantics : byte
    {
        /// <summary>
        ///     Unordered / loss-tolerant: map to unreliable transport and a best-effort channel. Handlers may run
        ///     Unordered / loss-tolerant: map to unreliable transport 和 a best-effort channel. Handlers may 跑局
        ///     as soon as the frame arrives; no cross-stream ordering is implied.
        ///     中文说明：as soon as the frame arrives; no cross-stream ordering is implied.
        /// </summary>
        BestEffort = 0,

        /// <summary>
        ///     Reliable, ordered with respect to other reliable sidecar traffic on the same ENet stream. This does not
        ///     Reliable, ordered 带有 respect to other reliable sidecar traffic on the same ENet stream. This does not
        ///     by itself marshal handler code to the Godot main thread or merge with vanilla game action serialization;
        ///     by itself marshal handler code to the Godot main thread 或 merge 带有 原版 game action serialization;
        ///     it only selects transport parameters for sidecar envelopes.
        ///     it only selects transport parameters 用于 sidecar envelopes.
        /// </summary>
        StableSync = 1,

        /// <summary>
        ///     Header extension omits a delivery tag; treated like <see cref="StableSync" /> for send helpers.
        ///     Header extension omits a delivery tag; treated like <c>StableSync</c> 用于 send helpers.
        /// </summary>
        Unspecified = 0xFF,
    }
}
