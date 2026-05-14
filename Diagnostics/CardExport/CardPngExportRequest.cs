namespace STS2RitsuLib.Diagnostics.CardExport
{
    /// <summary>
    ///     Parameters for a batch PNG export of <see cref="MegaCrit.Sts2.Core.Models.CardModel" /> instances.
    ///     Parameters 用于 a batch PNG export of <c>MegaCrit.Sts2.Core.Models.CardModel</c> instances.
    /// </summary>
    public readonly struct CardPngExportRequest
    {
        /// <summary>
        ///     Absolute or Godot <c>user://</c> / <c>res://</c> output directory. Invalid path characters in card ids are
        ///     Absolute 或 Godot <c>使用r://</c> / <c>res://</c> output directory. In有效 路径 characters in 卡牌 ids are
        ///     stripped from file names.
        ///     stripped 从 file names.
        /// </summary>
        public string OutputDirectory { get; init; }

        /// <summary>
        ///     Uniform scale applied to the card (and panel layout). Values below 1 shrink; above 1 enlarge (e.g. 2 for
        ///     Uniform scale applied to the 卡牌 (and panel layout). Values below 1 shrink; above 1 enlarge (e.g. 2 用于
        ///     higher-resolution exports).
        ///     中文说明：higher-resolution exports).
        /// </summary>
        public float Scale { get; init; }

        /// <summary>
        ///     Rasterization mode.
        ///     中文说明：Rasterization mode.
        /// </summary>
        public CardPngExportCaptureMode CaptureMode { get; init; }

        /// <summary>
        ///     When true, also exports an <c>_upgraded</c> PNG for cards where
        ///     当 true, also exports an <c>_upgraded</c> PNG 用于 卡牌s where
        ///     <see cref="MegaCrit.Sts2.Core.Models.CardModel.IsUpgradable" /> is true.
        /// </summary>
        public bool IncludeUpgradedVariants { get; init; }

        /// <summary>
        ///     When set, only cards whose <see cref="MegaCrit.Sts2.Core.Models.ModelId.Entry" /> contains this substring
        ///     当 设置, only 卡牌s whose <c>MegaCrit.Sts2.Core.Models.ModelId.Entry</c> 包含 this substring
        ///     (ordinal ignore-case) are exported.
        ///     中文说明：(ordinal ignore-case) are exported.
        /// </summary>
        public string? IdFilterSubstring { get; init; }

        /// <summary>
        ///     When positive, stops after this many <em>base</em> cards (upgraded variants do not count toward the cap).
        ///     当 positive, stops 之后 this many <em>base</em> 卡牌s (upgraded variants do not count toward the cap).
        /// </summary>
        public int MaxBaseCards { get; init; }

        /// <summary>
        ///     When false (default), only exports cards that appear in the in-game card library
        ///     当 false (default), only exports 卡牌s that appear in the in-game 卡牌 library
        ///     (<see cref="MegaCrit.Sts2.Core.Models.CardModel.ShouldShowInCardLibrary" />), matching the compendium set.
        ///     (<c>MegaCrit.Sts2.Core.Models.CardModel.ShouldShowInCardLibrary</c>), matching the compendium 设置.
        ///     When true, also includes registered cards that are hidden from the library.
        ///     为 true 时，also includes registered cards that are hidden from the library。
        /// </summary>
        public bool IncludeCardsHiddenFromLibrary { get; init; }

        /// <summary>
        ///     Defaults: <see cref="Scale" /> = 1, <see cref="CaptureMode" /> = <see cref="CardPngExportCaptureMode.CardOnly" />,
        ///     Defaults: <c>Scale</c> = 1, <c>CaptureMode</c> = <c>卡牌PngExportCaptureMode.卡牌Only</c>,
        ///     <see cref="IncludeUpgradedVariants" /> = true.
        /// </summary>
        public static CardPngExportRequest CreateDefault(string outputDirectory)
        {
            return new()
            {
                OutputDirectory = outputDirectory,
                Scale = 1f,
                CaptureMode = CardPngExportCaptureMode.CardOnly,
                IncludeUpgradedVariants = true,
                MaxBaseCards = 0,
            };
        }
    }
}
