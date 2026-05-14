namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Declares an optional card-library compendium pool filter for a stand-alone
    ///     Declares an 可选 卡牌-library compendium pool 过滤 用于 a stand-alone
    ///     <see cref="MegaCrit.Sts2.Core.Models.CardPoolModel" />. By default such pools do not receive a filter row;
    ///     register this object to add one (icon path and stable id are required).
    ///     注册 this object to add one (icon path and stable id are required)。
    /// </summary>
    public sealed class CardLibraryCompendiumSharedPoolFilterRegistration
    {
        /// <summary>
        ///     Mod that registered this filter (for logging).
        ///     Mod that 已注册 this 过滤 (用于 logging).
        /// </summary>
        public required string OwningModId { get; init; }

        /// <summary>
        ///     Unique key among all compendium shared-pool filters (ASCII letters, digits, underscore only).
        ///     Unique key among all compendium shared-pool 过滤 (ASCII letters, digits, underscore only).
        /// </summary>
        public required string StableId { get; init; }

        /// <summary>
        ///     Godot resource path for the filter button icon (64px-style texture, same usage as mod character icons).
        ///     Godot 资源 路径 用于 the filter button 图标 (64px-style 纹理, same usage as mod character Icons).
        /// </summary>
        public required string IconTexturePath { get; init; }

        /// <summary>
        ///     Concrete card pool type whose <c>AllCardIds</c> define the filter predicate.
        ///     Concrete 卡牌 pool type whose <c>AllCardIds</c> define the filter predicate.
        /// </summary>
        public required Type CardPoolType { get; init; }

        /// <summary>
        ///     Optional placement rules; when <c>null</c> or empty, the filter row is appended after existing siblings
        ///     可选 placement rules; 当 <c>null</c> 或 empty, the 过滤 row is appended 之后 existing siblings
        ///     (end of the filter strip). When non-empty, the first rule with a resolvable vanilla anchor sets the base
        ///     (end of the 过滤 strip). 当 non-empty, the first rule 带有 a resolvable 原版 anchor 设置 the base
        ///     index and mod-to-mod constraints from the full list are merged (see patch documentation).
        ///     index 和 mod-to-mod constraints 从 the full list are merged (see patch documentation).
        /// </summary>
        public IReadOnlyList<CardLibraryCompendiumPlacementRule>? PlacementRules { get; init; }
    }
}
