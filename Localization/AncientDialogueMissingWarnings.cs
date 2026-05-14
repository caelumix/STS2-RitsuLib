namespace STS2RitsuLib.Localization
{
    /// <summary>
    ///     Emits at most one warning per key when ancient dialogue resolution yields nothing but the run must continue.
    ///     Emits at most one warning per key 当 ancient dialogue resolution yields nothing but the 跑局 must continue.
    /// </summary>
    internal static class AncientDialogueMissingWarnings
    {
        private static readonly Lock SyncRoot = new();
        private static readonly HashSet<string> WarnedKeys = [];

        internal static void WarnOnce(string key, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            lock (SyncRoot)
            {
                if (!WarnedKeys.Add(key))
                    return;
            }

            RitsuLibFramework.Logger.Warn(message);
        }
    }
}
