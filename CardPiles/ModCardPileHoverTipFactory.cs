using Godot;
using MegaCrit.Sts2.Core.HoverTips;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Builds a vanilla <see cref="HoverTip" /> for a <see cref="ModCardPileDefinition" /> by combining its
    ///     localized title / description (resolved against <see cref="ModCardPileSpec.HoverTipLocTable" />)
    ///     with the icon texture loaded from <c>IconPath</c>. Mirrors <c>ModKeywordRegistry.CreateHoverTip</c>
    ///     so the same hover UX is available for piles.
    ///     为 <c>ModCardPileDefinition</c> 构建原版 <c>HoverTip</c>：组合其本地化 title /
    ///     description（基于 <c>ModCardPileSpec.HoverTipLocTable</c> 解析）和从 <c>IconPath</c>
    ///     加载的图标 texture。它对应 <c>ModKeywordRegistry.CreateHoverTip</c>，让 pile 也具备相同 hover UX。
    /// </summary>
    public static class ModCardPileHoverTipFactory
    {
        /// <summary>
        ///     Produces a <see cref="HoverTip" /> for <paramref name="definition" />. Title and description
        ///     come from <see cref="ModCardPileDefinition.Title" /> / <see cref="ModCardPileDefinition.Description" />
        ///     (keys use <see cref="ModCardPileDefinition.Id" /> as stem), and the icon is loaded
        ///     from <c>ResourceLoader</c> when <see cref="ModCardPileDefinition.IconPath" /> exists.
        ///     为 <c>definition</c> 生成 <c>HoverTip</c>。title 和 description 来自
        ///     <see cref="ModCardPileDefinition.Title" /> / <see cref="ModCardPileDefinition.Description" />
        ///     （key 使用 <c>ModCardPileDefinition.Id</c> 作为 stem）；当
        ///     <c>ModCardPileDefinition.IconPath</c> 存在时，图标由 <c>ResourceLoader</c> 加载。
        /// </summary>
        public static HoverTip Create(ModCardPileDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            Texture2D? icon = null;
            if (!string.IsNullOrWhiteSpace(definition.IconPath)
                && ResourceLoader.Exists(definition.IconPath))
                icon = ResourceLoader.Load<Texture2D>(definition.IconPath);

            return new(definition.Title, definition.Description, icon);
        }
    }
}
