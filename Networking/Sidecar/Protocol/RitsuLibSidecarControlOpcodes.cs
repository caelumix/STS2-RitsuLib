namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Reserved fixed opcodes for framework control-plane messages (0…
    ///     Reserved fixed opcodes 用于 framework control-plane messages (0…
    ///     <see cref="RitsuLibSidecarOpcodes.FixedProtocolOpcodeMaxInclusive" />).
    ///     User <see cref="RitsuLibSidecarOpcodes.For" /> opcodes are always above that range.
    ///     使用r <c>RitsuLibSidecarOpcodes.For</c> opcodes are always above that range.
    /// </summary>
    public static class RitsuLibSidecarControlOpcodes
    {
        /// <summary>
        ///     Optional framework keep-alive or latency probe (reserved).
        ///     可选 framework keep-alive 或 latency probe (reserved).
        /// </summary>
        public const ulong FrameworkPing = RitsuLibSidecarControlOpcodeLayout.FrameworkPing;

        /// <summary>
        ///     Capability advertisement and version negotiation (payload: <see cref="RitsuLibSidecarHandshakeBinary" />).
        ///     Capability advertisement 和 version negotiation (payload: <c>RitsuLibSidecarHandshakeBinary</c>).
        /// </summary>
        public const ulong Handshake =
            RitsuLibSidecarControlOpcodeLayout.ControlRangeStart + RitsuLibSidecarControlOpcodeLayout.HandshakeOffset;

        /// <summary>
        ///     Response to <see cref="Handshake" /> (payload: <see cref="RitsuLibSidecarHandshakeBinary" /> ack layout).
        ///     中文说明：Response to <c>Handshake</c> (payload: <c>RitsuLibSidecarHandshakeBinary</c> ack layout).
        ///     Response to <c>Handshake</c> (payload: <c>RitsuLibSidecarHandshakeBinary</c> ack layout).
        ///     中文说明：Response to <c>Handshake</c> (payload: <c>RitsuLibSidecarHandshakeBinary</c> ack layout).
        /// </summary>
        public const ulong HandshakeAck = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                          RitsuLibSidecarControlOpcodeLayout.HandshakeAckOffset;

        /// <summary>
        ///     One chunk of a large logical payload; see chunked stream reassembly.
        ///     中文说明：One chunk of a large logical payload; see chunked stream reassembly.
        ///     One chunk of a large logical payload; see chunked stream reassembly.
        ///     中文说明：One chunk of a large logical payload; see chunked stream reassembly.
        /// </summary>
        public const ulong ChunkedFrame = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                          RitsuLibSidecarControlOpcodeLayout.ChunkedFrameOffset;

        /// <summary>
        ///     Receiver → original chunk sender: missing part ranges (SACK / selective gap report; variable-length
        ///     中文说明：Receiver → original chunk sender: missing part ranges (SACK / selective gap report; variable-length
        ///     range list).
        ///     中文说明：range list).
        /// </summary>
        public const ulong ChunkStreamSelectiveNack = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                                      RitsuLibSidecarControlOpcodeLayout.ChunkStreamSelectiveNackOffset;

        /// <summary>
        ///     Receiver → original chunk sender: reassembly completed; sender may drop outbound buffers (8-byte
        ///     中文说明：Receiver → original chunk sender: reassembly completed; sender may drop outbound buffers (8-byte
        ///     <c>streamId</c>).
        /// </summary>
        public const ulong ChunkStreamReassemblyDone = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                                       RitsuLibSidecarControlOpcodeLayout
                                                           .ChunkStreamReassemblyDoneOffset;

        /// <summary>
        ///     Client → host: request a coordinated combat-state dump across all peers (host fans out
        ///     中文说明：Client → host: request a coordinated combat-state dump across all peers (host fans out
        ///     <see cref="DiagnosticRelayDumpFanout" />).
        /// </summary>
        public const ulong DiagnosticRelayDumpRequest = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                                        RitsuLibSidecarControlOpcodeLayout
                                                            .DiagnosticRelayDumpRequestOffset;

        /// <summary>
        ///     Host → everyone: carry <see cref="RitsuLibSidecarDiagnosticPayload" /> so each peer logs local state.
        ///     中文说明：Host → everyone: carry <c>RitsuLibSidecarDiagnosticPayload</c> so each peer logs local state.
        ///     Host → everyone: carry <c>RitsuLibSidecarDiagnosticPayload</c> so each peer logs local state.
        ///     中文说明：Host → everyone: carry <c>RitsuLibSidecarDiagnosticPayload</c> so each peer logs local state.
        /// </summary>
        public const ulong DiagnosticRelayDumpFanout = RitsuLibSidecarControlOpcodeLayout.ControlRangeStart +
                                                       RitsuLibSidecarControlOpcodeLayout
                                                           .DiagnosticRelayDumpFanoutOffset;
    }
}
