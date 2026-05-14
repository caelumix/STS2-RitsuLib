namespace STS2RitsuLib.Diagnostics.CompendiumExport
{
    /// <summary>
    ///     Batch export of compendium-style detail panels (relic inspect popup, potion lab focus + hovers) to PNG.
    ///     Batch export of compendium-style detail panels (遗物 inspect popup, potion lab focus + hovers) to PNG.
    /// </summary>
    public readonly struct CompendiumPngExportRequest
    {
        /// <summary>
        ///     Absolute or Godot <c>user://</c> / <c>res://</c> output directory.
        ///     Absolute 或 Godot <c>使用r://</c> / <c>res://</c> output directory.
        /// </summary>
        public required string OutputDirectory { get; init; }

        /// <summary>
        ///     Uniform scale (same as card export).
        ///     Uniform scale (same as 卡牌 export).
        /// </summary>
        public double Scale { get; init; }

        /// <summary>
        ///     Optional id substring filter; null / empty = all.
        ///     可选 id substring 过滤; null / empty = all.
        /// </summary>
        public string? IdFilterSubstring { get; init; }

        /// <summary>
        ///     Export the relic inspection popup (same <c>inspect_relic_screen</c> <c>Popup</c> content as the main menu
        ///     Export the 遗物 inspection popup (same <c>inspect_遗物_screen</c> <c>Popup</c> content as the main menu
        ///     compendium when an entry is selected).
        ///     compendium 当 an entry is selected).
        /// </summary>
        public bool Relics { get; init; }

        /// <summary>
        ///     Export the potion lab “focus” view (1.2x <c>NPotion</c> + character pool outline, with hover tips to the
        ///     Export the potion lab “focus” view (1.2x <c>NPotion</c> + character pool outline, 带有 hover tips to the
        ///     right).
        ///     中文说明：right).
        /// </summary>
        public bool Potions { get; init; }

        /// <summary>
        ///     When true, relic export includes the secondary hover columns.
        ///     为 true 时，relic export includes the secondary hover columns。
        /// </summary>
        public bool IncludeRelicHoverTips { get; init; }

        /// <summary>
        ///     Default <see cref="Scale" /> = 1; both <see cref="Relics" /> and <see cref="Potions" /> true.
        ///     默认 <c>Scale</c> = 1; both <c>Relics</c> and <c>Potions</c> true。
        /// </summary>
        public static CompendiumPngExportRequest CreateDefault(string outputDirectory)
        {
            return new()
            {
                OutputDirectory = outputDirectory,
                Scale = 1.0,
                Relics = true,
                Potions = true,
                IncludeRelicHoverTips = true,
            };
        }
    }
}
