namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Installs default sidecar control handlers (handshake, chunked reassembly) once. Idempotent. Called from
    ///     Installs default sidecar control handlers (handshake, chunked reassembly) once. Idempotent. Called 从
    ///     receive and from <see cref="RitsuLibSidecarHighLevelSend" />.
    ///     receive 和 从 <c>RitsuLibSidecarHighLevelSend</c>.
    /// </summary>
    public static class RitsuLibSidecarProtocol
    {
        private static int _registered;

        /// <summary>
        ///     Registers built-in handlers for control opcodes and chunked reassembly once per process. Safe to call
        ///     Registers built-in handlers 用于 control opcodes 和 chunked reassembly once per process. Safe to call
        ///     from send/receive paths; subsequent calls are no-ops.
        ///     从 send/receive 路径; subsequent calls are no-ops.
        /// </summary>
        public static void EnsureDefaultHandlers()
        {
            if (Interlocked.CompareExchange(ref _registered, 1, 0) != 0)
                return;

            RitsuLibSidecarSessionManager.EnsureProvidersBootstrapped();
            RitsuLibSidecarBuiltInHandlers.Register();
            RitsuLibSidecarNetworkingLifecycle.EnsureHooksInstalled();
            RitsuLibSidecarRequiredCapabilities.RegisterRequiredCapability(
                "ritsulib:sidecar_core_supported",
                RitsuLibSidecarSessionManager.CanSendToPeer);
        }
    }
}
