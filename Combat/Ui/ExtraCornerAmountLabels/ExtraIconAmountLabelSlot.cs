using Godot;

namespace STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels
{
    /// <summary>
    ///     One extra amount/text badge on a combat UI icon. Each slot is laid out independently.
    ///     One extra amount/text badge on a combat UI 图标. Each slot is laid out independently.
    /// </summary>
    /// <param name="Text">
    ///     Badge text. Whitespace-only entries are skipped.
    ///     中文说明：Badge text. Whitespace-only entries are skipped.
    /// </param>
    /// <param name="Corner">
    ///     <see cref="ExtraIconAmountLabelCorner.TopLeft" />, <see cref="ExtraIconAmountLabelCorner.TopRight" />,
    ///     <see cref="ExtraIconAmountLabelCorner.BottomLeft" />, or <see cref="ExtraIconAmountLabelCorner.BottomRight" />
    ///     for built-in layout, or
    ///     用于 built-in layout, or
    ///     <see cref="ExtraIconAmountLabelCorner.Custom" /> with <paramref name="CustomRect" />.
    /// </param>
    /// <param name="CustomRect">
    ///     When <paramref name="Corner" /> is <see cref="ExtraIconAmountLabelCorner.Custom" />, local rectangle
    ///     当 <c>Corner</c> is <c>ExtraIconAmountLabelCorner.自定义</c>, local rectangle
    ///     (position and size in host control space, same convention as <c>offset_*</c> for a top-left-anchored child).
    ///     (position 和 size in host control space, same convention as <c>off设置_*</c> 用于 a top-left-anchored child).
    ///     Ignored for presets. Entries with non-positive width or height are skipped at runtime.
    ///     Ignored 用于 pre设置. Entries 带有 non-positive width 或 height are skipped at runtime.
    /// </param>
    /// <param name="FontColor">
    ///     When set, overrides the foreground color for this badge after base typography is copied from the host
    ///     当 设置, overrides the 用于eground color 用于 this badge 之后 base typography is copied 从 the host
    ///     reference label. When <see langword="null" />, keeps the inherited color from that reference (default).
    ///     reference label. 当 <see langword="null" />, keeps the inherited color 从 that reference (default).
    /// </param>
    /// <param name="FontOutlineColor">
    ///     When set, overrides the outline color after base typography is copied. When <see langword="null" />, keeps
    ///     当 设置, overrides the outline color 之后 base typography is copied. 当 <see langword="null" />, keeps
    ///     the inherited outline from that reference (default).
    ///     该 inherited outline from that reference (default)。
    /// </param>
    public readonly record struct ExtraIconAmountLabelSlot(
        string Text,
        ExtraIconAmountLabelCorner Corner,
        Rect2 CustomRect,
        Color? FontColor,
        Color? FontOutlineColor)
    {
        /// <summary>
        ///     Preset-corner slot: default <c>CustomRect</c>, no color overrides.
        ///     Pre设置-corner slot: default <c>CustomRect</c>, no color overrides.
        /// </summary>
        public ExtraIconAmountLabelSlot(string text, ExtraIconAmountLabelCorner corner)
            : this(text, corner, default, null, null)
        {
        }

        /// <summary>
        ///     Custom-corner slot from bounds, no color overrides.
        ///     自定义-corner slot 从 bounds, no color overrides.
        /// </summary>
        public ExtraIconAmountLabelSlot(string text, ExtraIconAmountLabelCorner corner, Rect2 customRect)
            : this(text, corner, customRect, null, null)
        {
        }

        /// <summary>
        ///     Shorthand for a preset-corner slot: <c>new ExtraIconAmountLabelSlot(text, corner)</c>.
        ///     Shorthand 用于 a pre设置-corner slot: <c>new ExtraIconAmountLabelSlot(text, corner)</c>.
        /// </summary>
        public static ExtraIconAmountLabelSlot At(ExtraIconAmountLabelCorner corner, string text)
        {
            return new(text, corner);
        }

        /// <summary>
        ///     Preset-corner slot; forwards to the primary constructor with <c>CustomRect</c> default and no outline
        ///     Pre设置-corner slot; 用于wards to the primary constructor 带有 <c>CustomRect</c> default 和 no outline
        ///     override.
        ///     中文说明：override.
        /// </summary>
        public static ExtraIconAmountLabelSlot At(ExtraIconAmountLabelCorner corner, string text, Color? fontColor)
        {
            return new(text, corner, default, fontColor, null);
        }

        /// <summary>
        ///     Preset-corner slot with foreground and outline overrides (use <see langword="null" /> to keep inherited
        ///     Pre设置-corner slot 带有 用于eground 和 outline overrides (使用 <see langword="null" /> to keep inherited
        ///     reference colors for that channel).
        ///     reference colors 用于 that channel).
        /// </summary>
        public static ExtraIconAmountLabelSlot At(ExtraIconAmountLabelCorner corner, string text, Color? fontColor,
            Color? fontOutlineColor)
        {
            return new(text, corner, default, fontColor, fontOutlineColor);
        }

        /// <summary>
        ///     Slot at <see cref="ExtraIconAmountLabelCorner.Custom" /> with position and size (host-local space).
        ///     Slot at <c>ExtraIconAmountLabelCorner.自定义</c> 带有 position 和 size (host-local space).
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, Rect2 customRect)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom, customRect);
        }

        /// <summary>
        ///     Custom-bounds slot; foreground override only (no outline override).
        ///     自定义-bounds slot; 用于eground override only (no outline override).
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, Rect2 customRect, Color? fontColor)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom, customRect, fontColor, null);
        }

        /// <summary>
        ///     Custom-bounds slot with optional foreground and outline overrides (use <see langword="null" /> to keep
        ///     自定义-bounds slot 带有 可选 用于eground 和 outline overrides (使用 <see langword="null" /> to keep
        ///     inherited reference colors).
        ///     中文说明：inherited reference colors).
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, Rect2 customRect, Color? fontColor,
            Color? fontOutlineColor)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom, customRect, fontColor,
                fontOutlineColor);
        }

        /// <summary>
        ///     Slot at <see cref="ExtraIconAmountLabelCorner.Custom" /> with edges
        ///     Slot at <c>ExtraIconAmountLabelCorner.自定义</c> 带有 edges
        ///     <paramref name="left" />, <paramref name="top" />, <paramref name="right" />,
        ///     <paramref name="bottom" /> (host-local, same convention as control offsets from top-left anchor).
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, float left, float top, float right, float bottom)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom,
                new(left, top, right - left, bottom - top));
        }

        /// <summary>
        ///     Custom edge slot; foreground override only (no outline override).
        ///     自定义 edge slot; 用于eground override only (no outline override).
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, float left, float top, float right, float bottom,
            Color? fontColor)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom,
                new(left, top, right - left, bottom - top), fontColor, null);
        }

        /// <summary>
        ///     Custom edge slot with optional foreground and outline overrides.
        ///     自定义 edge slot 带有 可选 用于eground 和 outline overrides.
        /// </summary>
        public static ExtraIconAmountLabelSlot WithCustom(string text, float left, float top, float right, float bottom,
            Color? fontColor, Color? fontOutlineColor)
        {
            return new(text, ExtraIconAmountLabelCorner.Custom,
                new(left, top, right - left, bottom - top), fontColor, fontOutlineColor);
        }
    }
}
