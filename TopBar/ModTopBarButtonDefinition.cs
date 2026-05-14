using System.Numerics;
using MegaCrit.Sts2.Core.Localization;

namespace STS2RitsuLib.TopBar
{
    /// <summary>
    ///     Immutable registry entry for a mod-owned top-bar button.
    ///     Immutable 注册表 entry 用于 a mod-owned top-bar button.
    /// </summary>
    public sealed record ModTopBarButtonDefinition
    {
        internal ModTopBarButtonDefinition(
            string modId,
            string id,
            string? iconPath,
            int order,
            Vector2 offset,
            Action<ModTopBarButtonContext>? onClick,
            Func<ModTopBarButtonContext, bool>? visibleWhen,
            Func<ModTopBarButtonContext, bool>? isOpenWhen,
            Func<ModTopBarButtonContext, int>? countProvider)
        {
            ModId = modId;
            Id = id;
            IconPath = iconPath;
            Order = order;
            Offset = offset;
            OnClick = onClick;
            VisibleWhen = visibleWhen;
            IsOpenWhen = isOpenWhen;
            CountProvider = countProvider;
        }

        /// <summary>
        ///     Owning mod id.
        ///     中文说明：Owning mod id.
        /// </summary>
        public string ModId { get; }

        /// <summary>
        ///     Normalized global id (e.g. <c>MYMOD_TOPBARBUTTON_RECIPES</c>).
        ///     中文说明：Normalized global id (e.g. <c>MYMOD_TOPBARBUTTON_RECIPES</c>).
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Godot resource path for the icon, or null.
        ///     Godot 资源 路径 用于 the 图标, 或 null.
        /// </summary>
        public string? IconPath { get; }

        /// <summary>
        ///     Sort order within this mod's top-bar buttons.
        ///     Sort order 带有in this mod's top-bar buttons.
        /// </summary>
        public int Order { get; }

        /// <summary>
        ///     Extra pixel offset on top of the auto-stacked slot.
        ///     Extra pixel off设置 on top of the auto-stacked slot.
        /// </summary>
        public Vector2 Offset { get; }

        /// <summary>
        ///     Click handler; see <see cref="ModTopBarButtonSpec.OnClick" />.
        ///     中文说明：Click handler; see <c>ModTopBarButtonSpec.OnClick</c>.
        /// </summary>
        public Action<ModTopBarButtonContext>? OnClick { get; }

        /// <summary>
        ///     Optional visibility predicate; see <see cref="ModTopBarButtonSpec.VisibleWhen" />.
        ///     可选 visibility predicate; see <c>ModTopBarButtonSpec.VisibleWhen</c>.
        /// </summary>
        public Func<ModTopBarButtonContext, bool>? VisibleWhen { get; }

        /// <summary>
        ///     Optional "screen open" predicate; see <see cref="ModTopBarButtonSpec.IsOpenWhen" />.
        ///     可选 "screen open" predicate; see <c>ModTopBarButtonSpec.IsOpenWhen</c>.
        /// </summary>
        public Func<ModTopBarButtonContext, bool>? IsOpenWhen { get; }

        /// <summary>
        ///     Optional count provider for the badge; see <see cref="ModTopBarButtonSpec.CountProvider" />.
        ///     可选 count provider 用于 the badge; see <c>ModTopBarButtonSpec.CountProvider</c>.
        /// </summary>
        public Func<ModTopBarButtonContext, int>? CountProvider { get; }

        /// <summary>
        ///     Hover-tip title resolved against <c>static_hover_tips</c> with key <c>{Id}.title</c>.
        ///     Hover-tip title resolved against <c>static_hover_tips</c> 带有 key <c>{Id}.title</c>.
        /// </summary>
        public LocString Title => new(ModTopBarButtonSpec.HoverTipLocTable, $"{Id}.title");

        /// <summary>
        ///     Hover-tip description resolved against <c>static_hover_tips</c> with key <c>{Id}.description</c>.
        ///     Hover-tip description resolved against <c>static_hover_tips</c> 带有 key <c>{Id}.description</c>.
        /// </summary>
        public LocString Description => new(ModTopBarButtonSpec.HoverTipLocTable, $"{Id}.description");
    }
}
