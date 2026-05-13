namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Optional presentation overrides for the RitsuLib mod settings screen. Safe to set at mod load time.
    ///     RitsuLib Mod 设置屏幕的可选呈现覆盖项。可安全地在 Mod 加载时设置。
    /// </summary>
    public static class ModSettingsUiPresentation
    {
        /// <summary>
        ///     Default maximum height for paragraph entry body text (from <c>AddParagraph</c>) before the block uses
        ///     internal vertical scrolling. <c>null</c> (default) means no height cap — text grows with the layout.
        ///     Per-entry <see cref="ParagraphModSettingsEntryDefinition.MaxBodyHeight" /> overrides this when set.
        ///     段落条目正文（来自 <c>AddParagraph</c>）在启用内部垂直滚动前的默认最大高度。
        ///     <c>null</c>（默认）表示不限制高度，文本随布局增长。设置了逐条目的
        ///     <see cref="ParagraphModSettingsEntryDefinition.MaxBodyHeight" /> 时会覆盖此值。
        /// </summary>
        public static float? ParagraphMaxBodyHeight { get; set; }
    }
}
