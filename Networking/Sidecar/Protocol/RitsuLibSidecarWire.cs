using System.Buffers.Binary;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Wire-level constants for the RitsuLib multiplayer sidecar envelope. Values are fixed; nothing is derived
    ///     Wire-level constants 用于 the RitsuLib multiplayer sidecar envelope. Values are fixed; nothing is derived
    ///     from reflection or sorted type lists.
    ///     从 reflection 或 sorted type lists.
    /// </summary>
    public static class RitsuLibSidecarWire
    {
        /// <summary>
        ///     ENet / Steam channel for reliable sidecar traffic. Placed high to reduce overlap with other mods that
        ///     ENet / Steam channel 用于 reliable sidecar traffic. Placed high to reduce overlap 带有 other mods that
        ///     pick low spare channels; vanilla 0.104.0 uses 0 and 1 only.
        ///     pick low spare channels; 原版 0.104.0 使用 0 和 1 only.
        /// </summary>
        public const int RecommendedReliableChannel = 48;

        /// <summary>
        ///     ENet channel for best-effort sidecar traffic.
        ///     ENet channel 用于 best-effort sidecar traffic.
        /// </summary>
        public const int RecommendedUnreliableChannel = 49;

        /// <summary>
        ///     Wire format version written by <see cref="RitsuLibSidecar.CreateEnvelope" />.
        ///     Wire 用于mat version written 通过 <c>RitsuLibSidecar.创建Envelope</c>.
        /// </summary>
        public const ushort CurrentWireFormatVersion = 1;

        /// <summary>
        ///     Highest wire format this library accepts.
        ///     Highest wire 用于mat this library accepts.
        /// </summary>
        public const ushort SupportedWireFormatVersionMax = 1;

        /// <summary>
        ///     Maximum logical payload size (after gzip decompress).
        ///     最大 logical payload size (after gzip decompress)。
        /// </summary>
        public const uint MaxPayloadBytes = 4 * RitsuLibSidecarBinaryLayout.MiB;

        /// <summary>
        ///     Maximum header extension segment length (generous margin for future header TLVs).
        ///     最大 header extension segment length (generous margin for future header TLVs)。
        /// </summary>
        public const uint MaxHeaderExtensionBytes = 64 * RitsuLibSidecarBinaryLayout.KiB;

        /// <summary>
        ///     Length of <see cref="Magic" />.
        ///     中文说明：Length of <c>Magic</c>.
        /// </summary>
        public static int MagicLength => Magic.Length;

        /// <summary>
        ///     Minimum on-wire size: magic + wire version + flags + opcode + payload length + extension length (no
        ///     中文说明：Minimum on-wire size: magic + wire version + flags + opcode + payload length + extension length (no
        ///     Minimum on-wire size: magic + wire version + flags + opcode + payload length + extension length (no
        ///     中文说明：Minimum on-wire size: magic + wire version + flags + opcode + payload length + extension length (no
        ///     extension bytes, no payload).
        ///     中文说明：extension bytes, no payload).
        ///     extension bytes, no payload).
        ///     中文说明：extension bytes, no payload).
        /// </summary>
        public static int MinEnvelopeSize => MagicLength +
                                             RitsuLibSidecarBinaryLayout.U16Size +
                                             RitsuLibSidecarBinaryLayout.U32Size +
                                             RitsuLibSidecarBinaryLayout.U64Size +
                                             RitsuLibSidecarBinaryLayout.U32Size +
                                             RitsuLibSidecarBinaryLayout.U32Size;

        /// <summary>
        ///     Packet prefix; <c>"STS2RitsuLib"u8</c>.
        ///     中文说明：Packet prefix; <c>"STS2RitsuLib"u8</c>.
        /// </summary>
        public static ReadOnlySpan<byte> Magic => "STS2RitsuLib"u8;

        /// <summary>
        ///     Returns true when <paramref name="packet" /> begins with <see cref="Magic" />.
        ///     返回 true when <c>packet</c> begins with <c>Magic</c>。
        /// </summary>
        public static bool MatchesMagic(ReadOnlySpan<byte> packet)
        {
            return packet.Length >= MagicLength && packet[..MagicLength].SequenceEqual(Magic);
        }

        /// <summary>
        ///     Reads the 64-bit opcode from a sidecar envelope prefix when <see cref="MatchesMagic" /> holds and the
        ///     Reads the 64-bit opcode 从 a sidecar envelope prefix 当 <c>MatchesMagic</c> holds 和 the
        ///     span is long enough; does not validate the full envelope.
        ///     span is long enough; does not 有效ate the full envelope.
        /// </summary>
        public static bool TryPeekOpcode(ReadOnlySpan<byte> packet, out ulong opcode)
        {
            opcode = 0;
            if (packet.Length < MagicLength +
                RitsuLibSidecarBinaryLayout.U16Size +
                RitsuLibSidecarBinaryLayout.U32Size +
                RitsuLibSidecarBinaryLayout.U64Size || !MatchesMagic(packet))
                return false;

            opcode = BinaryPrimitives.ReadUInt64BigEndian(
                packet.Slice(RitsuLibSidecarEnvelopeLayout.OpcodeOffset, RitsuLibSidecarEnvelopeLayout.OpcodeSize));
            return true;
        }
    }
}
