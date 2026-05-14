namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Optional behavior for ModConfig mirror registration.
    ///     可选 behavior 用于 ModConfig mirror 注册.
    /// </summary>
    public sealed class ModConfigMirrorRegistrationOptions
    {
        /// <summary>
        ///     Default options used when none are passed.
        ///     默认 options used when none are passed。
        /// </summary>
        public static ModConfigMirrorRegistrationOptions Default { get; } = new();

        /// <summary>
        ///     Forwarded to mirrored key binding entries.
        ///     中文说明：Forwarded to mirrored key binding entries.
        /// </summary>
        public bool KeyBindAllowModifierCombos { get; init; } = false;

        /// <summary>
        ///     Forwarded to mirrored key binding entries.
        ///     中文说明：Forwarded to mirrored key binding entries.
        /// </summary>
        public bool KeyBindAllowModifierOnly { get; init; } = false;

        /// <summary>
        ///     Forwarded to mirrored key binding entries.
        ///     中文说明：Forwarded to mirrored key binding entries.
        /// </summary>
        public bool KeyBindDistinguishModifierSides { get; init; } = false;
    }
}
