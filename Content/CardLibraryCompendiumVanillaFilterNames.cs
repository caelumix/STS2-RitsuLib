namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Godot <c>%</c> unique names of all vanilla <c>NCardPoolFilter</c> pool toggles in the card-library
    ///     Godot <c>%</c> unique names of all 原版 <c>NCardPool过滤</c> pool toggles in the 卡牌-library
    ///     compendium strip (see
    ///     中文说明：compendium strip (see
    ///     <c>library.GetNodeOrNull&lt;NCardPoolFilter&gt;(...)</c> in game <c>NCardLibrary</c>). Set
    ///     <see cref="CardLibraryCompendiumPlacementRule.VanillaFilterAnchorUniqueName" /> to one of these
    ///     constants with
    ///     constants 带有
    ///     <see cref="CardLibraryCompendiumFilterInsertRelation" /> to place mod filter rows before or after
    ///     the corresponding vanilla control. Order of constants below matches the in-scene layout from left
    ///     the corresponding 原版 control. Order of constants below matches the in-场景 layout 从 left
    ///     to right in base game.
    ///     中文说明：to right in base game.
    /// </summary>
    public static class CardLibraryCompendiumVanillaFilterNames
    {
        /// <summary>
        ///     Ironclad (red) character pool filter.
        ///     Ironclad (red) character pool 过滤.
        /// </summary>
        public const string IroncladPool = "%IroncladPool";

        /// <summary>
        ///     Silent (green) character pool filter.
        ///     Silent (green) character pool 过滤.
        /// </summary>
        public const string SilentPool = "%SilentPool";

        /// <summary>
        ///     Defect (blue) character pool filter.
        ///     Defect (blue) character pool 过滤.
        /// </summary>
        public const string DefectPool = "%DefectPool";

        /// <summary>
        ///     Regent (purple) character pool filter.
        ///     Regent (purple) character pool 过滤.
        /// </summary>
        public const string RegentPool = "%RegentPool";

        /// <summary>
        ///     Necrobinder (orange) character pool filter.
        ///     Necrobinder (orange) character pool 过滤.
        /// </summary>
        public const string NecrobinderPool = "%NecrobinderPool";

        /// <summary>
        ///     Colorless pool filter.
        ///     Colorless pool 过滤.
        /// </summary>
        public const string ColorlessPool = "%ColorlessPool";

        /// <summary>
        ///     Ancients pool filter.
        ///     Ancients pool 过滤.
        /// </summary>
        public const string AncientsPool = "%AncientsPool";

        /// <summary>
        ///     Misc (token) pool filter.
        ///     Misc (token) pool 过滤.
        /// </summary>
        public const string MiscPool = "%MiscPool";

        private static readonly string[] AllInStripOrderArray =
        [
            IroncladPool, SilentPool, DefectPool, RegentPool, NecrobinderPool,
            ColorlessPool, AncientsPool, MiscPool,
        ];

        /// <summary>
        ///     The eight vanilla <c>%</c> unique names in compendium strip order (left to right, same as
        ///     The eight 原版 <c>%</c> unique names in compendium strip order (left to right, same as
        ///     <c>NCardLibrary</c> field setup). For iteration when resolving anchors; prefer the
        ///     <see cref="IroncladPool" />–<see cref="MiscPool" /> constants for single anchors.
        /// </summary>
        public static ReadOnlySpan<string> AllInStripOrder => AllInStripOrderArray;
    }
}
