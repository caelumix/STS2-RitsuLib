namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Builds sidecar envelopes for the current wire layout. Opcodes use <see cref="RitsuLibSidecarOpcodes.For" />
    ///     Builds sidecar envelopes 用于 the current wire layout. Opcodes 使用 <c>RitsuLibSidecarOpcodes.For</c>
    ///     or <see cref="RitsuLibSidecarControlOpcodes" />.
    ///     中文说明：or <c>RitsuLibSidecarControlOpcodes</c>.
    /// </summary>
    public static class RitsuLibSidecar
    {
        /// <summary>
        ///     Builds an envelope. <paramref name="headerExtension" /> is opaque; to record delivery semantics use
        ///     Builds an envelope. <c>headerExtension</c> is opaque; to record delivery semantics 使用
        ///     <see cref="CreateEnvelopeWithDelivery" />.
        /// </summary>
        /// <param name="opcode">
        ///     Sidecar opcode (user or control).
        ///     Sidecar opcode (使用r 或 control).
        /// </param>
        /// <param name="payload">
        ///     Logical payload after the fixed header and optional extension.
        ///     Logical payload 之后 the fixed header 和 可选 extension.
        /// </param>
        /// <param name="extraFlags">
        ///     Wire flags OR’d into the envelope (e.g. gzip).
        ///     中文说明：Wire flags OR’d into the envelope (e.g. gzip).
        /// </param>
        /// <param name="gzipPayload">
        ///     When <c>true</c>, compresses <paramref name="payload" /> and sets
        ///     当 <c>true</c>, compresses <c>payload</c> 和 设置
        ///     <see cref="RitsuLibSidecarWireFlags.PayloadGzip" />.
        /// </param>
        /// <param name="headerExtension">
        ///     Optional bytes between the fixed header and payload.
        ///     可选 bytes between the fixed header 和 payload.
        /// </param>
        public static byte[] CreateEnvelope(
            ulong opcode,
            ReadOnlySpan<byte> payload,
            RitsuLibSidecarWireFlags extraFlags = RitsuLibSidecarWireFlags.None,
            bool gzipPayload = false,
            ReadOnlySpan<byte> headerExtension = default)
        {
            return RitsuLibSidecarEnvelope.Build(
                RitsuLibSidecarWire.CurrentWireFormatVersion,
                extraFlags,
                opcode,
                headerExtension,
                payload,
                gzipPayload);
        }

        /// <summary>
        ///     Builds an envelope with a 1-byte delivery tag plus optional <paramref name="additionalHeaderExtension" />.
        ///     Builds an envelope 带有 a 1-byte delivery tag plus 可选 <c>additionalHeaderExtension</c>.
        ///     <see cref="RitsuLibSidecarDeliverySemantics.Unspecified" /> omits the tag; extension is only
        ///     <paramref name="additionalHeaderExtension" />.
        /// </summary>
        /// <param name="opcode">
        ///     Sidecar opcode (user or control).
        ///     Sidecar opcode (使用r 或 control).
        /// </param>
        /// <param name="payload">
        ///     Logical payload after the fixed header and optional extension.
        ///     Logical payload 之后 the fixed header 和 可选 extension.
        /// </param>
        /// <param name="delivery">
        ///     First byte of the header extension when not
        ///     First byte of the header extension 当 not
        ///     <see cref="RitsuLibSidecarDeliverySemantics.Unspecified" />.
        /// </param>
        /// <param name="extraFlags">
        ///     Wire flags OR’d into the envelope (e.g. gzip).
        ///     中文说明：Wire flags OR’d into the envelope (e.g. gzip).
        /// </param>
        /// <param name="gzipPayload">
        ///     When <c>true</c>, compresses <paramref name="payload" /> and sets
        ///     当 <c>true</c>, compresses <c>payload</c> 和 设置
        ///     <see cref="RitsuLibSidecarWireFlags.PayloadGzip" />.
        /// </param>
        /// <param name="additionalHeaderExtension">
        ///     Bytes after the 1-byte delivery tag in the extension.
        ///     Bytes 之后 the 1-byte delivery tag in the extension.
        /// </param>
        public static byte[] CreateEnvelopeWithDelivery(
            ulong opcode,
            ReadOnlySpan<byte> payload,
            RitsuLibSidecarDeliverySemantics delivery,
            RitsuLibSidecarWireFlags extraFlags = RitsuLibSidecarWireFlags.None,
            bool gzipPayload = false,
            ReadOnlySpan<byte> additionalHeaderExtension = default)
        {
            if (delivery is RitsuLibSidecarDeliverySemantics.Unspecified)
                return CreateEnvelope(opcode, payload, extraFlags, gzipPayload, additionalHeaderExtension);

            var ext = new byte[1 + additionalHeaderExtension.Length];
            ext[0] = (byte)delivery;
            additionalHeaderExtension.CopyTo(ext.AsSpan(1));
            return CreateEnvelope(opcode, payload, extraFlags, gzipPayload, ext);
        }
    }
}
