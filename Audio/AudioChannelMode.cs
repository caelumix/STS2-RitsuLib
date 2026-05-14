namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     How a named channel should behave when new playback arrives.
    ///     How a named channel should behave 当 new playback arrives.
    /// </summary>
    public enum AudioChannelMode
    {
        /// <summary>
        ///     Keep the existing playback and ignore the new request.
        ///     Keep the existing playback 和 ignore the new request.
        /// </summary>
        KeepExisting = 0,

        /// <summary>
        ///     Stop the existing playback and replace it with the new one.
        ///     Stop the existing playback 和 replace it 带有 the new one.
        /// </summary>
        ReplaceExisting = 1,
    }
}
