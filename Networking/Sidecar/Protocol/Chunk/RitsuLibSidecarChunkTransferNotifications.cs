namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Optional notifications for chunked sidecar transfers (receive path). Subscribe for UI such as image
    ///     可选 notifications 用于 chunked sidecar transfers (receive 路径). Subscribe 用于 UI such as image
    ///     download progress; keep handlers short and avoid blocking the multiplayer receive thread unless using
    ///     down加载 progress; keep handlers short 和 avoid blocking the multiplayer receive thread unless using
    ///     <see cref="RitsuLibSidecarGodotMainLoopScheduling.TryPostToMainLoop" />.
    /// </summary>
    public static class RitsuLibSidecarChunkTransferNotifications
    {
        /// <summary>
        ///     Raised after a new segment is accepted or when reassembly completes.
        ///     Raised 之后 a new segment is accepted 或 当 reassembly completes.
        /// </summary>
        public static event Action<RitsuLibSidecarChunkReceiveProgress>? ReceiveProgress;

        internal static void RaiseReceive(in RitsuLibSidecarChunkReceiveProgress progress)
        {
            ReceiveProgress?.Invoke(progress);
        }
    }
}
