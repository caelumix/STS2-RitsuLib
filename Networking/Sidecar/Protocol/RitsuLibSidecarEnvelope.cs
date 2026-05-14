using System.Buffers.Binary;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Parses and builds sidecar envelopes: fixed magic, wire version, flags, 64-bit opcode, optional header
    ///     Parses 和 builds sidecar envelopes: fixed magic, wire version, flags, 64-bit opcode, 可选 header
    ///     extension, then payload.
    ///     extension, then payload.
    /// </summary>
    public static class RitsuLibSidecarEnvelope
    {
        /// <summary>
        ///     Result of parsing an on-wire envelope.
        ///     中文说明：Result of parsing an on-wire envelope.
        /// </summary>
        public enum ParseOutcome
        {
            /// <summary>
            ///     Parse succeeded.
            ///     中文说明：Parse succeeded.
            /// </summary>
            Ok,

            /// <summary>
            ///     Packet shorter than the minimum header.
            ///     中文说明：Packet shorter than the minimum header.
            /// </summary>
            TooSmall,

            /// <summary>
            ///     Magic mismatch.
            ///     中文说明：Magic mismatch.
            /// </summary>
            BadMagic,

            /// <summary>
            ///     Wire format version is zero or greater than <see cref="RitsuLibSidecarWire.SupportedWireFormatVersionMax" />.
            ///     Wire 用于mat version is zero 或 greater than <c>RitsuLibSidecarWire.SupportedWireFormatVersionMax</c>.
            /// </summary>
            WireVersionUnsupported,

            /// <summary>
            ///     Declared payload length invalid, gzip corrupt, or decompressed size over cap.
            ///     Declared payload length invalid, gzip corrupt, 或 decompressed size over cap.
            /// </summary>
            PayloadLengthInvalid,

            /// <summary>
            ///     Header extension length over cap.
            ///     中文说明：Header extension length over cap.
            /// </summary>
            ExtensionLengthInvalid,

            /// <summary>
            ///     Total packet length does not match header fields.
            ///     中文说明：Total packet length does not match header fields.
            /// </summary>
            TotalLengthMismatch,
        }

        private const uint KnownWireFlagsMask = (uint)RitsuLibSidecarWireFlags.PayloadGzip;

        /// <summary>
        ///     Parses an envelope from a byte array backing store.
        ///     Parses an envelope 从 a byte array backing store.
        /// </summary>
        /// <param name="packet">
        ///     Full on-wire bytes; slice views reference this array.
        ///     中文说明：Full on-wire bytes; slice views reference this array.
        /// </param>
        /// <param name="parsed">
        ///     Populated when the return value is <see cref="ParseOutcome.Ok" />.
        ///     Populated 当 the 返回 value is <c>ParseOutcome.Ok</c>.
        /// </param>
        public static ParseOutcome TryParse(byte[] packet, out ParsedEnvelope parsed)
        {
            return TryParse(packet.AsSpan(), packet, out parsed);
        }

        /// <summary>
        ///     Parses an envelope; <paramref name="backing" /> must be the same array as <paramref name="packet" /> spans.
        ///     中文说明：Parses an envelope; <c>backing</c> must be the same array as <c>packet</c> spans.
        /// </summary>
        /// <param name="packet">
        ///     Full on-wire bytes as a span over <paramref name="backing" />.
        ///     中文说明：Full on-wire bytes as a span over <c>backing</c>.
        /// </param>
        /// <param name="backing">
        ///     Array used to construct <see cref="ReadOnlyMemory{T}" /> for extension and payload.
        ///     Array used to construct <c>ReadOnlyMemory{T}</c> 用于 extension 和 payload.
        /// </param>
        /// <param name="parsed">
        ///     Populated when the return value is <see cref="ParseOutcome.Ok" />.
        ///     Populated 当 the 返回 value is <c>ParseOutcome.Ok</c>.
        /// </param>
        public static ParseOutcome TryParse(ReadOnlySpan<byte> packet, byte[] backing, out ParsedEnvelope parsed)
        {
            parsed = default;
            if (packet.Length < RitsuLibSidecarWire.MinEnvelopeSize)
                return ParseOutcome.TooSmall;

            if (!RitsuLibSidecarWire.MatchesMagic(packet))
                return ParseOutcome.BadMagic;

            var wireVersion = BinaryPrimitives.ReadUInt16BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.WireVersionOffset,
                    RitsuLibSidecarEnvelopeLayout.WireVersionSize));
            var flagsRaw = BinaryPrimitives.ReadUInt32BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.FlagsOffset, RitsuLibSidecarEnvelopeLayout.FlagsSize));
            var opcode = BinaryPrimitives.ReadUInt64BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.OpcodeOffset, RitsuLibSidecarEnvelopeLayout.OpcodeSize));
            var payloadLen = BinaryPrimitives.ReadUInt32BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.PayloadLengthOffset,
                    RitsuLibSidecarEnvelopeLayout.PayloadLengthSize));
            var extLen = BinaryPrimitives.ReadUInt32BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.ExtensionLengthOffset,
                    RitsuLibSidecarEnvelopeLayout.ExtensionLengthSize));

            if (wireVersion is 0 or > RitsuLibSidecarWire.SupportedWireFormatVersionMax)
                return ParseOutcome.WireVersionUnsupported;

            if (payloadLen > RitsuLibSidecarWire.MaxPayloadBytes)
                return ParseOutcome.PayloadLengthInvalid;

            if (extLen > RitsuLibSidecarWire.MaxHeaderExtensionBytes)
                return ParseOutcome.ExtensionLengthInvalid;

            var flags = (RitsuLibSidecarWireFlags)(flagsRaw & KnownWireFlagsMask);
            var total = RitsuLibSidecarEnvelopeLayout.FixedHeaderSize + extLen + payloadLen;
            if (total != packet.Length)
                return ParseOutcome.TotalLengthMismatch;

            var extMem = extLen == 0
                ? ReadOnlyMemory<byte>.Empty
                : new(backing, RitsuLibSidecarEnvelopeLayout.FixedHeaderSize, (int)extLen);

            var payloadOffset = RitsuLibSidecarEnvelopeLayout.FixedHeaderSize + (int)extLen;
            var rawPayload = new ReadOnlyMemory<byte>(backing, payloadOffset, (int)payloadLen);

            ReadOnlyMemory<byte> logicalPayload;
            if ((flags & RitsuLibSidecarWireFlags.PayloadGzip) != 0)
            {
                if (!RitsuLibSidecarCompression.TryGunzip(rawPayload.Span, out var decompressed))
                    return ParseOutcome.PayloadLengthInvalid;

                logicalPayload = decompressed;
            }
            else
            {
                logicalPayload = rawPayload;
            }

            parsed = new(wireVersion, flags, opcode, extMem, logicalPayload);
            return ParseOutcome.Ok;
        }

        /// <summary>
        ///     Builds a complete on-wire envelope. <paramref name="headerExtension" /> is copied after the fixed
        ///     Builds a complete on-wire envelope. <c>headerExtension</c> is copied 之后 the fixed
        ///     header for forward-compatible optional fields.
        ///     header 用于 用于ward-compatible 可选 fields.
        /// </summary>
        /// <param name="wireFormatVersion">
        ///     Wire format version field; must be within the supported range.
        ///     Wire 用于mat version field; must be 带有in the supported range.
        /// </param>
        /// <param name="flags">
        ///     Wire flags; gzip may be set when <paramref name="gzipLogicalPayload" /> is <c>true</c>.
        ///     Wire flags; gzip may be 设置 当 <c>gzipLogicalPayload</c> is <c>true</c>.
        /// </param>
        /// <param name="opcode">
        ///     64-bit sidecar opcode.
        ///     中文说明：64-bit sidecar opcode.
        /// </param>
        /// <param name="headerExtension">
        ///     Optional bytes after the fixed header, before the payload.
        ///     可选 bytes 之后 the fixed header, 之前 the payload.
        /// </param>
        /// <param name="payloadLogical">
        ///     Uncompressed logical payload; may be compressed when
        ///     Uncompressed logical payload; may be compressed 当
        ///     <paramref name="gzipLogicalPayload" /> is <c>true</c>.
        /// </param>
        /// <param name="gzipLogicalPayload">
        ///     When <c>true</c>, compresses the payload and ORs in
        ///     当 <c>true</c>, compresses the payload 和 ORs in
        ///     <see cref="RitsuLibSidecarWireFlags.PayloadGzip" />.
        /// </param>
        public static byte[] Build(
            ushort wireFormatVersion,
            RitsuLibSidecarWireFlags flags,
            ulong opcode,
            ReadOnlySpan<byte> headerExtension,
            ReadOnlySpan<byte> payloadLogical,
            bool gzipLogicalPayload)
        {
            if (wireFormatVersion is 0 or > RitsuLibSidecarWire.SupportedWireFormatVersionMax)
                throw new ArgumentOutOfRangeException(nameof(wireFormatVersion));

            if (headerExtension.Length > RitsuLibSidecarWire.MaxHeaderExtensionBytes)
                throw new ArgumentOutOfRangeException(nameof(headerExtension));

            var wirePayload = payloadLogical;
            if (gzipLogicalPayload)
            {
                var compressed = RitsuLibSidecarCompression.GzipCompress(payloadLogical);
                wirePayload = compressed;
                flags |= RitsuLibSidecarWireFlags.PayloadGzip;
            }

            if (wirePayload.Length > RitsuLibSidecarWire.MaxPayloadBytes)
                throw new ArgumentOutOfRangeException(nameof(payloadLogical));

            var total = RitsuLibSidecarWire.MinEnvelopeSize + headerExtension.Length + wirePayload.Length;
            var buffer = new byte[total];
            var span = buffer.AsSpan();
            RitsuLibSidecarWire.Magic.CopyTo(span);
            BinaryPrimitives.WriteUInt16BigEndian(
                span.Slice(RitsuLibSidecarEnvelopeLayout.WireVersionOffset,
                    RitsuLibSidecarEnvelopeLayout.WireVersionSize),
                wireFormatVersion);
            BinaryPrimitives.WriteUInt32BigEndian(
                span.Slice(RitsuLibSidecarEnvelopeLayout.FlagsOffset, RitsuLibSidecarEnvelopeLayout.FlagsSize),
                (uint)flags);
            BinaryPrimitives.WriteUInt64BigEndian(
                span.Slice(RitsuLibSidecarEnvelopeLayout.OpcodeOffset, RitsuLibSidecarEnvelopeLayout.OpcodeSize),
                opcode);
            BinaryPrimitives.WriteUInt32BigEndian(
                span.Slice(RitsuLibSidecarEnvelopeLayout.PayloadLengthOffset,
                    RitsuLibSidecarEnvelopeLayout.PayloadLengthSize),
                (uint)wirePayload.Length);
            BinaryPrimitives.WriteUInt32BigEndian(
                span.Slice(RitsuLibSidecarEnvelopeLayout.ExtensionLengthOffset,
                    RitsuLibSidecarEnvelopeLayout.ExtensionLengthSize),
                (uint)headerExtension.Length);

            var extensionOffset = RitsuLibSidecarEnvelopeLayout.FixedHeaderSize;
            headerExtension.CopyTo(span.Slice(extensionOffset, headerExtension.Length));
            var payloadWriteOffset = extensionOffset + headerExtension.Length;
            wirePayload.CopyTo(span[payloadWriteOffset..]);
            return buffer;
        }

        /// <summary>
        ///     Decoded header fields and logical payload.
        ///     Decoded header fields 和 logical payload.
        /// </summary>
        public readonly struct ParsedEnvelope
        {
            /// <summary>
            ///     Creates a parsed envelope value.
            ///     创建 a parsed envelope value。
            /// </summary>
            /// <param name="wireFormatVersion">
            ///     Wire version from the packet.
            ///     Wire version 从 the packet.
            /// </param>
            /// <param name="flags">
            ///     Decoded wire flags.
            ///     中文说明：Decoded wire flags.
            /// </param>
            /// <param name="opcode">
            ///     64-bit opcode from the packet.
            ///     64-bit opcode 从 the packet.
            /// </param>
            /// <param name="headerExtension">
            ///     Optional extension segment.
            ///     可选 extension segment.
            /// </param>
            /// <param name="payload">
            ///     Logical payload (decompressed if gzip was set).
            ///     Logical payload (decompressed 如果 gzip was 设置).
            /// </param>
            public ParsedEnvelope(
                ushort wireFormatVersion,
                RitsuLibSidecarWireFlags flags,
                ulong opcode,
                ReadOnlyMemory<byte> headerExtension,
                ReadOnlyMemory<byte> payload)
            {
                WireFormatVersion = wireFormatVersion;
                Flags = flags;
                Opcode = opcode;
                HeaderExtension = headerExtension;
                Payload = payload;
            }

            /// <summary>
            ///     Wire format version from the packet.
            ///     Wire 用于mat version 从 the packet.
            /// </summary>
            public ushort WireFormatVersion { get; }

            /// <summary>
            ///     Decoded flags (unknown bits cleared).
            ///     中文说明：Decoded flags (unknown bits cleared).
            /// </summary>
            public RitsuLibSidecarWireFlags Flags { get; }

            /// <summary>
            ///     64-bit opcode (from <see cref="RitsuLibSidecarOpcodes.For" /> or a framework constant).
            ///     64-bit opcode (从 <c>RitsuLibSidecarOpcodes.For</c> 或 a framework constant).
            /// </summary>
            public ulong Opcode { get; }

            /// <summary>
            ///     Opaque header extension; v1 senders typically use length 0.
            ///     Opaque header extension; v1 senders typically 使用 length 0.
            /// </summary>
            public ReadOnlyMemory<byte> HeaderExtension { get; }

            /// <summary>
            ///     Logical payload (after optional gzip decompression).
            ///     Logical payload (之后 可选 gzip decompression).
            /// </summary>
            public ReadOnlyMemory<byte> Payload { get; }
        }
    }
}
