namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Values for FMOD Studio load_bank via the Godot FMOD addon.
    ///     Values 用于 FMOD Studio 加载_bank via the Godot FMOD addon.
    /// </summary>
    public enum FmodStudioLoadBankMode
    {
        /// <summary>
        ///     Default blocking load.
        ///     默认 blocking load。
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Load without blocking the caller.
        ///     加载 without blocking the caller。
        /// </summary>
        NonBlocking = 1,

        /// <summary>
        ///     Decompress sample data into memory (Studio load flag).
        ///     Decompress sample data into memory (Studio 加载 flag).
        /// </summary>
        DecompressSamples = 2,
    }
}
