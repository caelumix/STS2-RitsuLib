namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Registers a codec and processor on <see cref="RitsuLibSidecarBus" /> in one call.
    ///     注册 a codec and processor on <c>RitsuLibSidecarBus</c> in one call。
    /// </summary>
    public static class RitsuLibSidecarMessageBinding
    {
        /// <summary>
        ///     Subscribes <paramref name="processor" /> for <see cref="IRitsuLibSidecarMessageCodec{T}.Opcode" />. The
        ///     Subscribes <c>processor</c> 用于 <c>IRitsuLibSidecarMessageCodec{T}.Opcode</c>. The
        ///     handler runs on the same thread as <see cref="RitsuLibSidecarReceivePipeline.ShouldSuppressVanillaDeserialize" />
        ///     handler runs on the same thread as <c>RitsuLibSidecarReceivePipeline.ShouldSuppress原版Deserialize</c>
        ///     (vanilla multiplayer receive path), which is not guaranteed to be the Godot main thread. Send with
        ///     (原版 multiplayer receive 路径), which is not guaranteed to be the Godot main thread. Send 带有
        ///     <see cref="RitsuLibSidecar.CreateEnvelopeWithDelivery" /> or <see cref="RitsuLibSidecarHighLevelSend" /> to
        ///     record delivery semantics in the header extension.
        ///     中文说明：record delivery semantics in the header extension.
        /// </summary>
        /// <param name="codec">
        ///     Encodes and decodes the payload for this opcode.
        ///     Encodes 和 decodes the payload 用于 this opcode.
        /// </param>
        /// <param name="processor">
        ///     Applies decoded messages.
        ///     中文说明：Applies decoded messages.
        /// </param>
        public static void Register<T>(
            IRitsuLibSidecarMessageCodec<T> codec,
            IRitsuLibSidecarSyncProcessor<T> processor)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(codec);
            ArgumentNullException.ThrowIfNull(processor);
            RitsuLibSidecarProtocol.EnsureDefaultHandlers();
            RitsuLibSidecarBus.RegisterHandler(
                codec.Opcode,
                ctx =>
                {
                    if (!codec.TryDecode(ctx.Payload.Span, out var m) || m is null)
                        return;

                    processor.Apply(m, in ctx);
                });
        }

        /// <summary>
        ///     Like <see cref="Register{T}" />, but copies envelope bytes then decodes and calls
        ///     Like <c>Register{T}</c>, but copies envelope bytes then decodes 和 calls
        ///     <paramref name="processor" /> on the Godot main loop when
        ///     <see cref="RitsuLibSidecarGodotMainLoopScheduling.TryPostToMainLoop" />
        ///     succeeds; otherwise falls back to the receive thread (same as <see cref="Register{T}" />).
        ///     中文说明：succeeds; otherwise falls back to the receive thread (same as <c>Register{T}</c>).
        /// </summary>
        public static void RegisterForGodotMainLoop<T>(
            IRitsuLibSidecarMessageCodec<T> codec,
            IRitsuLibSidecarSyncProcessor<T> processor)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(codec);
            ArgumentNullException.ThrowIfNull(processor);
            RitsuLibSidecarProtocol.EnsureDefaultHandlers();
            RitsuLibSidecarBus.RegisterHandler(
                codec.Opcode,
                ctx =>
                {
                    var owned = ctx.WithOwnedEnvelopeMemory();

                    if (!RitsuLibSidecarGodotMainLoopScheduling.TryPostToMainLoop(ApplyOnLoop))
                        ApplyOnLoop();
                    return;

                    void ApplyOnLoop()
                    {
                        if (!codec.TryDecode(owned.Payload.Span, out var m) || m is null)
                            return;

                        processor.Apply(m, in owned);
                    }
                });
        }
    }
}
