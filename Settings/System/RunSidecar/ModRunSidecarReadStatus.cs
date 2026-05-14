namespace STS2RitsuLib.Settings.RunSidecar
{
    /// <summary>
    ///     Result of validating and reading a run sidecar JSON file.
    ///     Result of 有效ating 和 reading a 跑局 sidecar JSON file.
    /// </summary>
    public enum ModRunSidecarReadStatus
    {
        /// <summary>
        ///     Model loaded and fingerprint matched the active run.
        ///     模型 loaded 和 fingerprint matched the active 跑局.
        /// </summary>
        Ok,

        /// <summary>
        ///     No run in progress (main menu / post-run).
        ///     No 跑局 in progress (main menu / post-跑局).
        /// </summary>
        NoActiveRun,

        /// <summary>
        ///     Sidecar file does not exist yet.
        ///     中文说明：Sidecar file does not exist yet.
        /// </summary>
        MissingFile,

        /// <summary>
        ///     JSON could not be parsed.
        ///     中文说明：JSON could not be parsed.
        /// </summary>
        InvalidJson,

        /// <summary>
        ///     Stored fingerprint does not match the active run — data is ignored to avoid wrong-run binding.
        ///     Stored fingerprint does not match the active 跑局 — data is ignored to avoid wrong-跑局 binding.
        /// </summary>
        FingerprintMismatch,
    }
}
