namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Process-wide best-effort counters for sidecar traffic observed in RitsuLib hooks. Does not include vanilla
    ///     Process-wide best-effort counters 用于 sidecar traffic observed in RitsuLib hooks. Does not include 原版
    ///     <c>NetMessageBus</c> traffic or transport queue depth (not exposed here).
    /// </summary>
    public static class RitsuLibSidecarTrafficCounters
    {
        private static long _incomingPackets;
        private static long _incomingWireBytes;
        private static long _incomingLogicalPayloadBytes;
        private static long _outgoingSendOperations;
        private static long _outgoingWireBytes;

        /// <summary>
        ///     Inbound sidecar envelopes that passed parse and reached <see cref="RitsuLibSidecarBus.Dispatch" />.
        ///     Inbound sidecar envelopes that passed parse 和 reached <c>RitsuLibSidecarBus.Dispatch</c>.
        /// </summary>
        public static long IncomingPackets => Interlocked.Read(ref _incomingPackets);

        /// <summary>
        ///     Sum of full on-wire packet lengths for <see cref="IncomingPackets" />.
        ///     Sum of full on-wire packet lengths 用于 <c>IncomingPackets</c>.
        /// </summary>
        public static long IncomingWireBytes => Interlocked.Read(ref _incomingWireBytes);

        /// <summary>
        ///     Sum of logical payload lengths (after gzip when applicable) for <see cref="IncomingPackets" />.
        ///     Sum of logical payload lengths (之后 gzip 当 applicable) 用于 <c>IncomingPackets</c>.
        /// </summary>
        public static long IncomingLogicalPayloadBytes => Interlocked.Read(ref _incomingLogicalPayloadBytes);

        /// <summary>
        ///     Outbound send operations: one per client in a host broadcast, otherwise one per
        ///     中文说明：Outbound send operations: one per client in a host broadcast, otherwise one per
        ///     <see cref="RitsuLibSidecarSend" /> call that returned <c>true</c>.
        /// </summary>
        public static long OutgoingSendOperations => Interlocked.Read(ref _outgoingSendOperations);

        /// <summary>
        ///     Sum of envelope lengths passed to the vanilla send API for <see cref="OutgoingSendOperations" />.
        ///     Sum of envelope lengths passed to the 原版 send API 用于 <c>OutgoingSendOperations</c>.
        /// </summary>
        public static long OutgoingWireBytes => Interlocked.Read(ref _outgoingWireBytes);

        /// <summary>
        ///     Sets all counters to zero (e.g. diagnostics or tests).
        ///     设置 all counters to zero (e.g. diagnostics 或 tests).
        /// </summary>
        public static void Reset()
        {
            Interlocked.Exchange(ref _incomingPackets, 0);
            Interlocked.Exchange(ref _incomingWireBytes, 0);
            Interlocked.Exchange(ref _incomingLogicalPayloadBytes, 0);
            Interlocked.Exchange(ref _outgoingSendOperations, 0);
            Interlocked.Exchange(ref _outgoingWireBytes, 0);
        }

        internal static void AddIncoming(int wireLen, int logicalPayloadLen)
        {
            Interlocked.Increment(ref _incomingPackets);
            Interlocked.Add(ref _incomingWireBytes, wireLen);
            Interlocked.Add(ref _incomingLogicalPayloadBytes, logicalPayloadLen);
        }

        internal static void AddOutgoing(int operations, long wireBytes)
        {
            Interlocked.Add(ref _outgoingSendOperations, operations);
            Interlocked.Add(ref _outgoingWireBytes, wireBytes);
        }
    }
}
