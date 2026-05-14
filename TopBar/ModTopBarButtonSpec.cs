using System.Numerics;

namespace STS2RitsuLib.TopBar
{
    /// <summary>
    ///     Declarative spec for a mod-owned top-bar button. Mirrors <c>ModCardPileSpec</c>'s structure but is
    ///     Declarative spec 用于 a mod-owned top-bar button. Mirrors <c>ModCardPileSpec</c>'s structure but is
    ///     fully decoupled from card piles — the only contract is "show a button next to the vanilla deck
    ///     fully decoupled 从 卡牌 piles — the only contract is "show a button next to the 原版 deck
    ///     button, hover-tip via <c>static_hover_tips</c>, click runs a callback".
    ///     button, hover-tip via <c>static_hover_tips</c>, click runs a callback".
    /// </summary>
    /// <remarks>
    ///     Localization follows the same <c>static_hover_tips.{id}.title</c> / <c>.description</c>
    ///     中文说明：Localization follows the same <c>static_hover_tips.{id}.title</c> / <c>.description</c>
    ///     convention as mod card piles, where <c>id</c> is the qualified button id.
    ///     convention as mod 卡牌 piles, where <c>id</c> is the qualified button id.
    /// </remarks>
    public sealed record ModTopBarButtonSpec
    {
        /// <summary>
        ///     Vanilla loc table that the button's hover-tip resolves against.
        ///     原版 loc table that the button's hover-tip 解析 against.
        /// </summary>
        public const string HoverTipLocTable = ModTopBarButtonLocConstants.HoverTipLocTable;

        /// <summary>
        ///     Godot resource path for the button icon (for example <c>res://my_mod/icon.png</c>).
        ///     Godot 资源 路径 用于 the button 图标 (用于 example <c>res://my_mod/图标.png</c>).
        /// </summary>
        public string? IconPath { get; init; }

        /// <summary>
        ///     Sort order within a single mod's top-bar buttons; lower values render closer to the vanilla
        ///     Sort order 带有in a single mod's top-bar buttons; lower values render closer to the 原版
        ///     deck button.
        ///     中文说明：deck button.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        ///     Extra pixel offset applied on top of the auto-stacked slot position. Use this for
        ///     Extra pixel off设置 applied on top of the auto-stacked slot position. 使用 this 用于
        ///     fine-tuning when the default horizontal stacking layout doesn't match your icon.
        ///     fine-tuning 当 the default horizontal stacking layout doesn't match your 图标.
        /// </summary>
        public Vector2 Offset { get; init; }

        /// <summary>
        ///     Required click handler. Receives a <see cref="ModTopBarButtonContext" /> that exposes
        ///     中文说明：Required click handler. Receives a <c>ModTopBarButtonContext</c> that exposes
        ///     <see cref="ModTopBarButtonContext.OpenCapstoneScreen" /> / related helpers.
        /// </summary>
        public Action<ModTopBarButtonContext>? OnClick { get; init; }

        /// <summary>
        ///     Optional predicate that decides whether the button is visible for the current player. When
        ///     可选 predicate that decides whether the button is visible 用于 the current player. 当
        ///     null the button is always visible (once the top bar exists). Evaluated on
        ///     中文说明：null the button is always visible (once the top bar exists). Evaluated on
        ///     <see cref="Godot.Node._Process" /> to keep the visibility state in sync with combat/run state
        ///     changes.
        ///     中文说明：changes.
        /// </summary>
        public Func<ModTopBarButtonContext, bool>? VisibleWhen { get; init; }

        /// <summary>
        ///     Optional predicate used by the button to decide when it should render in its "screen open"
        ///     可选 predicate used 通过 the button to decide 当 it should render in its "screen open"
        ///     rocking state (mirroring vanilla <c>NTopBarDeckButton</c> / <c>NTopBarMapButton</c>). Typical
        ///     rocking state (mirroring 原版 <c>NTopBarDeckButton</c> / <c>NTopBarMapButton</c>). Typical
        ///     usage is
        ///     中文说明：usage is
        ///     <c>ctx =&gt; ModScreenService.CurrentCapstoneScreen is MyScreen</c>. When null the button never
        ///     enters the open state — pick this for one-shot "fire and forget" click handlers.
        ///     enters the open state — pick this 用于 one-shot "fire 和 用于get" click handlers.
        /// </summary>
        public Func<ModTopBarButtonContext, bool>? IsOpenWhen { get; init; }

        /// <summary>
        ///     Optional provider used to populate the count badge under the button's icon. Called on
        ///     可选 provider used to populate the count badge under the button's 图标. Called on
        ///     <see cref="Godot.Node._Process" /> — keep it cheap. When null, the count label is hidden
        ///     entirely, which is the right choice for fire-and-forget action buttons (menus, toggles).
        ///     entirely, which is the right choice 用于 fire-and-用于get action buttons (menus, toggles).
        ///     When set, the button reuses the vanilla card-pile "number jumped up" bump animation so the
        ///     当 设置, the button re使用 the 原版 卡牌-pile "number jumped up" bump animation so the
        ///     feedback feels identical to the player's deck / draw / discard counts.
        ///     feedback feels identical to the player's deck / draw / dis卡牌 counts.
        /// </summary>
        public Func<ModTopBarButtonContext, int>? CountProvider { get; init; }
    }

    /// <summary>
    ///     Shared constants for the top-bar-button localization convention.
    ///     Shared constants 用于 the top-bar-button localization convention.
    /// </summary>
    internal static class ModTopBarButtonLocConstants
    {
        public const string HoverTipLocTable = "static_hover_tips";
    }
}
