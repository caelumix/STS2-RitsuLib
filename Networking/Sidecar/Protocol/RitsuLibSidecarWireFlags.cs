namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Sidecar envelope flags (uint32, big-endian on wire). Unknown bits are cleared on read for forward
    ///     Sidecar envelope flags (uint32, big-endian on wire). Unknown bits are cleared on read 用于 用于ward
    ///     compatibility.
    ///     中文说明：compatibility.
    /// </summary>
    [Flags]
    public enum RitsuLibSidecarWireFlags : uint
    {
        /// <summary>
        ///     No flags.
        ///     中文说明：No flags.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Payload bytes are gzip-compressed (RFC 1952); handlers see decompressed bytes.
        ///     中文说明：Payload bytes are gzip-compressed (RFC 1952); handlers see decompressed bytes.
        ///     Payload bytes are gzip-compressed (RFC 1952); handlers see decompressed bytes.
        ///     中文说明：Payload bytes are gzip-compressed (RFC 1952); handlers see decompressed bytes.
        /// </summary>
        PayloadGzip = 1u << 0,
    }
}
