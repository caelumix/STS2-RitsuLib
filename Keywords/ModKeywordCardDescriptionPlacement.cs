namespace STS2RitsuLib.Keywords
{
    /// <summary>
    ///     Where a registered mod keyword’s inline card text (gold title + period) is merged into the rendered card
    ///     description, mirroring vanilla <c>CardKeywordOrder</c> behavior.
    ///     已注册 mod keyword 的 inline card text（gold title + period）应合并到渲染后 card description 的位置，
    ///     对应原版 <c>CardKeywordOrder</c> 行为。
    /// </summary>
    public enum ModKeywordCardDescriptionPlacement
    {
        /// <summary>
        ///     Do not inject keyword text into the card description (default).
        ///     不向 card description 注入 keyword 文本（默认）。
        /// </summary>
        None = 0,

        /// <summary>
        ///     Insert before the main description block (vanilla “before description” keywords).
        ///     插入到主 description block 之前（原版 “before description” keyword）。
        /// </summary>
        BeforeCardDescription = 1,

        /// <summary>
        ///     Append after the main description block (vanilla “after description” keywords).
        ///     追加到主 description block 之后（原版 “after description” keyword）。
        /// </summary>
        AfterCardDescription = 2,
    }
}
