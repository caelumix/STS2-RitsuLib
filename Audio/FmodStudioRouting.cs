namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Studio bus paths used by the game's AudioManagerProxy (FMOD Studio). Use with <see cref="FmodStudioServer" /> for
    ///     Studio bus 路径 used 通过 the game's AudioManagerProxy (FMOD Studio). 使用 带有 <c>FmodStudioServer</c> 用于
    ///     direct bus access.
    ///     中文说明：direct bus access.
    /// </summary>
    public static class FmodStudioRouting
    {
        /// <summary>
        ///     Root master bus path.
        ///     Root master bus 路径.
        /// </summary>
        public const string MasterBus = "bus:/master";

        /// <summary>
        ///     Game SFX bus under master.
        ///     中文说明：Game SFX bus under master.
        /// </summary>
        public const string SfxBus = "bus:/master/sfx";

        /// <summary>
        ///     Ambience bus under master.
        ///     中文说明：Ambience bus under master.
        /// </summary>
        public const string AmbienceBus = "bus:/master/ambience";

        /// <summary>
        ///     Music / BGM bus under master.
        ///     中文说明：Music / BGM bus under master.
        /// </summary>
        public const string MusicBus = "bus:/master/music";
    }
}
