namespace STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels
{
    /// <summary>
    ///     Preset corners for extra icon badges. Vanilla stack/value UI uses bottom-right; mods use the other three
    ///     Pre设置 corners 用于 extra 图标 badges. 原版 stack/value UI 使用 bottom-right; mods 使用 the other three
    ///     corners, or <see cref="Custom" /> with explicit bounds on <see cref="ExtraIconAmountLabelSlot" />.
    ///     corners, 或 <c>自定义</c> 带有 explicit bounds on <c>ExtraIconAmountLabelSlot</c>.
    /// </summary>
    public enum ExtraIconAmountLabelCorner
    {
        /// <summary>
        ///     Top-left band (built-in layout per host kind).
        ///     中文说明：Top-left band (built-in layout per host kind).
        /// </summary>
        TopLeft,

        /// <summary>
        ///     Top-right band (built-in layout per host kind).
        ///     中文说明：Top-right band (built-in layout per host kind).
        /// </summary>
        TopRight,

        /// <summary>
        ///     Bottom-left band (built-in layout per host kind).
        ///     中文说明：Bottom-left band (built-in layout per host kind).
        /// </summary>
        BottomLeft,

        /// <summary>
        ///     Bottom-right band (built-in layout per host kind).
        ///     中文说明：Bottom-right band (built-in layout per host kind).
        /// </summary>
        BottomRight,

        /// <summary>
        ///     Use <see cref="ExtraIconAmountLabelSlot.CustomRect" />; horizontal and vertical alignment are centered.
        ///     使用 <c>ExtraIconAmountLabelSlot.CustomRect</c>; horizontal 和 vertical alignment are centered.
        /// </summary>
        Custom,
    }
}
