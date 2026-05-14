namespace STS2RitsuLib.Diagnostics.CardExport
{
    /// <summary>
    ///     How much surrounding UI to include when rasterizing a card.
    ///     How much surrounding UI to include 当 rasterizing a 卡牌.
    /// </summary>
    public enum CardPngExportCaptureMode
    {
        /// <summary>
        ///     Only the <c>NCard</c> control (game-accurate card chrome, portrait, text).
        ///     Only the <c>NCard</c> control (game-accurate 卡牌 chrome, 肖像, text).
        /// </summary>
        CardOnly,

        /// <summary>
        ///     Card plus a right-hand column that approximates hover tips: text tips use the real
        ///     卡牌 plus a right-hand column that approximates hover tips: text tips 使用 the real
        ///     <c>hover_tip.tscn</c>; card-reference tips render as a scaled mini <c>NCard</c>.
        ///     Layout is fixed (not the same global positioning as in-game tooltips).
        ///     中文说明：Layout is fixed (not the same global positioning as in-game tooltips).
        /// </summary>
        CardWithHoverTipsPanel,
    }
}
