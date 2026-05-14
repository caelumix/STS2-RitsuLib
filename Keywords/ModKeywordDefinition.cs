using MegaCrit.Sts2.Core.Entities.Cards;

namespace STS2RitsuLib.Keywords
{
    /// <summary>
    ///     Immutable registration data for a mod keyword (localization tables, keys, optional icon).
    ///     mod keyword 的不可变注册数据（本地化 table、key、可选图标）。
    /// </summary>
    public sealed record ModKeywordDefinition
    {
        /// <summary>
        ///     Original binary-compatible constructor (seven CLR parameters); prior RitsuLib keyword definitions.
        ///     原始二进制兼容构造函数（七个 CLR 参数）；用于旧版 RitsuLib keyword definition。
        /// </summary>
        public ModKeywordDefinition(
            string ModId,
            string Id,
            string TitleTable,
            string TitleKey,
            string DescriptionTable,
            string DescriptionKey,
            string? IconPath = null)
        {
            this.ModId = ModId;
            this.Id = Id;
            this.TitleTable = TitleTable;
            this.TitleKey = TitleKey;
            this.DescriptionTable = DescriptionTable;
            this.DescriptionKey = DescriptionKey;
            this.IconPath = IconPath;
            CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.None;
            IncludeInCardHoverTip = true;
        }

        /// <summary>
        ///     Extended constructor: same as the legacy seven-parameter ABI plus placement and hover-tip inclusion.
        ///     扩展构造函数：与旧七参数 ABI 相同，并额外包含 placement 与 hover-tip inclusion。
        /// </summary>
        public ModKeywordDefinition(
            string ModId,
            string Id,
            string TitleTable,
            string TitleKey,
            string DescriptionTable,
            string DescriptionKey,
            string? IconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            this.ModId = ModId;
            this.Id = Id;
            this.TitleTable = TitleTable;
            this.TitleKey = TitleKey;
            this.DescriptionTable = DescriptionTable;
            this.DescriptionKey = DescriptionKey;
            this.IconPath = IconPath;
            CardDescriptionPlacement = cardDescriptionPlacement;
            IncludeInCardHoverTip = includeInCardHoverTip;
        }

        /// <summary>
        ///     Owning mod manifest id.
        ///     所属 mod manifest id。
        /// </summary>
        public string ModId { get; init; } = string.Empty;

        /// <summary>
        ///     Normalized keyword id (lowercase).
        ///     normalized keyword id（小写）。
        /// </summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>
        ///     Localization table for the title.
        ///     title 使用的本地化 table。
        /// </summary>
        public string TitleTable { get; init; } = string.Empty;

        /// <summary>
        ///     Key for the title string.
        ///     title 字符串使用的 key。
        /// </summary>
        public string TitleKey { get; init; } = string.Empty;

        /// <summary>
        ///     Localization table for the body text.
        ///     body text 使用的本地化 table。
        /// </summary>
        public string DescriptionTable { get; init; } = string.Empty;

        /// <summary>
        ///     Key for the description string.
        ///     description 字符串使用的 key。
        /// </summary>
        public string DescriptionKey { get; init; } = string.Empty;

        /// <summary>
        ///     Optional Godot resource path for hover icon.
        ///     hover icon 使用的可选 Godot ResourcePath。
        /// </summary>
        public string? IconPath { get; init; }

        /// <summary>
        ///     Whether and where to inject keyword BBCode into card descriptions.
        ///     是否以及在哪里将 keyword BBCode 注入 card description。
        /// </summary>
        public ModKeywordCardDescriptionPlacement CardDescriptionPlacement { get; init; } =
            ModKeywordCardDescriptionPlacement.None;

        /// <summary>
        ///     When true, this keyword’s hover tip is included from <c>RegisteredKeywordIds</c> / runtime mod-keyword sets
        ///     on cards and other mod templates.
        ///     为 true 时，此 keyword 的 hover tip 会从 card 和其它 mod template 上的
        ///     <c>RegisteredKeywordIds</c> / runtime mod-keyword set 中包含进来。
        /// </summary>
        public bool IncludeInCardHoverTip { get; init; }

        /// <summary>
        ///     Deterministic <see cref="CardKeyword" /> value minted for this keyword (hash of <see cref="Id" />,
        ///     forced above the vanilla enum range). Stored directly inside <c>CardModel.Keywords</c> so the mod
        ///     keyword rides vanilla workflows (lookups, cloning, canonical seeding, per-run saves) without any
        ///     parallel side-loaded state. Populated by <see cref="ModKeywordRegistry" /> at registration time;
        ///     remains <see cref="CardKeyword.None" /> for definitions constructed outside the registry.
        ///     为此 keyword 确定性 minted 的 <c>CardKeyword</c> 值（<c>Id</c> 的 hash，
        ///     强制高于原版 enum 范围）。它会直接存入 <c>CardModel.Keywords</c>，使 mod keyword 沿用原版流程
        ///     （lookup、clone、canonical seeding、per-run save），无需任何并行 side-loaded state。
        ///     由 <c>ModKeywordRegistry</c> 在注册时填充；在 registry 外构造的 definition 保持
        ///     <see cref="CardKeyword.None" />。
        /// </summary>
        public CardKeyword CardKeywordValue { get; init; } = CardKeyword.None;
    }
}
