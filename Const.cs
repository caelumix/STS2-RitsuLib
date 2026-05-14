namespace STS2RitsuLib
{
    /// <summary>
    ///     Stable identifiers and version constants for the RitsuLib mod assembly.
    ///     稳定的 identifiers and version constants for the RitsuLib mod assembly。
    /// </summary>
    public static class Const
    {
        /// <summary>
        ///     Human-readable mod name.
        ///     人类可读的 mod name。
        /// </summary>
        public const string Name = "RitsuLib";

        /// <summary>
        ///     Unique mod id used by the game and persistence.
        ///     Unique mod id used 通过 the game 和 persistence.
        /// </summary>
        public const string ModId = "com.ritsukage.sts2-RitsuLib";

        /// <summary>
        ///     Assembly / manifest version string.
        ///     中文说明：Assembly / manifest version string.
        /// </summary>
        public const string Version = "0.2.30";

        /// <summary>
        ///     Root key for RitsuLib JSON settings under the mod’s user folder.
        ///     Root key 用于 RitsuLib JSON 设置 under the mod’s 使用r folder.
        /// </summary>
        public const string SettingsKey = "settings";

        /// <summary>
        ///     On-disk settings file name.
        ///     On-disk 设置 file name.
        /// </summary>
        public const string SettingsFileName = "settings.json";

        /// <summary>
        ///     Subdirectory under global mod data for shell theme JSON (next to <see cref="SettingsFileName" />).
        ///     Subdirectory under global mod data 用于 shell theme JSON (next to <c>设置FileName</c>).
        /// </summary>
        public const string ShellThemesDirectoryName = "shell_themes";

        /// <summary>
        ///     BaseLib main Harmony instance id.
        ///     中文说明：BaseLib main Harmony instance id.
        /// </summary>
        public const string BaseLibHarmonyId = "BaseLib";

        /// <summary>
        ///     Harmony id used by RitsuLib content-registry patcher.
        ///     Harmony id used 通过 RitsuLib content-注册表 patcher.
        /// </summary>
        public const string FrameworkContentRegistryHarmonyId = ModId + ".framework-content-registry";
    }
}
