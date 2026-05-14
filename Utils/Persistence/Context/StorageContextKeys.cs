namespace STS2RitsuLib.Utils.Persistence.Context
{
    /// <summary>
    ///     Built-in <see cref="StorageContextKey{TValue}" /> values used by RitsuLib persistence.
    ///     Built-in <c>StorageContextKey{TValue}</c> values used 通过 RitsuLib persistence.
    /// </summary>
    public static class StorageContextKeys
    {
        /// <summary>
        ///     Overrides the active game profile id for a persistence operation.
        ///     Overrides the active game 档案 id 用于 a persistence operation.
        /// </summary>
        public static StorageContextKey<int> ProfileId { get; } = new("sts2ritsulib.profileId");

        /// <summary>
        ///     Stable per-run fingerprint stem used by run sidecar storage.
        ///     稳定的 per-run fingerprint stem used by run sidecar storage。
        /// </summary>
        /// <remarks>
        ///     This is expected to match <c>ModRunSidecarFingerprint.ComputeFileStem()</c> output.
        ///     This is expected to match <c>ModRunSidecarFingerprint.ComputeFileStem()</c> output.
        /// </remarks>
        public static StorageContextKey<string> RunFingerprintStem { get; } = new("sts2ritsulib.runFingerprintStem");
    }
}
