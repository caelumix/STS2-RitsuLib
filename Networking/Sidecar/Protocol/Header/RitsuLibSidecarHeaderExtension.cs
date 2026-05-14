namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Layout of the first byte in <see cref="RitsuLibSidecarEnvelope" /> <c>headerExtension</c> when
    ///     Layout of the first byte in <c>RitsuLibSidecarEnvelope</c> <c>headerExtension</c> 当
    ///     <see cref="RitsuLibSidecarHighLevelSend" /> / <see cref="RitsuLibSidecar.CreateEnvelopeWithDelivery" />
    ///     (delivery-aware helpers) are used. Additional extension bytes, if any, follow this byte in the same buffer.
    ///     (delivery-aware helpers) are used. Additional extension bytes, 如果 any, follow this byte in the same buffer.
    /// </summary>
    public static class RitsuLibSidecarHeaderExtension
    {
        /// <summary>
        ///     Minimum <see cref="RitsuLibSidecarEnvelope.ParsedEnvelope.HeaderExtension" /> when delivery is explicit.
        ///     最小 <c>RitsuLibSidecarEnvelope.ParsedEnvelope.HeaderExtension</c> when delivery is explicit。
        /// </summary>
        public const int MinBytesWithDelivery = RitsuLibSidecarBinaryLayout.ByteSize;

        /// <summary>
        ///     Reads the delivery field; if length is 0, returns <see cref="RitsuLibSidecarDeliverySemantics.Unspecified" />.
        ///     Reads the delivery field; 如果 length is 0, 返回 <c>RitsuLibSidecarDeliverySemantics.Unspecified</c>.
        /// </summary>
        public static RitsuLibSidecarDeliverySemantics GetDeliveryOrUnspecified(ReadOnlyMemory<byte> extension)
        {
            return extension.Length == 0
                ? RitsuLibSidecarDeliverySemantics.Unspecified
                : (RitsuLibSidecarDeliverySemantics)extension.Span[
                    RitsuLibSidecarEnvelopeLayout.DeliveryTagOffsetInExtension];
        }
    }
}
