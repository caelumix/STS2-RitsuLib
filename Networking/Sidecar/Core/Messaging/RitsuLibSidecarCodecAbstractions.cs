using System.Buffers;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Binary encode/decode for one opcode; implement one concrete type T.
    ///     Binary encode/decode 用于 one opcode; implement one concrete type T.
    /// </summary>
    public interface IRitsuLibSidecarMessageCodec<T>
        where T : notnull
    {
        /// <summary>
        ///     User or control <c>ulong</c> opcode; must match <see cref="RitsuLibSidecarBus" /> registration.
        ///     使用r 或 control <c>ulong</c> opcode; must match <c>RitsuLibSidecarBus</c> 注册.
        /// </summary>
        ulong Opcode { get; }

        /// <summary>
        ///     Decodes the sidecar logical payload (no outer magic; that is stripped by the bus).
        ///     Decodes the sidecar logical payload (no outer magic; that is stripped 通过 the bus).
        /// </summary>
        /// <param name="input">
        ///     Bytes after the fixed envelope header and optional extension.
        ///     Bytes 之后 the fixed envelope header 和 可选 extension.
        /// </param>
        /// <param name="message">
        ///     Set when the return value is <c>true</c>.
        ///     设置 当 the 返回 value is <c>true</c>.
        /// </param>
        bool TryDecode(ReadOnlySpan<byte> input, out T? message);

        /// <summary>
        ///     Appends the wire form of <paramref name="message" /> to <paramref name="writer" />.
        ///     Appends the wire 用于m of <c>message</c> to <c>writer</c>.
        /// </summary>
        /// <param name="writer">
        ///     Destination buffer writer.
        ///     目标 buffer writer。
        /// </param>
        /// <param name="message">
        ///     Value to encode.
        ///     中文说明：Value to encode.
        /// </param>
        void Encode(IBufferWriter<byte> writer, T message);
    }

    /// <summary>
    ///     Apply a decoded value after <see cref="IRitsuLibSidecarMessageCodec{T}.TryDecode" />; thread matches the
    ///     Apply a decoded value 之后 <c>IRitsuLibSidecarMessageCodec{T}.TryDecode</c>; thread matches the
    ///     sidecar receive path unless you register with
    ///     sidecar receive 路径 unless you register 带有
    ///     <see cref="RitsuLibSidecarMessageBinding.RegisterForGodotMainLoop{T}" />.
    /// </summary>
    public interface IRitsuLibSidecarSyncProcessor<in T>
        where T : notnull
    {
        /// <param name="message">
        ///     Value from <see cref="IRitsuLibSidecarMessageCodec{T}.TryDecode" />.
        ///     Value 从 <c>IRitsuLibSidecarMessageCodec{T}.TryDecode</c>.
        /// </param>
        /// <param name="context">
        ///     Per-packet transport and envelope information.
        ///     Per-packet transport 和 envelope information.
        /// </param>
        void Apply(T message, in RitsuLibSidecarDispatchContext context);
    }
}
