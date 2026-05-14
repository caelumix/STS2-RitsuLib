using STS2RitsuLib.CardPiles;

namespace STS2RitsuLib.Interop.AutoRegistration
{
    /// <summary>
    ///     Declaratively registers a mod card pile (see <see cref="ModCardPileRegistry" />). Place on any
    ///     concrete class inside your mod assembly; the type itself acts as the registration carrier and
    ///     can optionally implement <see cref="IModCardPileHandler" /> to customise button-click behaviour.
    ///     声明式注册 mod card pile（参见 <c>ModCardPileRegistry</c>）。将其放在你的 mod assembly
    ///     中任意具体类上；该类型本身作为注册载体，并且可以选择实现 <c>IModCardPileHandler</c>
    ///     来自定义按钮点击行为。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Field semantics mirror <see cref="ModCardPileSpec" />. Localization follows the vanilla
    ///         pile convention — hover-tip title / description and the empty-pile thought bubble are all
    ///         resolved against <see cref="ModCardPileSpec.HoverTipLocTable" /> using the keys
    ///         <c>"{id}.title"</c>, <c>"{id}.description"</c> and <c>"{id}.empty"</c> where <c>id</c> is the
    ///         qualified pile id. Because
    ///         mods can only extend existing loc tables (not create new ones) the table itself is not
    ///         configurable; author your translations in <c>static_hover_tips.json</c>.
    ///         字段语义对应 <c>ModCardPileSpec</c>。本地化遵循原版 pile 约定：hover-tip 标题 /
    ///         描述以及空 pile thought bubble 都会基于 <c>ModCardPileSpec.HoverTipLocTable</c>
    ///         解析，使用 <c>"{id}.title"</c>、<c>"{id}.description"</c> 和 <c>"{id}.empty"</c> key，
    ///         其中 <c>id</c> 是 qualified pile id。由于 mod 只能扩展现有 loc table（不能创建新表），
    ///         table 本身不可配置；请在 <c>static_hover_tips.json</c> 中编写翻译。
    ///     </para>
    ///     <para>
    ///         Anchor values split across <see cref="AnchorKind" /> plus optional
    ///         <see cref="AnchorOffsetX" /> / <see cref="AnchorOffsetY" /> /
    ///         <see cref="AnchorCustomX" /> / <see cref="AnchorCustomY" />; when <see cref="AnchorKind" />
    ///         is left at <see cref="ModCardPileAnchorKind.StyleDefault" /> the pile auto-stacks per the
    ///         "explicit anchor + auto-stack fallback" rule.
    ///         anchor 值拆分为 <c>AnchorKind</c> 以及可选的 <c>AnchorOffsetX</c> /
    ///         <see cref="AnchorOffsetY" /> / <see cref="AnchorCustomX" /> / <see cref="AnchorCustomY" />；
    ///         当 <c>AnchorKind</c> 保持为 <c>ModCardPileAnchorKind.StyleDefault</c> 时，
    ///         pile 会按“显式 anchor + auto-stack fallback”规则自动堆叠。
    ///     </para>
    ///     <para>
    ///         If the annotated type implements <see cref="IModCardPileHandler" />, ritsulib creates a
    ///         single instance (parameterless constructor required) and wires its
    ///         <see cref="IModCardPileHandler.OnOpen" /> method into
    ///         <see cref="ModCardPileSpec.OnOpen" />.
    ///         如果带注解的类型实现 <c>IModCardPileHandler</c>，ritsulib 会创建一个单例实例
    ///         （需要无参构造函数），并把它的 <c>IModCardPileHandler.OnOpen</c> 方法接入
    ///         <see cref="ModCardPileSpec.OnOpen" />。
    ///     </para>
    /// </remarks>
    /// <param name="localPileStem">
    ///     Local, mod-scoped pile stem (matches <c>RegisterOwned(localStem, ...)</c>).
    ///     mod 局部范围内的 pile stem（匹配 <c>RegisterOwned(localStem, ...)</c>）。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOwnedCardPileAttribute(string localPileStem) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Local, mod-scoped pile stem.
        ///     mod 局部范围内的 pile stem。
        /// </summary>
        public string LocalPileStem { get; } = localPileStem;

        /// <summary>
        ///     Lifetime scope (defaults to <see cref="ModCardPileScope.CombatOnly" />).
        ///     生命周期 scope（默认 <c>ModCardPileScope.CombatOnly</c>）。
        /// </summary>
        public ModCardPileScope Scope { get; set; } = ModCardPileScope.CombatOnly;

        /// <summary>
        ///     UI chrome family (defaults to <see cref="ModCardPileUiStyle.Headless" />).
        ///     UI chrome 类型族（默认 <c>ModCardPileUiStyle.Headless</c>）。
        /// </summary>
        public ModCardPileUiStyle Style { get; set; } = ModCardPileUiStyle.Headless;

        /// <summary>
        ///     Anchor slot hint (defaults to <see cref="ModCardPileAnchorKind.StyleDefault" />).
        ///     anchor slot 提示（默认 <c>ModCardPileAnchorKind.StyleDefault</c>）。
        /// </summary>
        public ModCardPileAnchorKind AnchorKind { get; set; } = ModCardPileAnchorKind.StyleDefault;

        /// <summary>
        ///     Extra X pixels added on top of the resolved anchor position.
        ///     在解析出的 anchor 位置上额外增加的 X 像素。
        /// </summary>
        public float AnchorOffsetX { get; set; }

        /// <summary>
        ///     Extra Y pixels added on top of the resolved anchor position.
        ///     在解析出的 anchor 位置上额外增加的 Y 像素。
        /// </summary>
        public float AnchorOffsetY { get; set; }

        /// <summary>
        ///     X authoring coordinate in the mount parent's local space when <see cref="AnchorKind" /> is
        ///     <see cref="ModCardPileAnchorKind.Custom" /> (paired with <see cref="AnchorCustomPivotX" /> /
        ///     <see cref="AnchorCustomPivotY" /> as chrome landmark fractions — see <see cref="ModCardPileAnchor" />).
        ///     当 <c>AnchorKind</c> 为 <c>ModCardPileAnchorKind.Custom</c> 时，mount parent
        ///     本地空间中的 X authoring 坐标（与作为 chrome landmark fraction 的
        ///     <c>AnchorCustomPivotX</c> / <c>AnchorCustomPivotY</c> 配对；参见
        ///     <see cref="ModCardPileAnchor" />）。
        /// </summary>
        public float AnchorCustomX { get; set; }

        /// <summary>
        ///     Y authoring coordinate when <see cref="AnchorKind" /> is <see cref="ModCardPileAnchorKind.Custom" />.
        ///     当 <c>AnchorKind</c> 为 <c>ModCardPileAnchorKind.Custom</c> 时的 Y authoring 坐标。
        /// </summary>
        public float AnchorCustomY { get; set; }

        /// <summary>
        ///     X-axis fraction (typically 0..1) on nominal chrome horizontal extent when
        ///     <see cref="AnchorKind" /> is <see cref="ModCardPileAnchorKind.Custom" />; default <c>0</c>
        ///     means upper-left anchored.
        ///     当 <c>AnchorKind</c> 为 <c>ModCardPileAnchorKind.Custom</c> 时，名义 chrome
        ///     水平范围上的 X 轴比例（通常为 0..1）；默认 <c>0</c> 表示左上角锚定。
        /// </summary>
        public float AnchorCustomPivotX { get; set; }

        /// <summary>
        ///     Y chrome landmark fraction for <see cref="ModCardPileAnchorKind.Custom" />; default <c>0</c>
        ///     with <see cref="AnchorCustomPivotX" /> behaves like upper-left authoring.
        ///     <c>ModCardPileAnchorKind.Custom</c> 使用的 Y chrome landmark 比例；默认 <c>0</c>
        ///     与 <c>AnchorCustomPivotX</c> 组合时表现为左上角 authoring。
        /// </summary>
        public float AnchorCustomPivotY { get; set; }

        /// <summary>
        ///     Godot resource path for the pile icon (e.g. <c>res://my_mod/icons/my_pile.png</c>).
        ///     pile 图标的 Godot ResourcePath（例如 <c>res://my_mod/icons/my_pile.png</c>）。
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        ///     Optional hotkey ids forwarded to <c>NCardPileScreen.ShowScreen</c>. Separate ids with commas
        ///     at the call site (<c>Hotkeys = new[] { "combat_pile_deck" }</c>).
        ///     转发给 <c>NCardPileScreen.ShowScreen</c> 的可选 hotkey id。在调用处用逗号分隔多个 id
        ///     （<c>Hotkeys = new[] { "combat_pile_deck" }</c>）。
        /// </summary>
        public string[]? Hotkeys { get; set; }

        /// <summary>
        ///     Only meaningful for <see cref="ModCardPileUiStyle.ExtraHand" />: when true, cards added to
        ///     the pile are rendered as <c>NCard</c> nodes inside the pile container.
        ///     仅对 <c>ModCardPileUiStyle.ExtraHand</c> 有意义：为 true 时，加入 pile 的 card 会在
        ///     pile 容器内渲染为 <c>NCard</c> 节点。
        /// </summary>
        public bool CardShouldBeVisible { get; set; }

        /// <summary>
        ///     Added to the hover tip position after automatic placement (see <see cref="ModCardPileSpec.HoverTipScreenOffset" />
        ///     ).
        ///     自动放置后追加到 hover tip 位置的偏移（参见 <c>ModCardPileSpec.HoverTipScreenOffset</c>）。
        /// </summary>
        public float HoverTipOffsetX { get; set; }

        /// <summary>
        ///     Added to the hover tip position after automatic placement (see <see cref="ModCardPileSpec.HoverTipScreenOffset" />
        ///     ).
        ///     自动放置后追加到 hover tip 位置的偏移（参见 <c>ModCardPileSpec.HoverTipScreenOffset</c>）。
        /// </summary>
        public float HoverTipOffsetY { get; set; }

        /// <summary>
        ///     Hover tip anchor relative to the pile button (see <see cref="ModCardPileSpec.HoverTipPlacement" />).
        ///     hover tip 相对于 pile 按钮的 anchor（参见 <c>ModCardPileSpec.HoverTipPlacement</c>）。
        /// </summary>
        public ModCardPileHoverTipPlacement HoverTipPlacement { get; set; }
    }
}
