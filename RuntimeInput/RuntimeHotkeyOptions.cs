namespace STS2RitsuLib.RuntimeInput
{
    /// <summary>
    ///     Controls optional runtime hotkey router behavior for a single registration.
    ///     Controls 可选 runtime hotkey router behavior 用于 a single 注册.
    /// </summary>
    public sealed class RuntimeHotkeyOptions
    {
        /// <summary>
        ///     Stable identifier for this hotkey registration.
        ///     稳定的 identifier for this hotkey registration。
        /// </summary>
        public string? Id { get; init; }

        /// <summary>
        ///     Optional human-readable display name for UI or help surfaces.
        ///     可选 human-readable display name 用于 UI 或 help surfaces.
        /// </summary>
        public RuntimeHotkeyText? DisplayName { get; init; }

        /// <summary>
        ///     Optional human-readable description explaining what the hotkey does.
        ///     可选 human-readable description explaining what the hotkey does.
        /// </summary>
        public RuntimeHotkeyText? Description { get; init; }

        /// <summary>
        ///     Optional short semantic purpose string used for grouping or formatting.
        ///     可选 short semantic purpose string used 用于 grouping 或 用于matting.
        /// </summary>
        public string? Purpose { get; init; }

        /// <summary>
        ///     Optional UI-facing category used to group related hotkeys.
        ///     可选 UI-facing category used to group related hotkeys.
        /// </summary>
        public RuntimeHotkeyText? Category { get; init; }

        /// <summary>
        ///     When true, marks the input event as handled after the hotkey callback runs.
        ///     为 true 时，marks the input event as handled after the hotkey callback runs。
        /// </summary>
        public bool MarkInputHandled { get; init; }

        /// <summary>
        ///     When true, suppresses the hotkey while a text input control is actively editing.
        ///     为 true 时，suppresses the hotkey while a text input control is actively editing。
        /// </summary>
        public bool SuppressWhenTextInputFocused { get; init; } = true;

        /// <summary>
        ///     When true, suppresses the hotkey while the developer console is visible.
        ///     为 true 时，suppresses the hotkey while the developer console is visible。
        /// </summary>
        public bool SuppressWhenDevConsoleVisible { get; init; } = true;

        /// <summary>
        ///     Optional debug name included in registration logs.
        ///     可选 debug name included in 注册 logs.
        /// </summary>
        public string? DebugName { get; init; }
    }
}
