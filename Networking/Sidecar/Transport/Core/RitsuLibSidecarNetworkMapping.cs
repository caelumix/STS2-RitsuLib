using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Maps <see cref="RitsuLibSidecarDeliverySemantics" /> to ENet/Steam <see cref="NetTransferMode" /> and channel.
    ///     Maps <c>RitsuLibSidecarDeliverySemantics</c> to ENet/Steam <c>NetTransferMode</c> 和 channel.
    /// </summary>
    public static class RitsuLibSidecarNetworkMapping
    {
        /// <summary>
        ///     <see cref="RitsuLibSidecarDeliverySemantics.BestEffort" /> → unreliable + best-effort channel; all other
        ///     values (including <see cref="RitsuLibSidecarDeliverySemantics.Unspecified" />) → reliable + sidecar
        ///     中文说明：values (including <c>RitsuLibSidecarDeliverySemantics.Unspecified</c>) → reliable + sidecar
        ///     reliable channel.
        ///     中文说明：reliable channel.
        /// </summary>
        /// <param name="semantics">
        ///     Delivery intent from the envelope extension or caller.
        ///     Delivery intent 从 the envelope extension 或 caller.
        /// </param>
        /// <param name="mode">
        ///     Resulting <see cref="NetTransferMode" /> for the vanilla send API.
        ///     Resulting <c>NetTransferMode</c> 用于 the 原版 send API.
        /// </param>
        /// <param name="channel">
        ///     ENet channel index for the vanilla send API.
        ///     ENet channel index 用于 the 原版 send API.
        /// </param>
        public static void GetNetworkParameters(
            RitsuLibSidecarDeliverySemantics semantics,
            out NetTransferMode mode,
            out int channel)
        {
            if (semantics is RitsuLibSidecarDeliverySemantics.BestEffort)
            {
                mode = NetTransferMode.Unreliable;
                channel = RitsuLibSidecarWire.RecommendedUnreliableChannel;
            }
            else
            {
                mode = NetTransferMode.Reliable;
                channel = RitsuLibSidecarWire.RecommendedReliableChannel;
            }
        }
    }
}
