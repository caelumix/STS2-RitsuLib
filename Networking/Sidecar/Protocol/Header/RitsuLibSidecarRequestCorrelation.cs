using System.Buffers;
using System.Buffers.Binary;
using MegaCrit.Sts2.Core.Runs;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Optional 8-byte big-endian correlation id in the header extension immediately after the 1-byte delivery tag
    ///     可选 8-byte big-endian correlation id in the header extension immediately 之后 the 1-byte delivery tag
    ///     from <see cref="RitsuLibSidecar.CreateEnvelopeWithDelivery" /> (layout: delivery, correlation × 8, optional
    ///     从 <c>RitsuLibSidecar.创建EnvelopeWithDelivery</c> (layout: delivery, correlation × 8, 可选
    ///     tail).
    ///     中文说明：tail).
    /// </summary>
    public static class RitsuLibSidecarRequestCorrelation
    {
        /// <summary>
        ///     Size of the correlation id in the extension (after the delivery byte).
        ///     Size of the correlation id in the extension (之后 the delivery byte).
        /// </summary>
        public const int BigEndianU64Bytes = RitsuLibSidecarBinaryLayout.U64Size;

        /// <summary>
        ///     Minimum full <see cref="RitsuLibSidecarEnvelope.ParsedEnvelope.HeaderExtension" /> length to read a
        ///     中文说明：Minimum full <c>RitsuLibSidecarEnvelope.ParsedEnvelope.HeaderExtension</c> length to read a
        ///     correlation.
        ///     中文说明：correlation.
        /// </summary>
        public const int MinHeaderExtensionBytesWithCorrelation =
            RitsuLibSidecarBinaryLayout.ByteSize + BigEndianU64Bytes;

        private static long _nextCorrelation;

        /// <summary>
        ///     Allocates a monotonically increasing correlation value for request/reply matching.
        ///     Allocates a monotonically increasing correlation value 用于 request/reply matching.
        /// </summary>
        public static ulong AllocateCorrelationId()
        {
            return (ulong)Interlocked.Increment(ref _nextCorrelation);
        }

        /// <summary>
        ///     Writes <paramref name="correlationId" /> big-endian into the first 8 bytes of <paramref name="destination" />.
        ///     写入 <c>correlationId</c> big-endian into the first 8 bytes of <c>destination</c>。
        /// </summary>
        public static void WriteCorrelationBigEndian(Span<byte> destination, ulong correlationId)
        {
            BinaryPrimitives.WriteUInt64BigEndian(destination, correlationId);
        }

        /// <summary>
        ///     Builds <c>additionalHeaderExtension</c> for
        ///     Builds <c>additionalHeaderExtension</c> 用于
        ///     <see
        ///         cref="RitsuLibSidecarHighLevelSend.TrySendAsClient(RunManager?,ulong,System.ReadOnlySpan{byte},RitsuLibSidecarDeliverySemantics,RitsuLibSidecarWireFlags,bool,System.ReadOnlySpan{byte})" />
        ///     cref="RitsuLibSidecarHighLevelSend.TrySendAsClient(跑局Manager?,ulong,System.ReadOnlySpan{byte},RitsuLibSidecarDeliverySemantics,RitsuLibSidecarWireFlags,bool,System.ReadOnlySpan{byte})"
        ///     />
        ///     :
        ///     中文说明：:
        ///     correlation (8 BE) then <paramref name="tailAfterCorrelation" />.
        ///     correlation (8 BE) then <c>tail之后Correlation</c>.
        /// </summary>
        public static byte[] PackAdditional(ulong correlationId, ReadOnlySpan<byte> tailAfterCorrelation = default)
        {
            var buf = new byte[BigEndianU64Bytes + tailAfterCorrelation.Length];
            WriteCorrelationBigEndian(buf.AsSpan(0, BigEndianU64Bytes), correlationId);
            tailAfterCorrelation.CopyTo(buf.AsSpan(BigEndianU64Bytes));
            return buf;
        }

        /// <summary>
        ///     Appends correlation and tail to <paramref name="writer" />.
        ///     Appends correlation 和 tail to <c>writer</c>.
        /// </summary>
        public static void PackAdditionalTo(ulong correlationId, ReadOnlySpan<byte> tailAfterCorrelation,
            IBufferWriter<byte> writer)
        {
            var span = writer.GetSpan(BigEndianU64Bytes + tailAfterCorrelation.Length);
            WriteCorrelationBigEndian(span, correlationId);
            tailAfterCorrelation.CopyTo(span[BigEndianU64Bytes..]);
            writer.Advance(BigEndianU64Bytes + tailAfterCorrelation.Length);
        }

        /// <summary>
        ///     Reads the correlation from a full header extension (delivery byte first).
        ///     Reads the correlation 从 a full header extension (delivery byte first).
        /// </summary>
        public static bool TryReadCorrelation(ReadOnlyMemory<byte> fullHeaderExtension, out ulong correlationId)
        {
            correlationId = 0;
            if (fullHeaderExtension.Length < MinHeaderExtensionBytesWithCorrelation)
                return false;

            correlationId = BinaryPrimitives.ReadUInt64BigEndian(
                fullHeaderExtension.Span.Slice(
                    RitsuLibSidecarEnvelopeLayout.CorrelationOffsetInExtension,
                    BigEndianU64Bytes));
            return true;
        }

        /// <summary>
        ///     True when a correlation is present and equals <paramref name="expected" />.
        ///     当 a correlation is present and equals <c>expected</c> 时为 true。
        /// </summary>
        public static bool HeaderExtensionCorrelationEquals(ReadOnlyMemory<byte> fullHeaderExtension, ulong expected)
        {
            return TryReadCorrelation(fullHeaderExtension, out var c) && c == expected;
        }
    }
}
