namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Feature bits advertised in <see cref="RitsuLibSidecarHandshakeBinary" />.
    ///     中文说明：Feature bits advertised in <c>RitsuLibSidecarHandshakeBinary</c>.
    /// </summary>
    [Flags]
    public enum RitsuLibSidecarPeerFeatures : uint
    {
        /// <summary>
        ///     No optional features advertised.
        ///     No 可选 features advertised.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Chunked large-payload reassembly (opcode <see cref="RitsuLibSidecarControlOpcodes.ChunkedFrame" />).
        ///     中文说明：Chunked large-payload reassembly (opcode <c>RitsuLibSidecarControlOpcodes.ChunkedFrame</c>).
        ///     Chunked large-payload reassembly (opcode <c>RitsuLibSidecarControlOpcodes.ChunkedFrame</c>).
        ///     中文说明：Chunked large-payload reassembly (opcode <c>RitsuLibSidecarControlOpcodes.ChunkedFrame</c>).
        /// </summary>
        ChunkedStreams = 1 << 0,
    }
}
